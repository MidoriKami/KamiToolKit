using FFXIVClientStructs.FFXIV.Client.System.Input;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a hotbar node.
/// </summary>
public interface IHotbarNode : IDragDropNode {

    /// <summary>
    /// Gets whether this slot is empty.
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    /// Updates the hotbar slot's current state, cost, icon, and various other fields.
    /// </summary>
    void Update();

    /// <summary>
    /// Sets this hotbar slot from the provided payload information.
    /// </summary>
    void SetSlot(DragDropPayload payload);

    /// <summary>
    /// Sets this hotbar slot to the specific type and id.
    /// </summary>
    void SetSlot(DragDropType type, uint id);

    /// <summary>
    /// Sets this hotbar slot to the specific type and id.
    /// </summary>
    void SetSlot(DragDropType type, int id);

    /// <summary>
    /// Clears the data for this hotbar slot.
    /// </summary>
    void ClearSlot();

    /// <summary>
    /// Sets this hotbar slot to the specified action.
    /// </summary>
    void SetAction(uint actionId);

    /// <summary>
    /// Gets or sets the keybind that will activate this slot.
    /// </summary>
    KeySetting? KeyBind { get; set; }
}
