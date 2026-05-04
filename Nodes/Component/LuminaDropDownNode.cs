using System;
using Lumina.Excel;

namespace KamiToolKit.Nodes;

public class LuminaDropDownNode<T> : DropDownNode<LuminaButtonListNode<T>, T> where T : struct, IExcelRow<T> {

    public LuminaDropDownNode() {
        OptionListNode.OnOptionSelected += OptionSelectedHandler;
    }

    public Action<T>? OnOptionSelected { get; set; }

    public LuminaButtonListNode<T>.GetLabel? LabelFunction {
        get => OptionListNode.LabelFunction;
        set => OptionListNode.LabelFunction = value;
    }

    public LuminaButtonListNode<T>.ShouldShow? FilterFunction {
        get => OptionListNode.FilterFunction;
        set => OptionListNode.FilterFunction = value;
    }

    private void OptionSelectedHandler(T option) {
        OnOptionSelected?.Invoke(option);
        UpdateLabel(option);
        Toggle(false);
    }

    protected override void UpdateLabel(T option) {
        LabelNode.String = LabelFunction?.Invoke(option) ?? "ERROR: Label Function Not Set";
    }
}
