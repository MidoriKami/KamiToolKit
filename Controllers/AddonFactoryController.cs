using System;
using System.Runtime.InteropServices;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using InteropGenerator.Runtime;
using KamiToolKit.BaseTypes;
using KamiToolKit.Internal.Classes;

namespace KamiToolKit.Controllers;

/// <summary>
/// Controller intended to interact with the games native Addon Factory system
/// to fully replace a built-in game addon with a custom <see cref="NativeAddon"/>.
/// </summary>
public class AddonFactoryController : IDisposable {

    /// <summary>
    /// Addon name to bind to.
    /// </summary>
    public required string AddonName { get; init; }

    /// <summary>
    /// Function to allocate the <see cref="NativeAddon"/> that will replace the named addon.
    /// </summary>
    /// <remarks>
    /// KamiToolKit will take ownership of the created addon.
    /// </remarks>
    public required Func<NativeAddon> CreateNativeAddonFunction { get; init; }

    /// <summary>
    /// Enables the addon factory replacement.
    /// </summary>
    /// <remarks>
    /// Must be invoked from the main game thread.
    /// </remarks>
    public unsafe void Enable() {
        ThreadSafety.AssertMainThread();

        var factoryInfo = RaptureAtkModule.Instance()->GetAddonFactoryInfo(AddonName);
        if (factoryInfo is null) return;

        originalFactoryCreateAddress = (nint?)factoryInfo->Create;
        pinnedFactoryCreateMethod = CreateCustomAddon;
        factoryInfo->Create = (delegate* unmanaged<RaptureAtkModule*, CStringPointer, uint, AtkValue*, AtkUnitBase*>) Marshal.GetFunctionPointerForDelegate(pinnedFactoryCreateMethod);
    }

    /// <summary>
    /// Disables addon factory replacement and disposes any open replaced addons.
    /// </summary>
    /// <remarks>
    /// Must be invoked from the main game thread.
    /// </remarks>
    public unsafe void Disable() {
        ThreadSafety.AssertMainThread();

        nativeAddon?.Dispose();
        nativeAddon = null;

        var factoryInfo = RaptureAtkModule.Instance()->GetAddonFactoryInfo(AddonName);
        if (factoryInfo is null) return;

        // This is dumb, but the compiler will warn otherwise.
        if (originalFactoryCreateAddress is null) {
            factoryInfo->Create = null;
        }
        else {
            factoryInfo->Create = (delegate* unmanaged<RaptureAtkModule*, CStringPointer, uint, AtkValue*, AtkUnitBase*>) originalFactoryCreateAddress;
        }

        pinnedFactoryCreateMethod = null;
        originalFactoryCreateAddress = null;
    }

    /// <inheritdoc />
    public void Dispose()
        => Disable();

    private unsafe AtkUnitBase* CreateCustomAddon(RaptureAtkModule* raptureAtkModule, CStringPointer addonName, uint valueCount, AtkValue* values) {
        try {

            // We have no reasonable way to reuse the current instance, so dispose the previous and make a new one.
            nativeAddon?.Dispose();

            nativeAddon = CreateNativeAddonFunction();
            nativeAddon.InitializeForAddonFactory(valueCount, values);

            return nativeAddon;
        }
        catch (Exception e) {
            IPluginLog.Get().Exception(e);
        }

        return null;
    }

    private nint? originalFactoryCreateAddress;
    private RaptureAtkModule.AddonFactoryInfo.CreateDelegate? pinnedFactoryCreateMethod;
    private NativeAddon? nativeAddon;
}
