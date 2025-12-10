using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;
using static FFXIVClientStructs.FFXIV.Component.GUI.AtkModuleInterface;

namespace KamiToolKit.Classes.ContextMenu;

internal sealed unsafe class ContextMenuAtkEventInterface : IDisposable {
    private readonly CustomAtkEventInterface listener;
    private readonly Dictionary<ulong, ContextMenuItem> idToItem;
    private readonly Dictionary<ContextMenuItem, ulong> itemToId;
    private readonly List<ContextMenuItem> items;

    public IReadOnlyList<ContextMenuItem> Items => items;

    public ContextMenuAtkEventInterface(IEnumerable<ContextMenuItem> items) {
        this.items = items.ToList();

        idToItem = new Dictionary<ulong, ContextMenuItem>();

        var currentId = 1UL;
        foreach(var item in items) {
            idToItem[currentId] = item;
            currentId += 1;
        }

        itemToId = idToItem.ToDictionary(i => i.Value, i => i.Key);

        listener = new CustomAtkEventInterface(
            (AtkEventInterface* thisPtr, AtkValue* returnValue, AtkValue* values, uint valueCount, ulong eventKind) => {
                if (idToItem.TryGetValue(eventKind, out var item)) {
                    item.OnClick?.Invoke();
                }
                return returnValue;
            }
        );
    }

    public ulong GetId(ContextMenuItem item) => itemToId[item];

    public static implicit operator AtkEventInterface*(ContextMenuAtkEventInterface e) => e.listener;

    public void Dispose() => listener.Dispose();
}
