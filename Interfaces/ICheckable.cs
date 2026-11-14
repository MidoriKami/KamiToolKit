using System;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Represents something that can be checked.
/// </summary>
public interface ICheckable {

    /// <summary>
    /// Gets or sets if this node is checked.
    /// </summary>
    /// <remarks>
    /// Writing to this value will trigger <see cref="ICheckable.OnChecked"/>.
    /// </remarks>
    bool IsChecked { get; set; }

    /// <summary>
    /// Action called when this node's text or box are clicked.
    /// </summary>
    Action<bool>? OnChecked { get; set; }
}
