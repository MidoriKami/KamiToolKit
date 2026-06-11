using System.ComponentModel;

namespace KamiToolKit.Enums;

/// <summary>
/// 2-dimensional anchor positioning. For use with <see cref="Nodes.ListBoxNode"/>
/// </summary>
public enum LayoutAnchor {
    /// <summary>
    /// Anchor to the top left corner.
    /// </summary>
    [Description("Top Left")]
    TopLeft,

    /// <summary>
    /// Anchor to the top right corner.
    /// </summary>
    [Description("Top Right")]
    TopRight,

    /// <summary>
    /// Anchor to the bottom left corner.
    /// </summary>
    [Description("Bottom Left")]
    BottomLeft,

    /// <summary>
    /// Anchor to the bottom right corner.
    /// </summary>
    [Description("Bottom Right")]
    BottomRight,
}
