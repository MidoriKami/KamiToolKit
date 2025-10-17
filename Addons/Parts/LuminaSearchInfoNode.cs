using System;
using System.Text.RegularExpressions;
using Lumina.Excel;

namespace KamiToolKit.Addons.Parts;

public class LuminaSearchInfoNode<T> : BaseSearchInfoNode<T> where T : struct, IExcelRow<T> {
    public required Func<T, string> GetLabelFunc { get; init; }
    public Func<T, string>? GetSubLabelFunc { get; init; }
    public Func<T, uint>? GetIconIdFunc { get; init; }
    public Func<T, string>? GetTexturePathFunc { get; init; }

    protected override void SetOptionParams(T value) {
        LabelTextNode.String = GetLabelFunc(value);
        SubLabelTextNode.String = GetSubLabelFunc?.Invoke(value) ?? string.Empty;
        IdTextNode.String = value.RowId.ToString();

        if (GetIconIdFunc?.Invoke(value) is { } iconId) {
            IconNode.IconId = iconId;
        }

        if (GetTexturePathFunc?.Invoke(value) is { Length: > 0 } texturePath) {
            IconNode.LoadTexture(texturePath);
        } 
    }

    public override int Compare(BaseSearchInfoNode<T> other, string sortOption, bool reversed) {
        if (other is not LuminaSearchInfoNode<T> otherInfo) return 0;

        var result = sortOption switch {
            "Alphabetical" => string.CompareOrdinal(GetLabelFunc(Option), otherInfo.GetLabelFunc(otherInfo.Option)),
            "Id" => Option.RowId.CompareTo(otherInfo.Option.RowId), 
            _ => 0,
        };
        
        return reversed ? -result : result;
    }

    public override bool IsMatch(string searchString) {
        const RegexOptions regexOptions = RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;

        if (Regex.IsMatch(GetLabelFunc(Option), searchString, regexOptions)) return true;
        if (GetSubLabelFunc is not null && Regex.IsMatch(GetSubLabelFunc(Option), searchString, regexOptions)) return true;

        return false;
    }
}
