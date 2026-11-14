using KamiToolKit.Classes;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing an inventory item tooltip.
/// </summary>
public interface IInventoryItemTooltip {

    /// <summary>
    /// Gets or sets the Inventory Item Tooltip for this node. Takes an InventoryType and a slot index to represent the item in that slot.
    /// </summary>
    /// <remarks>
    /// If tooltip is set after the node is attached, a collision update will be required.
    /// </remarks>
    InventoryItemTooltip? InventoryItemTooltip { get; set; }

    /// <summary>
    /// Shows the currently set inventory item as a tooltip.
    /// </summary>
    void ShowInventoryItemTooltip();

    /// <summary>
    /// Hides the current tooltip.
    /// </summary>
    void HideInventoryItemTooltip();
}
