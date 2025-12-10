using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace KamiToolKit.Classes.ContextMenu;

/// <summary>
/// Helps create custom Context Menus
/// </summary>
public static class ContextMenuHelper {
    private static ContextMenuEventInterface? activeListener;

    public unsafe static void Open(params IEnumerable<ContextMenuItem> items) {
        activeListener?.Dispose();
        activeListener = new ContextMenuEventInterface(items);

        var ctx = AgentContext.Instance();
        ctx->ClearMenu();

        foreach (var item in activeListener.Items.OrderBy(item => item.SortOrder)) {
            ctx->AddMenuItem(item.Name, activeListener, (long)item.Id, disabled: !item.IsEnabled, submenu: false);
        }

        ctx->OpenContextMenu(false, false);
    }
}
