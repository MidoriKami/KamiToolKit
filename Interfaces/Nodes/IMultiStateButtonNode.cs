using System;
using System.Collections.Generic;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a multi-state button node that cycles through states.
/// </summary>
public interface IMultiStateButtonNode<T> : ITextButtonNode where T : notnull {

    /// <summary>
    /// Gets or sets the list of available states.
    /// </summary>
    List<T> States { get; set; }

    /// <summary>
    /// Gets or sets the currently selected state.
    /// </summary>
    T SelectedState { get; set; }

    /// <summary>
    /// Gets or sets the action invoked when the state changes.
    /// </summary>
    Action<T>? OnStateChanged { get; set; }
}
