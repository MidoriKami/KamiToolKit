using System;
using Lumina.Excel;

namespace KamiToolKit.Nodes;

/// <summary>
/// Specialization of a <see cref="DropDownNode{T,TU}"/> for use with Lumina rows.
/// </summary>
public class LuminaDropDownNode<T> : DropDownNode<LuminaButtonListNode<T>, T> where T : struct, IExcelRow<T> {

    /// <summary>
    /// Gets or sets the action that is invoked when an option is selected.
    /// </summary>
    public Action<T>? OnOptionSelected { get; set; }

    /// <summary>
    /// Gets or sets the function used to display a given excel row.
    /// </summary>
    public LuminaButtonListNode<T>.GetLabel? LabelFunction {
        get => OptionListNode.LabelFunction;
        set => OptionListNode.LabelFunction = value;
    }

    /// <summary>
    /// Gets or sets the function used to filter out unwanted excel rows.
    /// </summary>
    public LuminaButtonListNode<T>.ShouldShow? FilterFunction {
        get => OptionListNode.FilterFunction;
        set => OptionListNode.FilterFunction = value;
    }

    /// <summary>
    /// Constructs a new <see cref="LuminaDropDownNode{T}"/>.
    /// </summary>
    public LuminaDropDownNode()
        => OptionListNode.OnOptionSelected += OptionSelectedHandler;

    private void OptionSelectedHandler(T option) {
        OnOptionSelected?.Invoke(option);
        UpdateLabel(option);
        Toggle(false);
    }

    /// <inheritdoc />
    protected override void UpdateLabel(T option)
        => LabelNode.String = LabelFunction?.Invoke(option) ?? "ERROR: Label Function Not Set";
}
