using System.Text.RegularExpressions;

namespace KamiToolKit.Premade;

public interface IInfoNodeData {
    string GetLabel();
    string? GetSubLabel();
    uint? GetId();
    uint? GetIconId();
    string? GetTexturePath();

    bool ContainsSearchTerm(string searchTerm) {
        const RegexOptions regexOptions = RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;

        if (Regex.IsMatch(GetLabel(), searchTerm, regexOptions)) return true;
        if (GetSubLabel() is { Length: > 0 } subLabel && Regex.IsMatch(subLabel, searchTerm, regexOptions)) return true;

        return false;
    }

    int Compare(IInfoNodeData other, string sortingMode);
}
