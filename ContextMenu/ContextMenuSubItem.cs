using System;
using System.Collections.Generic;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.ContextMenu;

/// <summary>
/// Represents a context menu Sub-Menu.
/// One level of submenu only. Nested submenus not supported.
/// </summary>
public class ContextMenuSubItem : ContextMenuItem {

    /// <summary>
    /// A list of all the submenu items this master entry will show.
    /// </summary>
    public List<ContextMenuItem> SubItems { get; set; } = [];

    /// <summary>
    /// Add a new item with specified name and callback.
    /// </summary>
    public void AddItem(ReadOnlySeString name, Action callback) => SubItems.Add(new ContextMenuItem {
        Name = name,
        OnClick = callback,
    });

    /// <summary>
    /// Add an already constructed ContextMenuItem.
    /// </summary>
    public void AddItem(ContextMenuItem item)
        => SubItems.Add(item);
}
