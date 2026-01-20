using System;

namespace KamiToolKit.Nodes;

public class EnumButtonListNode<T> : ButtonListNode<T> where T : Enum {

    protected override string GetLabelForOption(T option)
        => option.Description;
}
