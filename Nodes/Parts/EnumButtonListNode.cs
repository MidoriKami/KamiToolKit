using System;
using KamiToolKit.Extensions.Internal;

namespace KamiToolKit.Nodes;

/// <summary>
/// Button list implementation for enums. Not intended for external use.
/// </summary>
public class EnumButtonListNode<T> : ButtonListNode<T> where T : Enum {

    protected override string GetLabelForOption(T option)
        => option.Description;
}
