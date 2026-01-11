using System.Linq;
using KamiToolKit.Classes;
using Lumina.Excel;

namespace KamiToolKit.Nodes;

public class LuminaDropDownListNode<T> : DropDownListNode<T> where T : struct, IExcelRow<T> {

    public delegate string GetLabel(T excelRow);

    public delegate bool ShouldShow(T excelRow);
    
    public GetLabel? LabelFunction {
        get;
        set {
            field = value;
            ResolveOptions();
        }
    }

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

        Options = DalamudInterface.Instance.DataManager.GetExcelSheet<T>()
            .Where(row => FilterFunction(row))
            .ToList();
    }

    protected override string GetLabelForOption(T option)
        => LabelFunction?.Invoke(option) ?? "ERROR: Label Function Not Found";
}
