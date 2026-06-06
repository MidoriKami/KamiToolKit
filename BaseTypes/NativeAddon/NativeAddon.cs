using System;
using System.Numerics;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Internal.Classes;
using KamiToolKit.Nodes;

namespace KamiToolKit.BaseTypes;

/// <summary>
/// Implementation of a custom native addon (AtkUnitBase).
/// </summary>
public unsafe partial class NativeAddon {

    /// <summary>
    /// Triggers a window update via AtkUnitBase.SetSize(...), to have the game recalculate NodeFlags.Anchor{Direction}.
    /// </summary>
    protected void UpdateAnchoringLayout()
        => SetWindowSize(Size);

    /// <summary>
    /// Pointer to the contained addon. This is also accessible via <see cref="op_Implicit"/>
    /// </summary>
    protected internal AtkUnitBase* InternalAddon { get; private set; }

    /// <summary>
    /// Converts this instance to a AtkUnitBase for seamless game interop.
    /// </summary>
    public static implicit operator AtkUnitBase*(NativeAddon addon) => addon.InternalAddon;

    /// <summary>
    /// Window node for this NativeAddon. May be null if <see cref="CreateWindowNode"/> returns a null windowNode
    /// </summary>
    protected WindowNodeBase? WindowNode { get; private set; }

    /// <summary>
    /// Gets or inits a function to be called for creating the window with a custom window node.
    /// </summary>
    public Func<WindowNodeBase>? CreateWindowNode { get; init; }

    /// <summary>
    /// Root node for this NativeAddon.
    /// </summary>
    public ResNode RootNode { get; private set; } = null!;

    private void AllocateAddon(uint atkValueCount = 0, AtkValue* atkValues = null) {
        if (InternalAddon is not null) {
            Services.Log.Warning("Tried to allocate addon that was already allocated.");
            return;
        }

        var currentAddonCount = RaptureAtkUnitManager.Instance()->AllLoadedUnitsList.Count;
        if (currentAddonCount >= 200) {
            Services.Log.Warning($"WARNING: Current Addon Count is approaching hard limits ({currentAddonCount}/250). Please ensure custom Addons are not being used as overlays.");
        }

        if (currentAddonCount >= 225) {
            Services.Log.Error($"ERROR: Current Addon Count is too high. Aborting allocation ({currentAddonCount}/250).");
            return;
        }

        if (InternalName.Length is 0) {
            throw new NullReferenceException("InternalName is empty, this is not allowed.");
        }

        Services.Log.Verbose($"[{InternalName}] Allocating NativeAddon");

        InternalAddon = NativeMemoryHelper.Create<AtkUnitBase>();

        RegisterVirtualTable();

        RootNode = new ResNode {
            NodeId = 1,
            NodeFlags = NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.Fill,
            IsAddonRootNode = true,
        };

        if (!IsOverlayAddon) {
            WindowNode = CreateWindowNode?.Invoke() ?? new WindowNode { NodeId = 2 };
        }

        InternalAddon->NameString = InternalName;

        InternalAddon->ShowSoundEffectId = (short)OpenWindowSoundEffectId;

        UpdateFlags();

        disposeHandle = GCHandle.Alloc(this);

        var localRef = InternalAddon;
        using var nameString = new Utf8String(InternalName);

        AtkStage.Instance()->RaptureAtkUnitManager->InitializeAddon(&localRef, nameString.StringPtr, atkValueCount, atkValues);

        if (localRef is null) {
            Dispose();
            throw new Exception("Failed to initialize addon!");
        }
    }

    private void SetInitialState() {
        WindowNode?.SetTitle(Title.ToString(), Subtitle?.ToString() ?? KamiToolKitLibrary.DefaultWindowSubtitle);

        InternalAddon->ShowSoundEffectId = (short)OpenWindowSoundEffectId;

        var addonConfig = LoadAddonConfig();
        if (addonConfig.Position != Vector2.Zero && RememberClosePosition) {
            var clampedPosition = GetScreenClampedPosition(addonConfig.Position);
            InternalAddon->SetPosition((short)clampedPosition.X, (short)clampedPosition.Y);
        }
        else {
            var screenSize = new Vector2(AtkStage.Instance()->ScreenSize.Width, AtkStage.Instance()->ScreenSize.Height);
            var defaultPosition = screenSize / 2.0f - Size / 2.0f;
            InternalAddon->SetPosition((short)defaultPosition.X, (short)defaultPosition.Y);
        }

        if (addonConfig.Scale is not 1.0f) {
            var newScale = Math.Clamp(addonConfig.Scale, 0.25f, 6.0f);

            InternalAddon->SetScale(newScale, true);
        }

        SetWindowSize(Size);

        if (LastClosePosition != Vector2.Zero && RememberClosePosition) {
            var clampedPosition = GetScreenClampedPosition(LastClosePosition);
            InternalAddon->SetPosition((short)clampedPosition.X, (short)clampedPosition.Y);
        }
    }

    private GCHandle? disposeHandle;
}
