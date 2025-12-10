using System;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Classes.ContextMenu;

public class ContextMenuItem {
    public required ReadOnlySeString Name { get; init; }
    public bool IsEnabled { get; init; } = true;
    public required Action OnClick { get; init; }

    private static int incrementer;

    /// <summary>
    /// If set, places this item relative to the SortOrder of other items.
    /// Otherwise, items are sorted based on the order of creation.
    /// </summary>
    public int SortOrder {
        get;
        init {
            if (value == 0) {
                field = incrementer++;
            }
            else {
                field = value;
            }
        }
    }
}
