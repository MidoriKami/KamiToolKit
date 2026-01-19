using System;

namespace KamiToolKit.Nodes;

public class EnumDropDownListNode<T> : DropDownListNode<T> where T : Enum {

    protected override string GetLabelForOption(T option)
        => option.Description;
}
