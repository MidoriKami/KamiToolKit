using System;
using KamiToolKit.Internal.Extensions;

namespace KamiToolKit.Nodes;

/// <summary>
/// Button list implementation for enums. Not intended for external use.
/// </summary>
public class EnumButtonListNode<T> : ButtonListNode<T> where T : Enum {

    /// <inheritdoc />
    protected override string GetLabelForOption(T option)
        => option.Description;
}
