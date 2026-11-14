using System;
using System.Collections.Generic;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a collection of options and a callback on selection.
/// </summary>
public interface IOptionSelectable<T> {

    /// <summary>
    /// Gets the options list.
    /// </summary>
    List<T> Options { get; }

    /// <summary>
    /// Gets or sets an action to be invoked when an option is selected.
    /// </summary>
    Action<T>? OnOptionSelected { get; set; }

    /// <summary>
    /// Gets or sets the seelcted option.
    /// </summary>
    T? SelectedOption { get; set; }
}
