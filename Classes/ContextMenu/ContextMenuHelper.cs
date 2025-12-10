using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace KamiToolKit.Classes.ContextMenu;

/// <summary>
/// Helps create custom Context Menus
/// </summary>
/// <example>
/// <code>
/// ContextMenuHelper.Popup(
///     new ContextMenuItem {
///         Name = "Item 1",
///         OnClick = () => { /* Item 1 on click code */ }
///     },
///     new ContextMenuItem {
///         Name = "Item 2",
///         OnClick = () => { /* Item 2 on click code */ }
///     },
///     new ContextMenuItem {
///         Name = "Item 3",
///         OnClick = () => { /* Item 3 on click code */ }
///     }
/// );
/// </code>
/// </example>
public static class ContextMenuHelper {
    private static ContextMenuAtkEventInterface? _activeListener;

    public unsafe static void Popup(params IEnumerable<ContextMenuItem> items) {
        _activeListener?.Dispose();
        _activeListener = new ContextMenuAtkEventInterface(items);

        var ctx = AgentContext.Instance();
        ctx->ClearMenu();

        foreach (var item in _activeListener.Items) {
            var id = _activeListener.GetId(item);
            ctx->AddMenuItem(item.Name, _activeListener, (long)id, disabled: !item.IsEnabled, submenu: false);
        }

        ctx->OpenContextMenu(false, false);
    }
}
