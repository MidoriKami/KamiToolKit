using System;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Represents a focusable element.
/// </summary>
public interface IFocusable {

    /// <summary>
    /// Gets if this node is focused.
    /// </summary>
    bool IsFocused { get; }

    /// <summary>
    /// Clears the nodes focus.
    /// </summary>
    void ClearFocus();

    /// <summary>
    /// Sets this node to be focused.
    /// </summary>
    void SetFocus();

    /// <summary>
    /// Gets or sets the action to be called when the node loses focus.
    /// </summary>
    /// <remarks>
    /// May be redundant with <see cref="OnUnfocused"/>
    /// </remarks>
    Action? OnFocusLost { get; set; }

    /// <summary>
    /// Gets or sets the action to be called when the node is focused.
    /// </summary>
    Action? OnFocused { get; set; }

    /// <summary>
    /// Gets or sets the action to be called when the node loses focus.
    /// </summary>
    /// <remarks>
    /// May be redundant with <see cref="OnFocusLost"/>
    /// </remarks>
    Action? OnUnfocused { get; set; }
}
