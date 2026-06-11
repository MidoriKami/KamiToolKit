using System;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.ContextMenu;

/// <summary>
/// Data object used for <see cref="ContextMenu"/>
/// </summary>
public class ContextMenuItem {

    /// <summary>
    /// The label to show in the context menu UI.
    /// </summary>
    public required ReadOnlySeString Name { get; init; }

    /// <summary>
    /// Gets or sets if this entry should be clickable at this time.
    /// </summary>
    public bool IsEnabled { get; init; } = true;

    /// <summary>
    /// Gets or sets the callback that is triggered when this item is clicked on.
    /// </summary>
    public required Action OnClick { get; init; }

    /// <summary>
    /// Gets or sets the display priority, higher is closer to the top of the list.
    /// </summary>
    public int DisplayPriority { get; set; }
}
