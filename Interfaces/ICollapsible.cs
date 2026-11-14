using System;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing something collapsible.
/// </summary>
public interface ICollapsible {

    /// <summary>
    /// Gets or sets collapsed state.
    /// </summary>
    bool IsCollapsed { get; }

    /// <summary>
    /// Gets or sets an action that is invoked when <see cref="IsCollapsed"/> is toggled.
    /// </summary>
    Action<bool>? OnCollapseToggled { get; set; }

    /// <summary>
    /// Gets or sets an action that is invoked when <see cref="IsCollapsed"/> is set to true.
    /// </summary>
    Action? OnCollapsed { get; set; }

    /// <summary>
    /// Gets or sets an action that is invoked when <see cref="IsCollapsed"/> is set to false.
    /// </summary>
    Action? OnUncollapsed { get; set; }

    /// <summary>
    /// Collapses the element.
    /// </summary>
    /// <param name="playSoundEffect">Whether to play collapse sfx.</param>
    void Collapse(bool playSoundEffect = true);

    /// <summary>
    /// Uncollapses the element.
    /// </summary>
    /// <param name="playSoundEffect">Whether to play collapse sfx.</param>
    void Uncollapse(bool playSoundEffect = true);

    /// <summary>
    /// Toggles the element.
    /// </summary>
    /// <param name="playSoundEffect">Whether to play collapse sfx.</param>
    void Toggle(bool playSoundEffect = true);
}
