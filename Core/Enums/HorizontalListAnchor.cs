using System.ComponentModel;

namespace KamiToolKit.Core.Enums;

/// <summary>
/// Anchor definitions for <see cref="KamiToolKit.UI.Nodes.HorizontalListNode"/>
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
