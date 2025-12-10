using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;
using static FFXIVClientStructs.FFXIV.Component.GUI.AtkModuleInterface;

namespace KamiToolKit.Classes.ContextMenu;

internal unsafe class ContextMenuEventInterface : IDisposable {
    private readonly CustomAtkEventInterface listener;
    private readonly Dictionary<uint, ContextMenuItem> idToItem = new();

    public IReadOnlyList<ContextMenuItem> Items => idToItem.Select(kv => kv.Value).ToList();

    public ContextMenuEventInterface(IEnumerable<ContextMenuItem> items) {
        var nextSortOrder = 0U;
        foreach (var (index, item) in items.Index()) {
            if (item.SortOrder == null) {
                item.SortOrder = nextSortOrder;
                nextSortOrder += 1;
            }

            item.Id = index;
            idToItem[(uint)index] = item;
        }

        listener = new CustomAtkEventInterface(ReceiveEvent);
    }

    private AtkValue* ReceiveEvent(AtkEventInterface* thisPtr, AtkValue* returnValue, AtkValue* values, uint valueCount, ulong eventKind) {
        if (idToItem.TryGetValue((uint)eventKind, out var item)) {
            item.OnClick?.Invoke();
        }
        return returnValue;
    }

    public static implicit operator AtkEventInterface*(ContextMenuEventInterface e) => e.listener;

    public void Dispose() => listener.Dispose();
}
