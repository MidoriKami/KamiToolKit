using System;
using KamiToolKit.BaseTypes;

namespace KamiToolKit.Enums;

/// <summary>
/// Enum representing which edit mode is enabled for <see cref="NodeBase{T}"/>
/// </summary>
[Flags]
public enum NodeEditMode {
    /// <summary>
    /// Resize
    /// </summary>
    Resize = 1 << 1,

    /// <summary>
    /// Move
    /// </summary>
    Move = 1 << 2,
}
