using KamiToolKit.Enums;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a list node oriented horizontally.
/// </summary>
public interface IHorizontalListNode : ILayoutFiller, IControllerNavigable {

    /// <summary>
    /// Gets or sets the alignment used when calculating layout.
    /// </summary>
    /// <remarks>
    /// Setting triggers layout recalculation.
    /// </remarks>
    HorizontalListAnchor Alignment { get; set; }

    /// <summary>
    /// Adjusts contained nodes heights to match this node's height
    /// </summary>
    bool FitHeight { get; set; }

    /// <summary>
    /// Resizes the horizontal list node to fit the height of all contents
    /// </summary>
    bool FitToContentHeight { get; set; }

    /// <summary>
    /// Resizes the horizontal list node to fit the width of all contents
    /// </summary>
    bool FitToContentWidth { get; set; }

    /// <summary>
    /// Gets the amount of space remaining in this node.
    /// </summary>
    float AreaRemaining { get; }
}
