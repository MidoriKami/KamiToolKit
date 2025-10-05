using System.Text.RegularExpressions;

namespace KamiToolKit.Addons.Parts;

public class OptionInfo<T> {
    public uint? IconId { get; init; }
    public string? TexturePath { get; init; }
    
    public required string Label { get; init; }
    public string? SubLabel { get; init; }
    public uint? Id { get; init; }

    public required T Option { get; set; }

    public bool ContainsSearchTerm(string searchTerm) {
        const RegexOptions regexOptions = RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;

        if (Regex.IsMatch(Label, searchTerm, regexOptions)) return true;
        if (SubLabel is not null && Regex.IsMatch(SubLabel, searchTerm, regexOptions)) return true;

        return false;
    }
}
