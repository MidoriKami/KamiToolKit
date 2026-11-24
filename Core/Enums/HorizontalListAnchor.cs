using System.ComponentModel;
using KamiToolKit.UI.Nodes.Layout;

namespace KamiToolKit.Core.Enums;

/// <summary>
/// Anchor definitions for <see cref="HorizontalListNode"/>
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
