using System;
using System.Numerics;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Enums;
using KamiToolKit.Internal.Classes;
using KamiToolKit.Timelines;

namespace KamiToolKit.BaseTypes;

public unsafe partial class NativeAddon {

    /// <summary>
    /// OnSetup Callback for an addon, this is called to attach and save references to created nodes.
    /// </summary>
    protected virtual void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValueSpan) { }

    /// <summary>
    /// OnShow Callback for an addon, this is called when the window is opened.
    /// </summary>
    /// <remarks>
    /// KamiToolKit intentionally does not allow hiding addons, so this is only called when it's opened.
    /// </remarks>
    protected virtual void OnShow(AtkUnitBase* addon) { }

    /// <summary>
    /// OnHide Callback for an addon, this is called when the window is opened.
    /// </summary>
    /// <remarks>
    /// KamiToolKit intentionally does not allow hiding addons, so this will then trigger close and then subsequently <see cref="OnFinalize"/>.
    /// </remarks>
    protected virtual void OnHide(AtkUnitBase* addon) { }

    /// <summary>
    /// OnDraw Callback for an addon, this is called every frame the addon is visible.
    /// </summary>
    protected virtual void OnDraw(AtkUnitBase* addon) { }

    /// <summary>
    /// OnUpdate Callback for an addon, this is called every frame the addon exists before its opened, and after it's closed but not finalized yet.
    /// </summary>
    protected virtual void OnUpdate(AtkUnitBase* addon) { }

    /// <summary>
    /// OnFinalize Callback for the addon, this is called immediately before it is deallocated/closed fully.
    /// </summary>
    protected virtual void OnFinalize(AtkUnitBase* addon) { }

    /// <summary>
    /// OnRequestedUpdate Callback for the addon, this is only called if you subscribe to string/number array data entries.
    /// </summary>
    protected virtual void OnRequestedUpdate(AtkUnitBase* addon, NumberArrayData** numberArrayData, StringArrayData** stringArrayData) { }

    /// <summary>
    /// OnRefresh Callback for the addon, the game calls this once on open, and may trigger it under other unknown conditions.
    /// </summary>
    protected virtual void OnRefresh(AtkUnitBase* addon, Span<AtkValue> atkValues) { }

    private void Initialize(AtkUnitBase* thisPtr) {
        IPluginLog.Get().Verbose($"[{InternalName}] Initialize");

        originalVirtualTable->Initialize(thisPtr);

        var widgetInfo = NativeMemoryHelper.UiAlloc<AtkUldWidgetInfo>(1, 16);
        widgetInfo->Id = 1;
        widgetInfo->NodeCount = 0;
        widgetInfo->NodeList = null;
        widgetInfo->WidgetAlignment = new AtkWidgetAlignment {
            AlignmentType = AlignmentType.Center,
            X = 50.0f,
            Y = 50.0f,
        };

        thisPtr->UldManager.InitializeResourceRendererManager();
        InternalAddon->UldManager.ResourceFlags |= AtkUldManagerResourceFlag.Initialized;

        InternalAddon->UldManager.Objects = (AtkUldObjectInfo*)widgetInfo;
        InternalAddon->UldManager.ObjectCount = 1;
        InternalAddon->UldManager.ResourceFlags |= AtkUldManagerResourceFlag.ArraysAllocated;

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

        InternalAddon->RootNode = RootNode;
        InternalAddon->UldManager.AddNodeToObjectList(RootNode);

        if (!IsOverlayAddon && WindowNode is not null) {
            WindowNode.AttachNode(this, NodePosition.AsFirstChild);
            InternalAddon->WindowNode = WindowNode;
            InternalAddon->UldManager.AddNodeToObjectList(WindowNode);
        }

        WindowNode?.WindowHeaderFocusNode.AddNodeFlags(NodeFlags.Focusable);
        InternalAddon->FocusNode = WindowNode is not null ? WindowNode.WindowHeaderFocusNode : RootNode;

        InternalAddon->UldManager.UpdateDrawNodeList();
        InternalAddon->UldManager.LoadedState = AtkLoadState.Loaded;

        InternalAddon->LoadState = AtkUnitBaseLoadState.LoadingUldResource;
        InternalAddon->WasLoadUldByNameCalled = true;
        InternalAddon->UpdateCollisionNodeList(false);

        // Now that we have constructed this instance, track it for auto-dispose
        CreatedAddons.Add(this);
    }

    private void Setup(AtkUnitBase* addon, uint valueCount, AtkValue* values) {
        IPluginLog.Get().Verbose($"[{InternalName}] Setup");

        if (!IsOverlayAddon) {
            SetInitialState();
        }
        else {
            ref var screenSize = ref AtkStage.Instance()->ScreenSize;

            addon->SetScale(1.0f / AtkUnitBase.GetGlobalUIScale(), true);
            addon->SetSize((ushort)screenSize.Width, (ushort)screenSize.Height);
            addon->SetPosition(0, 0);
        }

        originalVirtualTable->OnSetup(addon, valueCount, values);

        try {
            OnSetup(addon, new Span<AtkValue>(values, (int)valueCount));
        }
        catch (Exception e) {
            IPluginLog.Get().Exception(e);
        }

        // Initialize all TextId fields that were attached in OnSetup
        addon->UldManager.SetupTextRecursive();

        isSetup = true;
    }

    private void Show(AtkUnitBase* addon, bool silenceOpenSoundEffect, uint unsetShowHideFlags) {
        IPluginLog.Get().Verbose($"[{InternalName}] Show");

        try {
            OnShow(addon);
        }
        catch (Exception e) {
            IPluginLog.Get().Exception(e);
        }

        originalVirtualTable->Show(addon, silenceOpenSoundEffect, unsetShowHideFlags);
    }

    private void Update(AtkUnitBase* addon, float delta) {
        IPluginLog.Get().Excessive($"[{InternalName}] Update");

        try {
            OnUpdate(addon);
        }
        catch (Exception e) {
            IPluginLog.Get().Exception(e);
        }

        originalVirtualTable->Update(addon, delta);
    }

    private void Draw(AtkUnitBase* addon) {
        IPluginLog.Get().Excessive($"[{InternalName}] Draw");

        try {
            OnDraw(addon);
        }
        catch (Exception e) {
            IPluginLog.Get().Exception(e);
        }

        originalVirtualTable->Draw(addon);
    }

    private void Hide(AtkUnitBase* addon, bool unkBool, bool callHideCallback, uint setShowHideFlags) {
        IPluginLog.Get().Verbose($"[{InternalName}] Hide");

        try {
            OnHide(addon);
        }
        catch (Exception e) {
            IPluginLog.Get().Exception(e);
        }

        SaveAddonConfig();

        originalVirtualTable->Hide(addon, unkBool, callHideCallback, setShowHideFlags);
        originalVirtualTable->Close(addon, false);
    }

    private void Hide2(AtkUnitBase* addon) {
        IPluginLog.Get().Verbose($"[{InternalName}] Hide2");

        originalVirtualTable->Hide2(addon);
    }

    private void Finalizer(AtkUnitBase* addon) {
        IPluginLog.Get().Verbose($"[{InternalName}] Finalize");

        try {
            OnFinalize(addon);
        }
        catch (Exception e) {
            IPluginLog.Get().Exception(e);
        }

        if (RememberClosePosition) {
            LastClosePosition = new Vector2(InternalAddon->X, InternalAddon->Y);
        }

        originalVirtualTable->Finalizer(addon);
        isSetup = false;
    }

    private AtkEventListener* Destructor(AtkUnitBase* addon, byte flags) {
        IPluginLog.Get().Verbose($"[{InternalName}] Destructor");

        var result = originalVirtualTable->Dtor(addon, flags);

        if ((flags & 1) == 1) {
            InternalAddon = null;
            disposeHandle?.Dispose();
            disposeHandle = null;
            CreatedAddons.Remove(this);

            // Free our custom virtual table, the game doesn't know this exists and won't clear it on its own.
            NativeMemoryHelper.Free(modifiedVirtualTable, 0x8 * VirtualTableEntryCount);
        }

        return result;
    }

    private void RequestedUpdate(AtkUnitBase* thisPtr, NumberArrayData** numberArrayData, StringArrayData** stringArrayData) {
        IPluginLog.Get().Verbose($"[{InternalName}] RequestedUpdate");

        // Prevent calls to OnRequestedUpdate before Setup is completed. The game will try to call this after Show but before Setup
        if (isSetup) {
            try {
                OnRequestedUpdate(thisPtr, numberArrayData, stringArrayData);

            }
            catch (Exception e) {
                IPluginLog.Get().Exception(e);
            }
        }

        originalVirtualTable->OnRequestedUpdate(thisPtr, numberArrayData, stringArrayData);
    }

    private bool Refresh(AtkUnitBase* thisPtr, uint valueCount, AtkValue* values) {
        IPluginLog.Get().Verbose($"[{InternalName}] Refresh");

        try {
            OnRefresh(thisPtr, new Span<AtkValue>(values, (int)valueCount));
        }
        catch (Exception e) {
            IPluginLog.Get().Exception(e);
        }

        return originalVirtualTable->OnRefresh(thisPtr,valueCount, values);
    }

    private void ScreenSizeChange(AtkUnitBase* thisPtr, int width, int height) {
        IPluginLog.Get().Verbose($"[{InternalName}] ScreenSizeChange");

        originalVirtualTable->OnScreenSizeChange(thisPtr, width, height);

        if (IsOverlayAddon || IgnoreGlobalScale) {
            thisPtr->SetScale(1.0f / AtkUnitBase.GetGlobalUIScale(), true);
        }
    }

    private bool isSetup;
}
