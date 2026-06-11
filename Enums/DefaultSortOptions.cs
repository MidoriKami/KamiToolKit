using System.ComponentModel;

namespace KamiToolKit.Enums;

/// <summary>
/// Enumeration of default sorting options to be used with searchable lists. todo: evaluate if I wanna keep this.
/// </summary>
public enum DefaultSortOptions {

    /// <summary>
    /// Default value indicating that no sorting mode has been selected.
    /// </summary>
    [Description("ERROR: Unset")]
    Unset,

    /// <summary>
    /// Sort alphabetically.
    /// </summary>
    [Description("Alphabetical")]
    Alphabetical,

    /// <summary>
    /// Sort based on a provided id value.
    /// </summary>
    [Description("Id")]
    Id,
}
