using System.Text.RegularExpressions;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Extensions;

public static class ReadOnlySeStringExtensions {
    extension(ReadOnlySeString seString) {
        public Regex AsRegex() {
            Regex searchRegex;

            try {
                searchRegex = new Regex(seString.ToString(), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            }
            catch (RegexParseException) {
                searchRegex = new Regex(string.Empty);
            }

            return searchRegex;
        }
    }
}
