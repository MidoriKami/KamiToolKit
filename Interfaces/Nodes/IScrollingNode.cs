using KamiToolKit.BaseTypes;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a scrolling area node.
/// </summary>
public interface IScrollingNode<out T> : ILayoutUpdatable, ILayoutFiller where T : NodeBase, new() {

    /// <summary>
    /// Gets a reference to the contained content node.
    /// </summary>
    T ContentNode { get; }

    /// <summary>
    /// Hides the Scroll Bar Node when its disabled due to the content being larger than the scroll.
    /// </summary>
    bool AutoHideScrollBar { get; set; }

    /// <summary>
    /// Set the scroll speed, default 24px per scroll.
    /// </summary>
    int ScrollSpeed { get; set; }

    /// <summary>
    /// Gets or sets whether this scrolling node scrolls horizontally.
    /// </summary>
    bool IsHorizontal { get; set; }

    /// <summary>
    /// Sets the scroll position to the start.
    /// </summary>
    void ScrollToStart();

    /// <summary>
    /// Sets the scroll position to the end.
    /// </summary>
    void ScrollToEnd();
}
