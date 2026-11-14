using KamiToolKit.Enums;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing capabilities of a vertical list node.
/// </summary>
public interface IVerticalListNode : ILayoutUpdatable, ILayoutFiller, IStretchWidth {

    /// <summary>
    /// Displays items starting from either the bottom or the top of the list
    /// </summary>
    VerticalListAnchor Anchor { get; set; }

    /// <summary>
    /// Displays items either left aligned or right aligned
    /// </summary>
    VerticalListAlignment Alignment { get; set; }

    /// <summary>
    /// Resizes this layout node to fit the height of the contained nodes.
    /// </summary>
    bool FitContents { get; set; }
}
