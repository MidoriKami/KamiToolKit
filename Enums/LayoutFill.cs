using System;
using KamiToolKit.Interfaces;

namespace KamiToolKit.Enums;

/// <summary>
/// Enum for controlling <see cref="ILayoutFiller"/>.
/// </summary>
[Flags]
public enum LayoutFill {

    /// <summary>
    /// No layout filling.
    /// </summary>
    Disabled = 0,

    /// <summary>
    /// Fill horizontally.
    /// </summary>
    Horizontal = 1 << 0,

    /// <summary>
    /// Fill vertically.
    /// </summary>
    Vertical = 1 << 1,

    /// <summary>
    /// Fill horizontally and vertically.
    /// </summary>
    Both = Horizontal | Vertical,
}
