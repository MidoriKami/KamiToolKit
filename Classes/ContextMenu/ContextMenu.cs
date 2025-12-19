using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Classes.ContextMenu;

public unsafe class ContextMenu : IDisposable {
    private readonly CustomEventInterface contextMenuEventInterface;

    private Dictionary<int, ContextMenuItem>? generatedEntries;
    private List<ContextMenuItem> Items { get; set; } = [];
    private IOrderedEnumerable<ContextMenuItem> OrderedItems => Items.OrderBy(item => item.DisplayPriority);

    public ContextMenu() {
        contextMenuEventInterface = new CustomEventInterface(ContextMenuEventHandler);
    }

    public void Dispose() {
        contextMenuEventInterface.Dispose();
    }

    private AtkValue* ContextMenuEventHandler(AtkModuleInterface.AtkEventInterface* thisPtr, AtkValue* returnValue, AtkValue* values, uint valueCount, ulong eventKind) {
        if (generatedEntries is null) return returnValue;

        var indexClicked = values[1].Int;

        if (generatedEntries?.TryGetValue(indexClicked, out var item) ?? false) {
            item.OnClick();
        }

        generatedEntries?.Clear();
        generatedEntries = null;

        return returnValue;
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

        var indexer = 0;
        generatedEntries = [];
        foreach (var item in OrderedItems) {
            generatedEntries.Add(indexer++, item);
            agentContextMenu->AddMenuItem(item.Name, contextMenuEventInterface, indexer, !item.IsEnabled);
        }

        agentContextMenu->OpenContextMenu();
    }

    public void Close() {
        var agentContextMenu = AgentContext.Instance();
        
        agentContextMenu->ClearMenu();
    }
}
