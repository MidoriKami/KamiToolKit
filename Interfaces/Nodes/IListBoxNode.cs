using System.Numerics;
using KamiToolKit.Enums;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a list box node with border and background customization.
/// </summary>
public interface IListBoxNode : ILayoutNode {

    /// <summary>
    /// Gets or sets the layout anchor position.
    /// </summary>
    LayoutAnchor LayoutAnchor { get; set; }

    /// <summary>
    /// Gets or sets whether the node should resize to fit its contents.
    /// </summary>
    bool FitContents { get; set; }

    /// <summary>
    /// Gets or sets which direction nodes will travel relative to the layout anchor.
    /// </summary>
    LayoutOrientation LayoutOrientation { get; set; }

    /// <summary>
    /// Gets or sets the background color.
    /// </summary>
    Vector4 BackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets whether the background color is shown.
    /// </summary>
    bool ShowBackground { get; set; }

    /// <summary>
    /// Gets or sets whether the border is shown.
    /// </summary>
    bool ShowBorder { get; set; }
}
