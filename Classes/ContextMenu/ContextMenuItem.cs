using System;
using System.Collections.Generic;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Classes.ContextMenu;

public class ContextMenuItem {
    public required ReadOnlySeString Name { get; set; }
    public bool IsEnabled { get; set; } = true;
    public Action? OnClick { get; set; } = null;
}
