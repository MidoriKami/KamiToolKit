using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes.ComponentNode;
using KamiToolKit.Classes;
using KamiToolKit.Enums;
using KamiToolKit.Nodes.Simplified;
using KamiToolKit.Timelines;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// Implementation of the games DragDropNode and associated component.
/// </summary>
public unsafe class DragDropNode : ComponentNode<AtkComponentDragDrop, AtkUldComponentDataDragDrop> {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ImageNode DragDropBackgroundNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public IconNode IconNode { get; }

    /// <summary>
    /// Action that is triggered when a DragDrop is beginning
    /// </summary>
    public Action<DragDropNode>? OnBegin { get; set; }

    /// <summary>
    /// Action that is triggered when a DragDrop has finished
    /// </summary>
    public Action<DragDropNode>? OnEnd { get; set; }

    /// <summary>
    /// Action that is triggered when a compatible DragDrop is dropped onto this node
    /// </summary>
    public Action<DragDropNode, DragDropPayload>? OnPayloadAccepted { get; set; }

    /// <summary>
    /// Action that is triggered when the item in this drag drop is being dropped onto the world
    /// </summary>
    public Action<DragDropNode>? OnDiscard { get; set; }

    /// <summary>
    /// Action that is triggered when the item is clicked
    /// </summary>
    public Action<DragDropNode>? OnClicked { get; set; }

    /// <summary>
    /// Action that is triggered when the item is being moused over
    /// </summary>
    public Action<DragDropNode>? OnRollOver { get; set; }

    /// <summary>
    /// Action that is triggered when the item is no longer being moused over
    /// </summary>
    public Action<DragDropNode>? OnRollOut { get; set; }

    /// <summary>
    /// Gets or sets the DragDrop Payload.
    /// </summary>
    public DragDropPayload Payload { get; set; }

    /// <summary>
    /// Gets or sets the displayed IconId.
    /// </summary>
    public uint IconId {
        get => IconNode.IconId;
        set {
            IconNode.IconId = value;
            IconNode.IsVisible = value != 0;
        }
    }

    /// <summary>
    /// Gets or sets if the icon is disabled.
    /// </summary>
    public bool IsIconDisabled {
        get => IconNode.IsIconDisabled;
        set => IconNode.IsIconDisabled = value;
    }

    /// <summary>
    /// Gets or sets the quantity value.
    /// </summary>
    /// <remarks>
    /// Value is actually stores as a string, this incurs parsing costs.
    /// </remarks>
    public int Quantity {
        get => int.Parse(Component->GetQuantityText().ToString());
        set => Component->SetQuantity(value);
    }

    /// <summary>
    /// Gets or sets the quantity text value.
    /// </summary>
    public ReadOnlySeString QuantityString {
        get => Component->GetQuantityText().ToString();
        set => Component->SetQuantityText(value);
    }

    /// <summary>
    /// Gets or sets the accepted payload types for this node.
    /// </summary>
    public DragDropType AcceptedType {
        get => Component->AcceptedType;
        set => Component->AcceptedType = value;
    }

    /// <summary>
    /// Gets or sets the sound effect suppression values for the DragDrop.
    /// </summary>
    public AtkDragDropInterface.SoundEffectSuppression SoundEffectSuppression {
        get => Component->AtkDragDropInterface.DragDropSoundEffectSuppression;
        set => Component->AtkDragDropInterface.DragDropSoundEffectSuppression = value;
    }

    /// <summary>
    /// Gets or sets whether the icon is draggable.
    /// </summary>
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

    /// <summary>
    /// When true, allows left-clicking the item to trigger OnClicked
    /// </summary>
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

    /// <summary>
    /// Gets or sets the text tooltip for this node.
    /// </summary>
    public override ReadOnlySeString TextTooltip {
        get;
        set {
            field = value;
            switch (value) {
                case { IsEmpty: false } when !TooltipRegistered:
                    AddEvent(AtkEventType.DragDropRollOver, ShowTooltip);
                    AddEvent(AtkEventType.DragDropRollOut, HideTooltip);

                    TooltipRegistered = true;
                    break;
            }
        }
    }

    /// Clear the payload data and set iconId to zero
    public void Clear() {
        Payload.Clear();
        IconId = 0;
    }

    // Show fancy tooltip for the currently stored data
    public override void ShowTooltip() {
        if (AtkStage.Instance()->DragDropManager.IsDragging) return;
        ActionTooltip = (uint)Payload.Int2;

        base.ShowTooltip();
    }

    public DragDropNode() {
        SetInternalComponentType(ComponentType.DragDrop);

        DragDropBackgroundNode = new SimpleImageNode {
            NodeId = 3,
            Size = new Vector2(44.0f, 44.0f),
            TexturePath = "ui/uld/DragTargetA.tex",
            TextureCoordinates = new Vector2(0.0f, 0.0f),
            TextureSize = new Vector2(44.0f, 44.0f),
            WrapMode = WrapMode.Tile,
            NodeFlags = NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
        };
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

        Payload.Clear();
        IconId = 0;

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
