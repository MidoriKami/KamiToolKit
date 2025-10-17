using System.Text.RegularExpressions;

namespace KamiToolKit.Addons.Interfaces;

public interface IInfoNodeData {
    string GetLabel();
    string? GetSubLabel() => null;
    uint? GetId() => null;
    uint? GetIconId() => null;
    string? GetTexturePath() => null;

    bool ContainsSearchTerm(string searchTerm) {
        const RegexOptions regexOptions = RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;

        if (Regex.IsMatch(GetLabel(), searchTerm, regexOptions)) return true;
        if (GetSubLabel() is { Length: > 0 } subLabel && Regex.IsMatch(subLabel, searchTerm, regexOptions)) return true;

        return false;
    }

    int Compare(IInfoNodeData other, string sortingMode);
}
