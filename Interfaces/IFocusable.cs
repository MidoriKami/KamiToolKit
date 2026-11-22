using System;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Represents a focusable element.
/// </summary>
public interface IFocusable {

    /// <summary>
    /// Gets or sets whether this node is focused.
    /// </summary>
    bool IsFocused { get; set; }

    /// <summary>
    /// Clears the nodes focus.
    /// </summary>
    void ClearFocus() {
        IsFocused = false;
        OnUnfocused?.Invoke();
    }

    /// <summary>
    /// Sets this node to be focused.
    /// </summary>
    void SetFocus() {
        IsFocused = true;
        OnFocused?.Invoke();
    }

    /// <summary>
    /// Gets or sets the action to be called when the node is focused.
    /// </summary>
    Action? OnFocused { get; set; }

    /// <summary>
    /// Gets or sets the action to be called when the node loses focus.
    /// </summary>
    Action? OnUnfocused { get; set; }
}
