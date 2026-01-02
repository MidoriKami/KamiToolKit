using System;
using System. Collections.Generic;
using Lumina.Text. ReadOnly;

namespace KamiToolKit.Classes.ContextMenu;

/// <summary>
/// One level of submenu only. Nested submenus not supported.
/// </summary>
public class ContextMenuSubItem : ContextMenuItem
{
    public List<ContextMenuItem> SubItems { get; set; } = [];

    public void AddItem(ReadOnlySeString name, Action callback)
    {
        SubItems.Add(new ContextMenuItem {
            Name = name,
            OnClick = callback,
        });
    }

    public void AddItem(ContextMenuItem item) => SubItems.Add(item);
}
