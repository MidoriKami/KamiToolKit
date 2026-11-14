using System;
using KamiToolKit.Enums;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a text input node.
/// </summary>
public interface ITextInputNode : IFocusable, IPlaceholderText {

    /// <summary>
    /// Gets or sets the maximum number of characters allowed.
    /// </summary>
    int MaxCharacters { get; set; }

    /// <summary>
    /// Gets or sets whether the <see cref="MaxCharacters"/> limit should be shown.
    /// </summary>
    bool ShowLimitText { get; set; }

    /// <summary>
    /// Gets or sets if auto-translate/completion should be shown when pressing Tab.
    /// </summary>
    bool EnableCompletion { get; set; }

    /// <summary>
    /// Gets or sets whether sound effects should be enabled when selecting or unselecting the node.
    /// </summary>
    bool EnableFocusSounds { get; set; }

    /// <summary>
    /// Gets or sets the text input flags.
    /// </summary>
    TextInputFlags TextInputFlags { get; set; }

    /// <summary>
    /// Gets or sets the current text input.
    /// </summary>
    ReadOnlySeString String { get; set; }

    /// <summary>
    /// Gets or sets whether the text input's border should be reddened to indicate an error.
    /// </summary>
    bool IsError { get; set; }

    /// <summary>
    /// Gets or sets whether all text should be selected when this element becomes focused.
    /// </summary>
    bool AutoSelectAll { get; set; }

    /// <summary>
    /// Gets or sets the action to be invoked when typing into the input, is triggered for each letter/symbol.
    /// Includes a reference to the modified string.
    /// </summary>
    Action<ReadOnlySeString>? OnInputReceived { get; set; }

    /// <summary>
    /// Gets or sets the action to be invoked when input is completed via pressing return.
    /// Includes a reference to the modified string.
    /// </summary>
    Action<ReadOnlySeString>? OnInputComplete { get; set; }

    /// <summary>
    /// Gets or sets the action to be called when input is canceled via escape.
    /// </summary>
    Action? OnEscapeEntered { get; set; }

    /// <summary>
    /// Gets or sets the action to be called when tab is input.
    /// </summary>
    Action? OnTabEntered { get; set; }
}
