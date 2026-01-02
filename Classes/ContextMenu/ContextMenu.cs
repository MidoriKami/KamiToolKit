using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Classes.ContextMenu;

public unsafe class ContextMenu :  IDisposable {
    private readonly CustomEventInterface contextMenuEventInterface;

    private Dictionary<long, ContextMenuItem>? mainMenuEntries;
    private Dictionary<long, ContextMenuSubItem>? mainMenuSubMenus;
    private Dictionary<long, ContextMenuItem>? subMenuEntries;

    // Prevent the return entry from colliding with submenu items
    private const int SubMenuIndexOffset = 1000;

    private List<ContextMenuItem> Items { get; set; } = [];
    private IOrderedEnumerable<ContextMenuItem> OrderedItems => Items.OrderBy(item => item.DisplayPriority);

    public ContextMenu() {
        contextMenuEventInterface = new CustomEventInterface(ContextMenuEventHandler);
    }

    public void Dispose() {
        contextMenuEventInterface.Dispose();
    }

    private AtkValue* ContextMenuEventHandler(AtkModuleInterface.AtkEventInterface* thisPtr, AtkValue* returnValue, AtkValue* values, uint valueCount, ulong eventKind) {
        var handlerParam = (long)eventKind;

        if (handlerParam >= SubMenuIndexOffset) {
            if (subMenuEntries?.TryGetValue(handlerParam, out var subItem) ?? false) {
                subItem.OnClick();
                ClearAll();
            }
            return returnValue;
        }

        if (mainMenuSubMenus?.TryGetValue(handlerParam, out var subMenuItem) ?? false) {
            OpenSubMenu(subMenuItem);
            return returnValue;
        }

        if (mainMenuEntries?.TryGetValue(handlerParam, out var item) ?? false) {
            item.OnClick();
            ClearAll();
            return returnValue;
        }

        subMenuEntries?.Clear();
        subMenuEntries = null;

        return returnValue;
    }

    private void ClearAll() {
        mainMenuEntries?.Clear();
        mainMenuEntries = null;
        mainMenuSubMenus?.Clear();
        mainMenuSubMenus = null;
        subMenuEntries?.Clear();
        subMenuEntries = null;
    }

    public void AddItem(ReadOnlySeString name, Action callback) {
        AddItem(new ContextMenuItem {
            Name = name,
            OnClick = callback,
        });
    }

    public void RemoveItem(ReadOnlySeString name) {
        var targetItem = Items.FirstOrDefault(item => item.Name == name);
        if (targetItem is null) return;

        Items.Remove(targetItem);
    }

    public void AddItem(ContextMenuItem item, params ContextMenuItem[] items) {
        foreach (var entry in items.Prepend(item)) {
            Items.Add(entry);
        }
    }

    public void RemoveItem(ContextMenuItem item, params ContextMenuItem[] items) {
        foreach (var entry in items.Prepend(item)) {
            Items.Remove(entry);
        }
    }

    public void Clear() => Items.Clear();

    public void Open() {
        var agentContextMenu = AgentContext.Instance();

        agentContextMenu->ClearMenu();

        mainMenuEntries = [];
        mainMenuSubMenus = [];
        subMenuEntries = null;

        foreach (var (index, item) in OrderedItems.Index()) {
            if (item is ContextMenuSubItem subItem) {
                mainMenuSubMenus.Add(index, subItem);
                agentContextMenu->AddMenuItem(item.Name, contextMenuEventInterface, index, !item.IsEnabled, submenu: true);
            } else {
                mainMenuEntries.Add(index, item);
                agentContextMenu->AddMenuItem(item.Name, contextMenuEventInterface, index, !item.IsEnabled, submenu: false);
            }
        }

        agentContextMenu->OpenContextMenu();
    }

    private void OpenSubMenu(ContextMenuSubItem subItem) {
        var agentContextMenu = AgentContext.Instance();

        // Set the state again to prevent the menu closing when going back and forth between the submenus
        agentContextMenu->SubContextMenu.SelectedContextItemIndex = 0;
        agentContextMenu->SubContextMenu.CurrentEventIndex = 8;

        agentContextMenu->OpenSubMenu();

        var indexer = 0;
        subMenuEntries = [];

        foreach (var item in subItem.SubItems.OrderBy(i => i.DisplayPriority)) {
            if (item is ContextMenuSubItem) continue;

            var paramIndex = SubMenuIndexOffset + indexer;
            subMenuEntries.Add(paramIndex, item);
            agentContextMenu->AddMenuItem(item.Name, contextMenuEventInterface, paramIndex, !item.IsEnabled, submenu: false);
            indexer++;
        }
    }

    public void Close() {
        var agentContextMenu = AgentContext.Instance();

        agentContextMenu->ClearMenu();
        ClearAll();
    }
}
