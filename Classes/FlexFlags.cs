using System;

namespace KamiToolKit.Classes;

[Flags]
public enum FlexFlags {
    /// <summary>
    ///     Adjusts the height of the inserted node to be the same as the area generated
    /// </summary>
    FitHeight = 1 << 0,

    /// <summary>
    ///     Adjusts the width of the inserted node to be the same as the area generated
    /// </summary>
    FitWidth = 1 << 1,

    /// <summary>
    ///     Adjusts the FlexNode's height to fit the nodes that are inserted into it
    /// </summary>
    FitContentHeight = 1 << 3,

    /// <summary>
    ///     Center inserted nodes into the middle of the flex area horizontally
    /// </summary>
    CenterVertically = 1 << 4,

    /// <summary>
    ///     Center inserted nodes into the middle of the flex area vertically
    /// </summary>
    CenterHorizontally = 1 << 5,
}
