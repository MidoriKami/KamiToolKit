using System.Collections.Generic;
using System.Linq;

namespace KamiToolKit.Classes.ContextMenu;

public class ContextMenu {
    public List<ContextMenuItem> Items { get; set; } = new();

    public ContextMenu(params IEnumerable<ContextMenuItem> items) {
        Items = items.ToList();
    }

    public void Open() {
        ContextMenuHelper.Open(Items);
    }
}
