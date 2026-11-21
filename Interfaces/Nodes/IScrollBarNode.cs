using System;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a scroll bar node.
/// </summary>
public interface IScrollBarNode {

    /// <summary>
    /// Gets or sets the current scroll position.
    /// </summary>
    float ScrollPosition { get; set; }

    /// <summary>
    /// Gets the maximum valid scroll position for the current content.
    /// </summary>
    int ScrollMaxPosition { get; }

    /// <summary>
    /// Gets or sets the scroll speed, default is 24px per scroll.
    /// </summary>
    int ScrollSpeed { get; set; }

    /// <summary>
    /// Gets or sets whether this scrollbar scrolls horizontally.
    /// </summary>
    bool IsHorizontal { get; set; }

    /// <summary>
    /// Gets or sets whether this scrollbar is hidden when disabled due to content area being bigger than the scrollbar.
    /// </summary>
    bool HideWhenDisabled { get; set; }

    /// <summary>
    /// Gets or sets whether this scrollbar is accepting mouse wheel events.
    /// </summary>
    bool IsAcceptingMouseWheelEvents { get; set; }

    /// <summary>
    /// Event that is called when the scroll bar's scroll position is changed.
    /// </summary>
    Action<int>? OnScrollChanged { get; set; }
}
