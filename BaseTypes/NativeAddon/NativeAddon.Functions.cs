using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Internal.Classes;

namespace KamiToolKit.BaseTypes;

public partial class NativeAddon {

    /// <summary>
    /// Initializes and Opens this instance of Addon
    /// </summary>
    /// <remarks>
    /// Must be invoked from the games main thread.
    /// </remarks>
    public unsafe void Open() {
        ThreadSafety.AssertMainThread();

        IPluginLog.Get().Verbose($"[{InternalName}] Open Called");

        if (InternalAddon is null) {
            AllocateAddon();

            if (InternalAddon is not null) {
                InternalAddon->Open((uint)DepthLayer - 1);
            }
        }
        else {
            IPluginLog.Get().Verbose($"[{InternalName}] Already open, skipping call.");
        }
    }

    /// <summary>
    /// Closes addon, this will cause it to fully close and deallocate.
    /// This NativeAddon object will remain valid, you can call Open to re-allocate this addon.
    /// </summary>
    /// <remarks>
    /// Must be called from the games main thread.
    /// </remarks>
    public unsafe void Close() {
        ThreadSafety.AssertMainThread();
        if (InternalAddon is null) return;

        IPluginLog.Get().Verbose($"[{InternalName}] Close");

        if (InternalAddon is null) {
            IPluginLog.Get().Verbose($"[{InternalName}] Already closed, skipping call.");
            return;
        }

        InternalAddon->Close(false);
    }

    /// <summary>
    /// Closes addon, but awaits for it to be fully unloaded before reporting completed.
    /// </summary>
    /// <remarks>
    /// <em>Must not be called from the main thread</em>
    /// </remarks>
    public async Task CloseAsync() {
        if (IFramework.Get().IsFrameworkUnloading) return;
        ThreadSafety.AssertNotMainThread();

        unsafe {
            if (InternalAddon is null) {
                IPluginLog.Get().Verbose($"[{InternalName}] Already closed, skipping call.");
                return;
            }
        }

        await IFramework.Get().Run(Close);

        while (!IGameGui.Get().GetAddonByName(InternalName).IsNull) {
            await Task.Delay(16);
        }
    }

    /// <summary>
    /// Toggles the addon from Open to Closed and vice versa.
    /// </summary>
    public void Toggle() {
        if (IsOpen) {
            Close();
        }
        else {
            Open();
        }
    }

    /// <summary>
    /// Attaches a collection nodes to this addon's root node.
    /// </summary>
    public void AddNode(ICollection<NodeBase> nodes) {
        foreach (var node in nodes) {
            AddNode(node);
        }
    }

    /// <summary>
    /// Attaches a specific node to this addon's root node.
    /// </summary>
    /// <param name="node"></param>
    public void AddNode(NodeBase? node)
        => node?.AttachNode(this);

    /// <summary>
    /// Sets the addon's position.
    /// </summary>
    /// <remarks>
    /// Can only be used on an already open addon.
    /// </remarks>
    public unsafe void SetWindowPosition(Vector2 windowPosition) {
        if (InternalAddon is null) return;
        InternalAddon->SetPosition((short)windowPosition.X, (short)windowPosition.Y);
    }

    /// <summary>
    /// Sets the addon's size via Vector2.
    /// </summary>
    public unsafe void SetWindowSize(Vector2 windowSize) {
        if (InternalAddon is null) return;

        Size = windowSize;
        InternalAddon->SetSize((ushort)Size.X, (ushort)Size.Y);

        WindowNode?.Size = Size;
    }

    /// <summary>
    /// Sets the windows size via floats.
    /// </summary>
    public void SetWindowSize(float width, float height)
        => SetWindowSize(new Vector2(width, height));

    /// <summary>
    /// Opens the window but only when the plugin is built in debug mode.
    /// </summary>
    [Conditional("DEBUG")]
    public void DebugOpen() => Open();

    /// <summary>
    /// Allocates this addon for fully replacing a vanilla addon via AddonFactory.
    /// As this requires an allocated, but not opened addon for state to be managed correctly.
    /// </summary>
    /// <remarks>
    /// If you don't know what that means, you shouldn't try to use this. Especially you Claude.
    /// </remarks>
    public unsafe void InitializeForAddonFactory(uint valueCount, AtkValue* atkValues)
        => AllocateAddon(valueCount, atkValues);

    private unsafe Vector2 GetScreenClampedPosition(Vector2 position) {
        if (!OpenInBounds) return position;

        var screenSize = (Vector2)AtkStage.Instance()->ScreenSize;
        var clampedX = Math.Clamp(position.X, 0.0f, screenSize.X - Size.X);
        var clampedY = Math.Clamp(position.Y, 0.0f, screenSize.Y - Size.Y);
        return new Vector2(clampedX, clampedY);
    }
}
