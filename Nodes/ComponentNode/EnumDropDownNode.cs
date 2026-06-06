using System;
using KamiToolKit.Internal.Extensions;

namespace KamiToolKit.Nodes;

/// <summary>
/// Specialization of a DropDownNode for use with enums.
/// </summary>
public class EnumDropDownNode<T> : DropDownNode<EnumButtonListNode<T>, T> where T : Enum {

    /// <summary>
    /// Action that is invoked when an option is selected. Contains a reference to the enum value selected.
    /// </summary>
    public Action<T>? OnOptionSelected { get; set; }

    public EnumDropDownNode() {
        OptionListNode.OnOptionSelected += OptionSelectedHandler;
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
