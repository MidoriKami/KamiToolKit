using System;
using System.Collections.Generic;

namespace KamiToolKit.Nodes;

public class EnumDropDownNode<T> : DropDownNode<EnumButtonListNode<T>, T> where T : Enum{

    public EnumDropDownNode() {
        OptionListNode.OnOptionSelected += OptionSelectedHandler;
    }
    
    public Action<T>? OnOptionSelected { get; set; }

    public required List<T>? Options {
        get => OptionListNode.Options;
        set {
            OptionListNode.Options = value;
            OptionListNode.SelectDefaultOption();
            UpdateLabel(OptionListNode.SelectedOption);
        }
    }

    private void OptionSelectedHandler(T option) {
        OnOptionSelected?.Invoke(option);
        UpdateLabel(option);
        Toggle(false);
    }
    
    protected override void UpdateLabel(T? option) {
        LabelNode.String = option?.Description;
    }
}
