using System.Linq;
using KamiToolKit.Internal.Classes;
using Lumina.Excel;

namespace KamiToolKit.Nodes;

/// <summary>
/// Specialization of <see cref="ButtonListNode{T}"/>, for use in <see cref="LuminaDropDownNode{T}"/>.
/// Not intended for external use.
/// </summary>
public class LuminaButtonListNode<T> : ButtonListNode<T> where T : struct, IExcelRow<T> {

    /// <summary>
    /// Delegate used to get the displayed label for a given lumina row.
    /// </summary>
    public delegate string GetLabel(T excelRow);

    /// <summary>
    /// Delegate used to check if a given row should be shown.
    /// </summary>
    public delegate bool ShouldShow(T excelRow);

    /// <summary>
    /// Gets or sets the function that will be used to resolve labels from lumina row objects.
    /// </summary>
    public GetLabel? LabelFunction {
        get;
        set {
            field = value;
            ResolveOptions();
        }
    }

    /// <summary>
    /// Gets or sets the function that will be used to filter out entries from the datasheets.
    /// </summary>
    public ShouldShow? FilterFunction {
        get;
        set {
            field = value;
            ResolveOptions();
        }
    }

    private void ResolveOptions() {
        if (LabelFunction is null) return;
        if (FilterFunction is null) return;

        Options = Services.DataManager.GetExcelSheet<T>()
            .Where(row => FilterFunction(row))
            .ToList();
    }

    protected override string GetLabelForOption(T option)
        => LabelFunction?.Invoke(option) ?? "ERROR: Label Function Not Found";
}
