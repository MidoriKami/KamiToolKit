namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing an action tooltip;
/// </summary>
public interface IActionTooltip {

    /// <summary>
    /// Gets or sets the Action Tooltip for this node. Uses ActionId.
    /// </summary>
    /// <remarks>
    /// If tooltip is set after the node is attached, a collision update will be required.
    /// </remarks>
    uint ActionTooltip { get; set; }

    /// <summary>
    /// Shows the currently set action as a tooltip.
    /// </summary>
    void ShowActionTooltip();

    /// <summary>
    /// Hides the current tooltip.
    /// </summary>
    void HideActionTooltip();
}
