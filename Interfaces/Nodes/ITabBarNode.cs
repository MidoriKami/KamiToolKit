using System;
using System.Collections.Generic;
using KamiToolKit.Classes;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a tab bar node.
/// </summary>
public interface ITabBarNode : IControllerNavigable {

    /// <summary>
    /// Sets the initial nodes that this list will use.
    /// </summary>
    ICollection<TabBarEntry> InitialEntries { init; }

    /// <summary>
    /// Selects the tab matching the given label.
    /// </summary>
    void SelectTab(ReadOnlySeString tabName);

    /// <summary>
    /// Disables a tab matching the given label, the tab won't be selectable or interactable.
    /// </summary>
    void DisableTab(ReadOnlySeString tabName);

    /// <summary>
    /// Enables a tab matching the given label.
    /// </summary>
    void EnableTab(ReadOnlySeString tabName);

    /// <summary>
    /// Toggles a tab matching the given label's enabled/disabled state.
    /// </summary>
    void ToggleTab(ReadOnlySeString tabName);

    /// <summary>
    /// Removes a tab matching the given label.
    /// </summary>
    void RemoveTab(ReadOnlySeString tabName);

    /// <summary>
    /// Clear all tab nodes.
    /// </summary>
    void Clear();

    /// <summary>
    /// Add a new tab via <see cref="TabBarEntry"/>.
    /// </summary>
    void AddTab(TabBarEntry entry);

    /// <summary>
    /// Add a new tab with the provided callback and option tooltip.
    /// </summary>
    void AddTab(ReadOnlySeString name, Action? callback = null, ReadOnlySeString? tooltip = null, bool isEnabled = true);
}
