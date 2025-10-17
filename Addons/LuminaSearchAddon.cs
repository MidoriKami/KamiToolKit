using System;
using KamiToolKit.Addons.Parts;
using Lumina.Excel;

namespace KamiToolKit.Addons;

public class LuminaSearchAddon<T> : BaseSearchAddon<T> where T : struct, IExcelRow<T> {

    public required Func<T, string> GetLabelFunc { get; init; }
    public Func<T, string>? GetSubLabelFunc { get; init; }
    public Func<T, uint>? GetIconIdFunc { get; init; }
    public Func<T, string>? GetTexturePathFunc { get; init; }

    protected override BaseSearchInfoNode<T> BuildOptionNode(T option) => new LuminaSearchInfoNode<T> {
        IsVisible = true,
        GetLabelFunc = GetLabelFunc,
        GetSubLabelFunc = GetSubLabelFunc,
        GetIconIdFunc = GetIconIdFunc,
        GetTexturePathFunc = GetTexturePathFunc,
        Option = option,
    };
}
