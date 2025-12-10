using System;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Classes.ContextMenu;

public class ContextMenuItem {
    public required ReadOnlySeString Name { get; init; }
    public bool IsEnabled { get; init; } = true;
    public Action? OnClick { get; init; } = null;

    /// <summary>
    /// The identifier of this item
    /// </summary>
    internal int Id { get; set; }

    /// <summary>
    /// If set, places this item relative to the SortOrder of other items.
    ///
    /// Otherwise, items are sorted based on the order of creation.
    /// </summary>
    public uint? SortOrder { get; internal set; } = null;
}
