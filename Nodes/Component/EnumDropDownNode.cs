using System;

namespace KamiToolKit.Nodes;

public class EnumDropDownNode<T> : DropDownNode<EnumButtonListNode<T>, T> where T : Enum {

    public EnumDropDownNode() {
        OptionListNode.OnOptionSelected += OptionSelectedHandler;
    }

    public Action<T>? OnOptionSelected { get; set; }

    private void OptionSelectedHandler(T option) {
        OnOptionSelected?.Invoke(option);
        UpdateLabel(option);
        Toggle(false);
    }

    protected override void UpdateLabel(T? option) {
        LabelNode.String = option?.Description;
    }
}
