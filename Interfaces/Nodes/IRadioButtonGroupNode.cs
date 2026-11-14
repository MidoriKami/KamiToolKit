using System;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a group of radio buttons.
/// </summary>
public interface IRadioButtonGroupNode {

    /// <summary>
    /// Gets or sets the currently selected option by label.
    /// </summary>
    ReadOnlySeString? SelectedOption { get; set; }

    /// <summary>
    /// Gets or sets the vertical padding between radio buttons.
    /// </summary>
    float VerticalPadding { get; set; }

    /// <summary>
    /// Adds a radio button with the given label and callback.
    /// </summary>
    /// <param name="label">The label for the radio button.</param>
    /// <param name="callback">The action to invoke when the button is clicked.</param>
    void AddButton(ReadOnlySeString label, Action callback);

    /// <summary>
    /// Removes the radio button with the given label.
    /// </summary>
    /// <param name="label">The label of the button to remove.</param>
    void RemoveButton(ReadOnlySeString label);

    /// <summary>
    /// Removes all radio buttons from this group.
    /// </summary>
    void Clear();
}
