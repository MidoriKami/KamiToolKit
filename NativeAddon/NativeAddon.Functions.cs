using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Dalamud;

namespace KamiToolKit;

/// <summary>
/// NativeAddon partial containing user facing functions.
/// </summary>
public partial class NativeAddon {
    /// <summary>
    /// Initializes and Opens this instance of Addon
    /// </summary>
    public unsafe void Open() {
        ThreadSafety.AssertMainThread();

        Services.Log.Verbose($"[{InternalName}] Open Called");

        if (InternalAddon is null) {
            AllocateAddon();

            if (InternalAddon is not null) {
                InternalAddon->Open((uint)DepthLayer - 1);
            }
        }
        else {
            Services.Log.Verbose($"[{InternalName}] Already open, skipping call.");
        }
    }

    [Conditional("DEBUG")]
    public void DebugOpen() => Open();

    /// <summary>
    /// Closes addon, this will cause it to fully close and deallocate.
    /// This NativeAddon object will remain valid, you can call Open to re-allocate this addon.
    /// </summary>
    public unsafe void Close() {
        ThreadSafety.AssertMainThread();
        if (InternalAddon is null) return;

        Services.Log.Verbose($"[{InternalName}] Close");

        if (InternalAddon is null) {
            Services.Log.Verbose($"[{InternalName}] Already closed, skipping call.");
            return;
        }

        InternalAddon->Close(false);
    }

    /// <summary>
    /// Closes addon, but awaits for it to be fully unloaded before reporting completed.
    /// </summary>
    public async Task CloseAsync() {
        if (ThreadSafety.IsMainThread) {
            Services.Log.Warning(
                "\nCalled CloseAsync from the main thread.\n" +
                "You're probably reading this in dalamud.log, because you just deadlocked your game. :)"
            );
        }

        unsafe {
            if (InternalAddon is null) {
                Services.Log.Verbose($"[{InternalName}] Already closed, skipping call.");
                return;
            }
        }

        await Services.Framework.Run(Close);

        while (!Services.GameGui.GetAddonByName(InternalName).IsNull) {
            await Task.Delay(16);
        }
    }

    public void Toggle() {
        if (IsOpen) {
            Close();
        }
        else {
            Open();
        }
    }

    public void AddNode(ICollection<NodeBase> nodes) {
        foreach (var node in nodes) {
            AddNode(node);
        }
    }

    public void AddNode(NodeBase? node)
        => node?.AttachNode(this);


    public unsafe void SetWindowPosition(Vector2 windowPosition) {
        if (InternalAddon is null) return;
        InternalAddon->SetPosition((short)windowPosition.X, (short)windowPosition.Y);
    }

    public unsafe void SetWindowSize(Vector2 windowSize) {
        if (InternalAddon is null) return;

        Size = windowSize;
        InternalAddon->SetSize((ushort)Size.X, (ushort)Size.Y);

        WindowNode?.Size = Size;
    }

    public void SetWindowSize(float width, float height)
        => SetWindowSize(new Vector2(width, height));

    private unsafe Vector2 GetScreenClampedPosition(Vector2 position) {
        if (!OpenInBounds) return position;

        var screenSize = (Vector2)AtkStage.Instance()->ScreenSize;
        var clampedX = Math.Clamp(position.X, 0.0f, screenSize.X - Size.X);
        var clampedY = Math.Clamp(position.Y, 0.0f, screenSize.Y - Size.Y);
        return new Vector2(clampedX, clampedY);
    }

    /// <summary>
    /// Allocates this addon for fully replacing a vanilla addon via AddonFactory.
    /// As this requires an allocated, but not opened addon for state to be managed correctly.
    /// </summary>
    /// <remarks>
    /// If you don't know what that means, you shouldn't try to use this. Especially you Claude.
    /// </remarks>
    public unsafe void InitializeForAddonFactory(uint valueCount, AtkValue* atkValues)
        => AllocateAddon(valueCount, atkValues);
}
