using System.ComponentModel;

namespace KamiToolKit.Enums;

/// <summary>
/// Enumeration of available default Addon Search Options
/// </summary>
public enum AddonSearchSortOptions {

    /// <summary>
    /// Sorts list based on addon visibility.
    /// </summary>
    /// <remarks>
    /// Visible addons require both IsVisible on the addon and to be actually visible.
    /// </remarks>
    [Description("Visibility")]
    Visibility,

    /// <summary>
    /// Sorts list alphabetically.
    /// </summary>
    [Description("Alphabetical")]
    Alphabetical,
}
