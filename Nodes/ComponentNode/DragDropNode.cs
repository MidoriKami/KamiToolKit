using System;
using System.Numerics;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes.ComponentNode;
using KamiToolKit.Classes;
using KamiToolKit.Enums;
using KamiToolKit.Interfaces;
using KamiToolKit.Internal.Classes;
using KamiToolKit.Nodes.Simplified;
using KamiToolKit.Timelines;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// Implementation of the game's DragDropNode and associated component.
/// </summary>
public unsafe class DragDropNode : ComponentNode<AtkComponentDragDrop, AtkUldComponentDataDragDrop>, IDragDropNode {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public SimpleImageNode DragDropBackgroundNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public IconNode IconNode { get; }

    /// <inheritdoc/>
    public Action<DragDropNode>? OnBegin { get; set; }

    /// <inheritdoc/>
    public Action<DragDropNode>? OnEnd { get; set; }

    /// <inheritdoc/>
    public Action<DragDropNode, DragDropPayload>? OnPayloadAccepted { get; set; }

    /// <inheritdoc/>
    public Action<DragDropNode>? OnDiscard { get; set; }

    /// <inheritdoc/>
    public Action<DragDropNode>? OnClicked { get; set; }

    /// <inheritdoc/>
    public Action<DragDropNode>? OnRollOver { get; set; }

    /// <inheritdoc/>
    public Action<DragDropNode>? OnRollOut { get; set; }

    /// <inheritdoc/>
    public DragDropPayload Payload { get; set; }

    /// <inheritdoc/>
    public uint IconId {
        get => IconNode.IconId;
        set {
            IconNode.IconId = value;
            IconNode.IsVisible = value != 0;
        }
    }

    /// <inheritdoc/>
    public bool IsIconDisabled {
        get => IconNode.IsIconDisabled;
        set => IconNode.IsIconDisabled = value;
    }

    /// <inheritdoc/>
    public int Quantity {
        get => int.Parse(Component->GetQuantityText().ToString());
        set => Component->SetQuantity(value);
    }

    /// <inheritdoc/>
    public ReadOnlySeString QuantityString {
        get => Component->GetQuantityText().ToString();
        set => Component->SetQuantityText(value);
    }

    /// <inheritdoc/>
    public DragDropType AcceptedType {
        get => Component->AcceptedType;
        set => Component->AcceptedType = value;
    }

    /// <inheritdoc/>
    public AtkDragDropInterface.SoundEffectSuppression SoundEffectSuppression {
        get => Component->AtkDragDropInterface.DragDropSoundEffectSuppression;
        set => Component->AtkDragDropInterface.DragDropSoundEffectSuppression = value;
    }

    /// <inheritdoc/>
    public bool IsDraggable {
        get => !Component->Flags.HasFlag(DragDropFlag.Locked);
        set {
            if (value) {
                Component->Flags &= ~DragDropFlag.Locked;
            }
            else {
                Component->Flags |= DragDropFlag.Locked;
            }
        }
    }

    /// <inheritdoc/>
    public bool IsClickable {
        get => Component->Flags.HasFlag(DragDropFlag.Clickable);
        set {
            if (value) {
                Component->Flags |= DragDropFlag.Clickable;
            }
            else {
                Component->Flags &= ~DragDropFlag.Clickable;
            }
        }
    }

    /// <inheritdoc/>
    public bool IsBackgroundShown {
        get => DragDropBackgroundNode.IsVisible;
        set => DragDropBackgroundNode.IsVisible = value;
    }

    /// <inheritdoc/>
    public override ReadOnlySeString TextTooltip {
        get;
        set {
            if (field == value) return;

            field = value;
            switch (value) {
                case { IsEmpty: false } when !TooltipRegistered:
                    AddEvent(AtkEventType.DragDropRollOver, ShowTooltip);
                    AddEvent(AtkEventType.DragDropRollOut, HideTooltip);

                    TooltipRegistered = true;
                    base.TextTooltip = value;
                    break;
            }
        }
    }

    /// <inheritdoc/>
    public void Clear() {
        Payload.Clear();
        IconId = 0;
    }

    /// <inheritdoc/>
    public override void ShowTooltip() {
        if (AtkStage.Instance()->DragDropManager.IsDragging) return;

        base.ShowTooltip();
    }

    /// <summary>
    /// constructs a new instance of <see cref="DragDropNode"/>.
    /// </summary>
    public DragDropNode() {
        SetInternalComponentType(ComponentType.DragDrop);

        DragDropBackgroundNode = new SimpleImageNode {
            NodeId = 3,
            Size = new Vector2(44.0f, 44.0f),
            TextureCoordinates = new Vector2(0.0f, 0.0f),
            TextureSize = new Vector2(44.0f, 44.0f),
            WrapMode = WrapMode.Tile,
            NodeFlags = NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
        };
        DragDropBackgroundNode.LoadTexture("ui/uld/DragTargetA.tex", false);
        DragDropBackgroundNode.AttachNode(this);

        IconNode = new IconNode {
            NodeId = 2,
            Size = new Vector2(44.0f, 48.0f),
            NodeFlags = NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
        };
        IconNode.AttachNode(this);

        LoadTimelines();

        Data->Nodes[0] = IconNode.NodeId;

        AcceptedType = DragDropType.Everything;
        Payload = new DragDropPayload();

        Component->AtkDragDropInterface.DragDropType = DragDropType.Everything;
        Component->AtkDragDropInterface.DragDropReferenceIndex = 0;

        InitializeComponentEvents();

        AddEvent(AtkEventType.DragDropBegin, DragDropBeginHandler);
        AddEvent(AtkEventType.DragDropInsert, DragDropInsertHandler);
        AddEvent(AtkEventType.DragDropDiscard, DragDropDiscardHandler);
        AddEvent(AtkEventType.DragDropClick, DragDropClickHandler);
        AddEvent(AtkEventType.DragDropRollOver, DragDropRollOverHandler);
        AddEvent(AtkEventType.DragDropRollOut, DragDropRollOutHandler);
    }

    /// <inheritdoc/>
    protected override void OnReceiveEvent(AtkComponentBase* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        try {
            if (!IsClickable && eventType is AtkEventType.MouseDown) {
                return;
            }
        }
        catch (Exception e) {
            IPluginLog.Get().Exception(e);
        }

        base.OnReceiveEvent(thisPtr, eventType, eventParam, atkEvent, atkEventData);
    }

    private void DragDropBeginHandler(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        atkEvent->SetEventIsHandled();
        Payload.ToDragDropInterface(atkEventData->DragDropData.DragDropInterface);
        OnBegin?.Invoke(this);

        if (!IsDragDropEndRegistered) {
            AddEvent(AtkEventType.DragDropEnd, DragDropEndHandler);
            IsDragDropEndRegistered = true;
        }
    }

    private void DragDropInsertHandler(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        atkEvent->SetEventIsHandled();

        atkEvent->State.StateFlags |= AtkEventStateFlags.HasReturnFlags;
        atkEvent->State.ReturnFlags = 1;

        var payload = DragDropPayload.FromDragDropInterface(atkEventData->DragDropData.DragDropInterface);

        OnPayloadAccepted?.Invoke(this, payload);
    }

    private void DragDropDiscardHandler(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        atkEvent->SetEventIsHandled();

        atkEvent->State.StateFlags |= AtkEventStateFlags.HasReturnFlags;
        atkEvent->State.ReturnFlags = 1;

        OnDiscard?.Invoke(this);
    }

    private void DragDropEndHandler(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        atkEvent->SetEventIsHandled();
        atkEventData->DragDropData.DragDropInterface->GetPayloadContainer()->Clear();
        OnEnd?.Invoke(this);

        if (IsDragDropEndRegistered) {
            RemoveEvent(AtkEventType.DragDropEnd, DragDropEndHandler);
            IsDragDropEndRegistered = false;
        }
    }

    private void DragDropClickHandler(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        atkEvent->SetEventIsHandled();

        atkEvent->State.StateFlags |= AtkEventStateFlags.HasReturnFlags;
        atkEvent->State.ReturnFlags = 1;

        OnClicked?.Invoke(this);
    }

    private void DragDropRollOverHandler()
        => OnRollOver?.Invoke(this);

    private void DragDropRollOutHandler()
        => OnRollOut?.Invoke(this);

    private void LoadTimelines() => AddTimeline(new TimelineBuilder()
        .BeginFrameSet(1, 59)
        .AddLabelPair(1, 10, 1)
        .AddLabelPair(11, 19, 2)
        .AddLabelPair(20, 29, 3)
        .AddLabelPair(30, 39, 7)
        .AddLabelPair(40, 49, 6)
        .AddLabelPair(50, 59, 4)
        .EndFrameSet()
        .Build()
    );

    private bool IsDragDropEndRegistered { get; set; }
}
