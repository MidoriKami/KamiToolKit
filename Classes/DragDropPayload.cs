using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Text;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Classes;

/// <summary>
/// Data wrapper for a native DragDropPayload.
/// </summary>
public unsafe class DragDropPayload {

    /// <summary>
    /// Gets or sets the Drag Drop Type.
    /// </summary>
    public DragDropType Type { get; set; } = DragDropType.Nothing;

    /// <summary>
    /// Gets or sets the reference index.
    /// </summary>
    public short ReferenceIndex { get; set; }

    /// <remarks> Index (like AtkDragDropInterface.ReferenceIndex), InventoryType, etc. </remarks>
    public int Int1 { get; set; }

    /// <remarks> ActionId, ItemId, EmoteId, InventorySlotIndex, ListIndex, MacroIndex etc. </remarks>
    public int Int2 { get; set; } = -1;

    // unknown usage
    // public ulong Unk8 { get; set; }

    // unknown usage
    // public AtkValue* AtkValue { get; set; }

    /// <summary>
    /// Gets or sets the payload text.
    /// </summary>
    public ReadOnlySeString Text { get; set; }

    // unknown usage
    // public uint Flags { get; set; }

    /// <summary>
    /// Builds a DragDropPayload from the provided DragDropEventInterface.
    /// </summary>
    /// <param name="dragDropInterface">The instance to build the payload from.</param>
    /// <returns>A built DragDropPayload.</returns>
    public static implicit operator DragDropPayload(AtkDragDropInterface* dragDropInterface)
        => FromDragDropInterface(dragDropInterface);

    /// <summary>
    /// Builds a DragDropPayload from the provided DragDropEventInterface.
    /// </summary>
    /// <param name="dragDropInterface">The instance to build the payload from.</param>
    /// <returns>A built DragDropPayload.</returns>
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

    /// <summary>
    /// Populates the provided DragDropInterface with the information from this payload.
    /// </summary>
    /// <param name="dragDropInterface">The instance to populate.</param>
    /// <param name="writeToPayloadContainer">If the params for this payload should be written to the target PayloadContainer.</param>
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

    /// <summary>
    /// Clears this payload and resets values to default.
    /// </summary>
    public void Clear() {
        Type = DragDropType.Nothing;
        ReferenceIndex = 0;
        Int1 = 0;
        Int2 = -1;
        Text = default;
    }
}
