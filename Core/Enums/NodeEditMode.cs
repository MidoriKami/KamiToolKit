using System;
using KamiToolKit.Core.Primitives.NodeBase;

namespace KamiToolKit.Core.Enums;

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
