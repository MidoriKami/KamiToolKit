using System;
using System.Collections.Generic;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing something that allows multi select.
/// </summary>
/// <remarks>
/// If looking for single select use <see cref="IOptionSelectable{T}"/>.
/// </remarks>
public interface IOptionMultiSelectable<T> {

    /// <summary>
    /// Gets the options list.
    /// </summary>
    List<T> Options { get; }

    /// <summary>
    /// Gets or sets an action to be invoked when an option is selected.
    /// </summary>
    Action<T>? OnOptionSelected { get; set; }

    /// <summary>
    /// Gets the list of currently selected options.
    /// </summary>
    List<T> SelectedOptions { get; }

    /// <summary>
    /// Clears the currently selected items.
    /// </summary>
    void ClearSelection();

    /// <summary>
    /// Gets or sets whether multiselect will be allowed.
    /// </summary>
    bool AllowMultipleSelection { get; set; }
}
