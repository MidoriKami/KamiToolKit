﻿using System;
using System.Numerics;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Enums;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Classes.TimelineBuilding;
using Newtonsoft.Json;

namespace KamiToolKit.Nodes;

public unsafe class DragDropNode : ComponentNode<AtkComponentDragDrop, AtkUldComponentDataDragDrop> {

    public readonly ImageNode DragDropBackgroundNode;
    public readonly IconNode IconNode;

    public DragDropNode() {
        SetInternalComponentType(ComponentType.DragDrop);

        DragDropBackgroundNode = new SimpleImageNode {
            NodeId = 3,
            Size = new Vector2(44.0f, 44.0f),
            TexturePath = "ui/uld/DragTargetA.tex",
            TextureCoordinates = new Vector2(0.0f, 0.0f),
            TextureSize = new Vector2(44.0f, 44.0f),
            WrapMode = WrapMode.Tile,
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
        };
        DragDropBackgroundNode.AttachNode(this);

        IconNode = new IconNode {
            NodeId = 2, 
            Size = new Vector2(44.0f, 48.0f), 
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
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

    private bool IsDragDropEndRegistered { get; set; }

    /// <summary>
    ///     Event that is triggered when a DragDrop is beginning
    /// </summary>
    public Action<DragDropNode>? OnBegin { get; set; }

    /// <summary>
    ///     Event that is triggered when a DragDrop has finished
    /// </summary>
    public Action<DragDropNode>? OnEnd { get; set; }

    /// <summary>
    ///     Event that is triggered when a compatible DragDrop is dropped onto this node
    /// </summary>
    public Action<DragDropNode, DragDropPayload>? OnPayloadAccepted { get; set; }

    /// <summary>
    ///     Event that is triggered when the item in this drag drop is being dropped onto the world
    /// </summary>
    public Action<DragDropNode>? OnDiscard { get; set; }

    /// <summary>
    ///     Event that is triggered when the item is clicked
    /// </summary>
    public Action<DragDropNode>? OnClicked { get; set; }

    /// <summary>
    ///     Event that is triggered when the item is being moused over
    /// </summary>
    public Action<DragDropNode>? OnRollOver { get; set; }

    /// <summary>
    ///     Event that is triggered when the item is no longer being moused over
    /// </summary>
    public Action<DragDropNode>? OnRollOut { get; set; }

    [JsonProperty] public DragDropPayload Payload { get; set; }

    [JsonProperty] public uint IconId {
        get => IconNode.IconId;
        set {
            IconNode.IconId = value;
            IconNode.IsVisible = value != 0;
        }
    }

    [JsonProperty] public bool IsIconDisabled {
        get => IconNode.IsIconDisabled;
        set => IconNode.IsIconDisabled = value;
    }

    public int Quantity {
        get => int.Parse(Component->GetQuantityText().ToString());
        set => Component->SetQuantity(value);
    }

    [JsonProperty] public string QuantityString {
        get => Component->GetQuantityText().ToString();
        set => Component->SetQuantityText(value);
    }

    [JsonProperty] public DragDropType AcceptedType {
        get => Component->AcceptedType;
        set => Component->AcceptedType = value;
    }

    [JsonProperty] public AtkDragDropInterface.SoundEffectSuppression SoundEffectSuppression {
        get => Component->AtkDragDropInterface.DragDropSoundEffectSuppression;
        set => Component->AtkDragDropInterface.DragDropSoundEffectSuppression = value;
    }

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
    ///     When true, allows left-clicking the item to trigger OnClicked
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

    private void DragDropBeginHandler(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        atkEvent->SetEventIsHandled();
        Payload.ToDragDropInterface(atkEventData->DragDropData.DragDropInterface);
        OnBegin?.Invoke(this);

        if (!IsDragDropEndRegistered) {
            AddEvent(AtkEventType.DragDropEnd, DragDropEndHandler);
            IsDragDropEndRegistered = true;
        }
    }

    public override SeString? Tooltip {
        get;
        set {
            if (value is not null && !value.TextValue.IsNullOrEmpty()) {
                field = value;

                if (!TooltipRegistered) {
                    AddEvent(AtkEventType.DragDropRollOver, ShowTooltip);
                    AddEvent(AtkEventType.DragDropRollOut, HideTooltip);

                    TooltipRegistered = true;
                }
            }
            else if (value is null) {
                if (TooltipRegistered) {
                    RemoveEvent(AtkEventType.DragDropRollOver, ShowTooltip);
                    RemoveEvent(AtkEventType.DragDropRollOut, HideTooltip);

                    TooltipRegistered = false;
                }
            }
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

    /// Clear the payload data and set iconId to zero
    public void Clear() {
        Payload.Clear();
        IconId = 0;
    }

    // Show fancy tooltip for the currently stored data
    public void ShowTooltip(AtkTooltipManager.AtkTooltipType type, ActionKind actionKind) {
        if (AtkStage.Instance()->DragDropManager.IsDragging) return;

        var addon = RaptureAtkUnitManager.Instance()->GetAddonByNode(InternalResNode);
        if (addon is null) return;

        var tooltipArgs = new AtkTooltipManager.AtkTooltipArgs();
        tooltipArgs.Ctor();
        tooltipArgs.ActionArgs.Id = Payload.Int2;
        tooltipArgs.ActionArgs.Kind = (DetailKind)actionKind;

        AtkStage.Instance()->TooltipManager.ShowTooltip(
            AtkTooltipManager.AtkTooltipType.Action,
            addon->Id,
            InternalResNode,
            &tooltipArgs);
    }

    private void LoadTimelines() {
        AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 59)
            .AddLabelPair(1, 10, 1)
            .AddLabelPair(11, 19, 2)
            .AddLabelPair(20, 29, 3)
            .AddLabelPair(30, 39, 7)
            .AddLabelPair(40, 49, 6)
            .AddLabelPair(50, 59, 4)
            .EndFrameSet()
            .Build());
    }
}
