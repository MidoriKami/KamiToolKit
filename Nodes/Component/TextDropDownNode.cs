using System;

namespace KamiToolKit.Nodes;

public class TextDropDownNode : DropDownNode<TextButtonListNode, string> {

    public TextDropDownNode() {
        OptionListNode.OnOptionSelected += OptionSelectedHandler;
    }

    public Action<string>? OnOptionSelected { get; set; }

    private void OptionSelectedHandler(string option) {
        OnOptionSelected?.Invoke(option);
        UpdateLabel(option);
        Toggle(false);
    }

    protected override void UpdateLabel(string? option) {
        LabelNode.String = option ?? "ERROR: Invalid Default Option";
    }
}
