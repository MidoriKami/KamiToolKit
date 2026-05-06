using System;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using InteropGenerator.Runtime;
using KamiToolKit.Dalamud;

namespace KamiToolKit.Controllers;

/// <summary>
/// Controller intended to interact with the games native Addon Factory system
/// to fully replace a built-in game addon with a custom <see cref="NativeAddon"/>.
/// </summary>
public unsafe class AddonFactoryController : IDisposable {
    public required string AddonName { get; init; }

    private nint? originalFactoryCreateAddress;
    private RaptureAtkModule.AddonFactoryInfo.CreateDelegate? pinnedFactoryCreateMethod;
    private NativeAddon? nativeAddon;

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
    public void Enable() => Services.Framework.RunOnFrameworkThread(() => {
        var factoryInfo = RaptureAtkModule.Instance()->GetAddonFactoryInfo(AddonName);
        if (factoryInfo is null) return;

        originalFactoryCreateAddress = (nint?)factoryInfo->Create;
        pinnedFactoryCreateMethod = CreateCustomAddon;
        factoryInfo->Create = (delegate* unmanaged<RaptureAtkModule*, CStringPointer, uint, AtkValue*, AtkUnitBase*>) Marshal.GetFunctionPointerForDelegate(pinnedFactoryCreateMethod);
    });

    /// <summary>
    /// Disables addon factory replacement and disposes any open replaced addons.
    /// </summary>
    public void Disable() {
        nativeAddon?.Dispose();
        nativeAddon = null;

        Services.Framework.RunOnFrameworkThread(() => {
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
        });
    }

    private AtkUnitBase* CreateCustomAddon(RaptureAtkModule* raptureAtkModule, CStringPointer addonName, uint valueCount, AtkValue* values) {
        try {
            nativeAddon?.Dispose();
            nativeAddon = CreateNativeAddonFunction();
            nativeAddon.InitializeForAddonFactory(valueCount, values);

            return nativeAddon;
        }
        catch (Exception e) {
            Services.Log.Exception(e);
        }

        return null;
    }

    public void Dispose()
        => Disable();
}
