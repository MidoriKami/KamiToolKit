using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Classes.Timelines;
using KamiToolKit.Nodes;

namespace KamiToolKit;

public abstract unsafe partial class NativeAddon {

    private GCHandle? disposeHandle;

    internal AtkUnitBase* InternalAddon;

    public ResNode RootNode = null!;

    protected WindowNodeBase WindowNode { get; set; } = null!;

    private void AllocateAddon() {
        if (InternalAddon is not null) {
            Log.Warning("Tried to allocate addon that was already allocated.");
            return;
        }

        var currentAddonCount = RaptureAtkUnitManager.Instance()->AllLoadedUnitsList.Count;
        if (currentAddonCount >= 200) {
            Log.Warning($"WARNING: Current Addon Count is approaching hard limits ({currentAddonCount}/250). Please ensure custom Addons are not being used as overlays.");
        }

        if (currentAddonCount >= 225) {
            Log.Error($"ERROR: Current Addon Count is too high. Aborting allocation ({currentAddonCount}/250).");
            return;
        }

        if (InternalName.Length is 0) {
            throw new NullReferenceException("InternalName is empty, this is not allowed.");
        }

        Log.Verbose($"[{InternalName}] Allocating NativeAddon");

        InitializeExtras();

        InternalAddon = NativeMemoryHelper.Create<AtkUnitBase>();

        RegisterVirtualTable();

        RootNode = new ResNode {
            NodeId = 1, 
            NodeFlags = NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.Fill | NodeFlags.Focusable | NodeFlags.EmitsEvents,
            IsAddonRootNode = true,
        };

        WindowNode = CreateWindowNode?.Invoke() ?? new WindowNode { NodeId = 2 };

        InternalAddon->NameString = InternalName;

        InternalAddon->OpenSoundEffectId = (short)OpenWindowSoundEffectId;

        UpdateFlags();
    }

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

        WindowNode.AttachNode(this, NodePosition.AsFirstChild);
        InternalAddon->WindowNode = WindowNode;
        InternalAddon->UldManager.AddNodeToObjectList(WindowNode);

        // UldManager finished loading the uld
        InternalAddon->Flags198 |= 2 << 0x1C;

        // LoadUldByName called
        InternalAddon->Flags1A2 |= 4;

        InternalAddon->UpdateCollisionNodeList(false);

        // Now that we have constructed this instance, track it for auto-dispose
        CreatedAddons.Add(this);
    }

    private void SetInitialState() {
        WindowNode.SetTitle(Title.ToString(), Subtitle.ToString());

        InternalAddon->OpenSoundEffectId = (short)OpenWindowSoundEffectId;

        var addonConfig = LoadAddonConfig();
        if (addonConfig.Position != Vector2.Zero) {
            InternalAddon->SetPosition((short)addonConfig.Position.X, (short)addonConfig.Position.Y);
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

        InternalAddon->SetSize((ushort)Size.X, (ushort)Size.Y);
        WindowNode.Size = Size;

        if (LastClosePosition != Vector2.Zero && RememberClosePosition) {
            InternalAddon->SetPosition((short)LastClosePosition.X, (short)LastClosePosition.Y);
        }
    }

    public Func<WindowNodeBase>? CreateWindowNode { get; init; }

    /// <summary>
    ///     Initializes and Opens this instance of Addon
    /// </summary>
    public void Open() => DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
        Log.Verbose($"[{InternalName}] Open Called");

        if (InternalAddon is null) {
            AllocateAddon();

            if (InternalAddon is not null) {
                AtkStage.Instance()->RaptureAtkUnitManager->InitializeAddon(InternalAddon, InternalName);
                InternalAddon->Open((uint)DepthLayer - 1);
                disposeHandle = GCHandle.Alloc(this);
            }
        }
        else {
            Log.Verbose($"[{InternalName}] Already open, skipping call.");
        }
    });

    [Conditional("DEBUG")]
    public void DebugOpen() => Open();

    public void Close() {
        if (InternalAddon is null) return;

        DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
            Log.Verbose($"[{InternalName}] Close");

            if (InternalAddon is not null) {
                InternalAddon->Close(false);
            }
        });
    }

    public void Toggle() {
        if (IsOpen) {
            Close();
        }
        else {
            Open();
        }
    }

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
