using System.Text.RegularExpressions;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Extensions;

/// <summary>
/// Extension methods for <see cref="ReadOnlySeString"/>
/// </summary>
public static class ReadOnlySeStringExtensions {
    extension(ReadOnlySeString seString) {

        /// <summary>
        /// Gets the ReadOnlySeString as a Regex object, returning a Regex(string.Empty)
        /// if the contents of the SeString are not a valid regex expression.
        /// </summary>
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
