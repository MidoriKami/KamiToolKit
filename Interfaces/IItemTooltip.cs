namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing an item tooltip.
/// </summary>
public interface IItemTooltip {

    /// <summary>
    /// Gets or sets the Item Tooltip for this node. Uses ItemId.
    /// </summary>
    /// <remarks>
    /// If tooltip is set after the node is attached, a collision update will be required.
    /// </remarks>
    uint ItemTooltip { get; set; }

    /// <summary>
    /// Shows the currently set item as a tooltip.
    /// </summary>
    void ShowItemTooltip();

    /// <summary>
    /// Hides the current tooltip.
    /// </summary>
    void HideItemTooltip();
}
