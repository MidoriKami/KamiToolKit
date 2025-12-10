using System;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Classes.ContextMenu;

public class ContextMenuItem {
    public required ReadOnlySeString Name { get; init; }
    public bool IsEnabled { get; init; } = true;
    public required Action OnClick { get; init; }
    public int DisplayPriority { get; set; }
}
