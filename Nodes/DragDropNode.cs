using System;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.TimelineBuilding;
using KamiToolKit.Extensions;
using Lumina.Text;
using Lumina.Text.ReadOnly;
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
			WrapMode = 1,
			ImageNodeFlags = 0,
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

		AcceptedType = DragDropType.Nothing;
		Payload = new();

		Component->AtkDragDropInterface.DragDropType = DragDropType.Nothing;
		Component->AtkDragDropInterface.DragDropReferenceIndex = 0;

		InitializeComponentEvents();

		AddEvent(AddonEventType.DragDropBegin, DragDropBeginHandler);
		AddEvent(AddonEventType.DragDropInsert, DragDropInsertHandler);
		AddEvent(AddonEventType.DragDropDiscard, DragDropDiscardHandler);
		AddEvent(AddonEventType.DragDropEnd, DragDropEndHandler);
		AddEvent(AddonEventType.DragDropCancel, DragDropCancelHandler);
	}

	private void DragDropBeginHandler(AddonEventData data) {
		data.SetHandled();
		Payload.ToDragDropInterface(data.GetDragDropData().DragDropInterface);
		OnBegin?.Invoke(this, data);
	}

	private void DragDropInsertHandler(AddonEventData data) {
		data.SetHandled();

		var evt = (AtkEvent*)data.AtkEventPointer;
		evt->State.StateFlags |= AtkEventStateFlags.HasReturnFlags;
		evt->State.ReturnFlags = 1;

		var payload = DragDropPayload.FromDragDropInterface(data.GetDragDropData().DragDropInterface);

		if(AcceptedType.Accepts(payload.Type)) {
			OnPayloadAccepted?.Invoke(this, data, payload);
		}
		else {
			OnPayloadRejected?.Invoke(this, data, payload);
		}
	}

	private void DragDropDiscardHandler(AddonEventData data) {
		data.SetHandled();

		var evt = (AtkEvent*)data.AtkEventPointer;
		evt->State.StateFlags |= AtkEventStateFlags.HasReturnFlags;
		evt->State.ReturnFlags = 1;

		OnDiscard?.Invoke(this, data);
	}

	private void DragDropEndHandler(AddonEventData data) {
		data.SetHandled();
		data.GetDragDropData().DragDropInterface->GetPayloadContainer()->Clear();
		OnEnd?.Invoke(this, data);
	}

	private void DragDropCancelHandler(AddonEventData data) {
		data.SetHandled();

		var evt = (AtkEvent*)data.AtkEventPointer;
		evt->State.StateFlags |= AtkEventStateFlags.HasReturnFlags;
		evt->State.ReturnFlags = 1;

		OnClicked?.Invoke(this, data);
	}

	public Action<DragDropNode, AddonEventData>? OnBegin { get; set; }
	public Action<DragDropNode, AddonEventData>? OnEnd { get; set; }
	public Action<DragDropNode, AddonEventData, DragDropPayload>? OnPayloadAccepted { get; set; }
	public Action<DragDropNode, AddonEventData, DragDropPayload>? OnPayloadRejected { get; set; }
	public Action<DragDropNode, AddonEventData>? OnDiscard { get; set; }
	public Action<DragDropNode, AddonEventData>? OnClicked { get; set; }

	[JsonProperty]
	public DragDropPayload Payload { get; set; }

	[JsonProperty]
	public uint IconId {
		get => IconNode.IconId;
		set {
			IconNode.IconId = value;
			IconNode.IsVisible = value != 0;
		}
	}

	[JsonProperty]
	public bool IsIconDisabled {
		get => IconNode.IsIconDisabled;
		set => IconNode.IsIconDisabled = value;
	}

	public int Quantity {
		get => int.Parse(Component->GetQuantityText().ToString());
		set => Component->SetQuantity(value);
	}

	[JsonProperty]
	public string QuantityText {
		get => Component->GetQuantityText().ToString();
		set => Component->SetQuantityText(value);
	}

	[JsonProperty]
	public DragDropType AcceptedType {
		get => Component->AcceptedType;
		set => Component->AcceptedType = value;
	}

	[JsonProperty]
	public AtkDragDropInterface.SoundEffectSuppression SoundEffectSuppression {
		get => Component->AtkDragDropInterface.DragDropSoundEffectSuppression;
		set => Component->AtkDragDropInterface.DragDropSoundEffectSuppression = value;
	}

	public bool IsClickable {
		get => Component->Flags.HasFlag(DragDropFlag.Clickable);
		set => Component->Flags |= DragDropFlag.Clickable;
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

public unsafe record DragDropPayload {
	#region AtkDragDropInterface

	[JsonProperty]
	public DragDropType Type { get; set; } = DragDropType.Nothing;

	[JsonProperty]
	public short ReferenceIndex { get; set; } = 0;

	#endregion

	#region AtkDragDropPayloadContainer

	/// <remarks> Index (like AtkDragDropInterface.ReferenceIndex), InventoryType, etc. </remarks>
	[JsonProperty]
	public int Int1 { get; set; } = 0;

	/// <remarks> ActionId, ItemId, EmoteId, InventorySlotIndex, ListIndex, MacroIndex etc. </remarks>
	[JsonProperty]
	public int Int2 { get; set; } = -1;

	// unknown usage
	// public ulong Unk8 { get; set; }

	// unknown usage
	// public AtkValue* AtkValue { get; set; }

	// TODO: not sure if [JsonProperty] works here
	public ReadOnlySeString Text { get; set; } = default;

	// unknown usage
	// public uint Flags { get; set; }

	#endregion

	public static DragDropPayload FromDragDropInterface(AtkDragDropInterface* ddi) {
		var payloadContainer = ddi->GetPayloadContainer();

		return new DragDropPayload {
			Type = ddi->DragDropType,
			ReferenceIndex = ddi->DragDropReferenceIndex,
			Int1 = payloadContainer->Int1,
			Int2 = payloadContainer->Int2,
			Text = new ReadOnlySeString(payloadContainer->Text)
		};
	}

	public void ToDragDropInterface(AtkDragDropInterface* ddi, bool writeToPayloadContainer = true) {
		ddi->DragDropType = Type;
		ddi->DragDropReferenceIndex = ReferenceIndex;

		if(writeToPayloadContainer) {
			var payloadContainer = ddi->GetPayloadContainer();
			payloadContainer->Clear();
			payloadContainer->Int1 = Int1;
			payloadContainer->Int2 = Int2;

			if(Text.IsEmpty) {
				payloadContainer->Text.Clear();
			}
			else {
				var sb = new SeStringBuilder().Append(Text);
				payloadContainer->Text.SetString(sb.GetViewAsSpan());
			}
		}
	}

	public void Clear() {
		Type = DragDropType.Nothing;
		ReferenceIndex = 0;
		Int1 = 0;
		Int2 = -1;
		Text = default;
	}
}
