using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Text;
using Lumina.Text.ReadOnly;
using Newtonsoft.Json;

namespace KamiToolKit.Classes;

public unsafe class DragDropPayload {
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
