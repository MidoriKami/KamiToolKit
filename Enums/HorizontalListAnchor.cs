using System.ComponentModel;

namespace KamiToolKit.Enums;

/// <summary>
/// Anchor definitions for <see cref="Nodes.HorizontalListNode"/>
/// </summary>
public enum HorizontalListAnchor {

    /// <summary>
    /// Anchors contents to the left, this is the default.
    /// </summary>
    [Description("Left")]
    Left,

    /// <summary>
    /// Anchor contents to the right.
    /// </summary>
    [Description("Right")]
    Right,
}
