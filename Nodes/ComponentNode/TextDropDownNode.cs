using System;

namespace KamiToolKit.Nodes;

/// <summary>
/// Specialization of a <see cref="DropDownNode{T,TU}"/> for use with standard strings.
/// </summary>
public class TextDropDownNode : DropDownNode<TextButtonListNode, string> {

    /// <summary>
    /// Gets or sets the action to be called when an option is selected. Contains a reference to the selected string.
    /// </summary>
    public Action<string>? OnOptionSelected { get; set; }

    /// <summary>
    /// Constructs a new <see cref="TextDropDownNode"/>
    /// </summary>
    public TextDropDownNode()
        => OptionListNode.OnOptionSelected += OptionSelectedHandler;

    private void OptionSelectedHandler(string option) {
        OnOptionSelected?.Invoke(option);
        UpdateLabel(option);
        Toggle(false);
    }

    /// <inheritdoc />
    protected override void UpdateLabel(string? option)
        => LabelNode.String = option ?? "ERROR: Invalid Default Option";
}
