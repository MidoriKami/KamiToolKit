using Lumina.Text.ReadOnly;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a text tooltip.
/// </summary>
public interface ITextTooltip {

    /// <summary>
    /// Gets or sets the Text Tooltip for this node.
    /// </summary>
    /// <remarks>
    /// If tooltip is set after the node is attached, a collision update will be required.
    /// </remarks>
    ReadOnlySeString TextTooltip { get; set; }

    /// <summary>
    /// Shows the current text tooltip.
    /// </summary>
    void ShowTextTooltip();

    /// <summary>
    /// Hides the current text tooltip.
    /// </summary>
    void HideTextTooltip();
}
