using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Text;
using Lumina.Text.ReadOnly;
using Newtonsoft.Json;

namespace KamiToolKit.Classes;

public unsafe class DragDropPayload {

    [JsonProperty] public DragDropType Type { get; set; } = DragDropType.Nothing;

    [JsonProperty] public short ReferenceIndex { get; set; }

    /// <remarks> Index (like AtkDragDropInterface.ReferenceIndex), InventoryType, etc. </remarks>
    [JsonProperty] public int Int1 { get; set; }

    /// <remarks> ActionId, ItemId, EmoteId, InventorySlotIndex, ListIndex, MacroIndex etc. </remarks>
    [JsonProperty] public int Int2 { get; set; } = -1;

    // unknown usage
    // public ulong Unk8 { get; set; }

    // unknown usage
    // public AtkValue* AtkValue { get; set; }

    public ReadOnlySeString Text { get; set; }

    // unknown usage
    // public uint Flags { get; set; }

    public static DragDropPayload FromDragDropInterface(AtkDragDropInterface* dragDropInterface) {
        var payloadContainer = dragDropInterface->GetPayloadContainer();

        return new DragDropPayload {
            Type = dragDropInterface->DragDropType,
            ReferenceIndex = dragDropInterface->DragDropReferenceIndex,
            Int1 = payloadContainer->Int1,
            Int2 = payloadContainer->Int2,
            Text = new ReadOnlySeString(payloadContainer->Text),
        };
    }

    public void ToDragDropInterface(AtkDragDropInterface* dragDropInterface, bool writeToPayloadContainer = true) {
        dragDropInterface->DragDropType = Type;
        dragDropInterface->DragDropReferenceIndex = ReferenceIndex;

        if (writeToPayloadContainer) {
            var payloadContainer = dragDropInterface->GetPayloadContainer();
            payloadContainer->Clear();
            payloadContainer->Int1 = Int1;
            payloadContainer->Int2 = Int2;

            if (Text.IsEmpty) {
                payloadContainer->Text.Clear();
            }
            else {
                var stringBuilder = new SeStringBuilder().Append(Text);
                payloadContainer->Text.SetString(stringBuilder.GetViewAsSpan());
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
