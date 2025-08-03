using System;
using System.Linq;
using KamiToolKit.Classes;
using Lumina.Excel;

namespace KamiToolKit.Nodes;

public class LuminaListNode<T> : ListNode<T> where T : struct, IExcelRow<T> {

    public Func<T, string>? LabelFunction {
        get;
        set {
            field = value;
            ResolveOptions();
        }
    }

    public Func<T, bool>? FilterFunction {
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
            .Where(FilterFunction)
            .ToList();
    }

    protected override string GetLabelForOption(T option)
        => LabelFunction?.Invoke(option) ?? "ERROR: Label Function Not Found";
}
