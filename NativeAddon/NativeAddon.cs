using System;
using System.Numerics;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Dalamud;
using KamiToolKit.Nodes;
using KamiToolKit.Timelines;

namespace KamiToolKit;

/// <summary>
/// NativeAddon Partial containing internal and private functions for allocating and initialize addon states.
/// </summary>
public unsafe partial class NativeAddon {
    private GCHandle? disposeHandle;
    internal AtkUnitBase* InternalAddon;

    protected WindowNodeBase? WindowNode { get; private set; }

    /// <summary>
    /// Entry point for allocating custom addons. Allocates memory, replaces virtual table, allocates required nodes, and sets the GC Handle.
    /// </summary>
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
            NodeFlags = NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.Fill | NodeFlags.Focusable | NodeFlags.EmitsEvents,
            IsAddonRootNode = true,
        };

        if (!IsOverlayAddon) {
            WindowNode = CreateWindowNode?.Invoke() ?? new WindowNode();
            WindowNode.NodeId = 2;
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

    /// <summary>
    /// Initializes data components of the addon to sane default values.
    /// </summary>
    private void InitializeAddon() {
        var widgetInfo = NativeMemoryHelper.UiAlloc<AtkUldWidgetInfo>(1, 16);
        widgetInfo->Id = 1;
        widgetInfo->NodeCount = 0;
        widgetInfo->NodeList = null;
        widgetInfo->WidgetAlignment = new AtkWidgetAlignment {
            AlignmentType = AlignmentType.Center,
            X = 50.0f,
            Y = 50.0f,
        };

        InternalAddon->UldManager.Objects = (AtkUldObjectInfo*)widgetInfo;
        InternalAddon->UldManager.ObjectCount = 1;
        InternalAddon->UldManager.ResourceFlags |= AtkUldManagerResourceFlag.ArraysAllocated;

        InternalAddon->RootNode = RootNode;
        InternalAddon->UldManager.AddNodeToObjectList(RootNode);

        LoadTimeline();

        InternalAddon->UldManager.UpdateDrawNodeList();
        InternalAddon->UldManager.LoadedState = AtkLoadState.Loaded;

        if (!IsOverlayAddon && WindowNode is not null) {
            WindowNode.AttachNode(this, NodePosition.AsFirstChild);
            InternalAddon->WindowNode = WindowNode;
            InternalAddon->UldManager.AddNodeToObjectList(WindowNode);
        }

        // UldManager finished loading the uld
        InternalAddon->Flags198 |= 2 << 0x1C;

        // LoadUldByName called
        InternalAddon->Flags1A2 |= 4;

        InternalAddon->UpdateCollisionNodeList(false);

        // Set focus node to allow controller nav
        WindowNode?.WindowHeaderFocusNode.AddNodeFlags(NodeFlags.Focusable);
        InternalAddon->FocusNode = WindowNode is not null ? WindowNode.WindowHeaderFocusNode : RootNode;

        // Now that we have constructed this instance, track it for auto-dispose
        CreatedAddons.Add(this);
    }

    /// <summary>
    /// Before the first OnSetup virtual function is invoked, set various fields such as open SFX, title, and initial position.
    /// </summary>
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

    /// <summary>
    /// Load timelines for WindowNode to properly show focused/unfocused state.
    /// </summary>
    private void LoadTimeline() {
        RootNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 89)
            .AddLabel(1, 101, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(10, 102, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(20, 103, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(30, 104, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(40, 105, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(50, 106, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(60, 107, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(70, 108, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(80, 109, AtkTimelineJumpBehavior.PlayOnce, 0)
            .EndFrameSet()
            .Build());
    }
}
