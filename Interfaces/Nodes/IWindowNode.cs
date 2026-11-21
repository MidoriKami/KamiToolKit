using System.Numerics;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a window node with title, subtitle, and button controls.
/// </summary>
public interface IWindowNode {

    /// <summary>
    /// Gets or sets the title text.
    /// </summary>
    ReadOnlySeString Title { get; set; }

    /// <summary>
    /// Gets or sets the subtitle text.
    /// </summary>
    ReadOnlySeString Subtitle { get; set; }

    /// <summary>
    /// Gets or sets whether the close button is visible.
    /// </summary>
    bool ShowCloseButton { get; set; }

    /// <summary>
    /// Gets or sets whether the gear button is visible.
    /// </summary>
    bool ShowConfigButton { get; set; }

    /// <summary>
    /// Gets or sets whether the help button is visible.
    /// </summary>
    bool ShowHelpButton { get; set; }

    /// <summary>
    /// Gets or sets whether the border node is visible.
    /// </summary>
    bool Focused { get; set; }

    /// <summary>
    /// Gets the area of the node minus the header size.
    /// </summary>
    Vector2 ContentSize { get; }

    /// <summary>
    /// Gets the position where the content starts, below the header node.
    /// </summary>
    Vector2 ContentStartPosition { get; }

    /// <summary>
    /// Gets the height of the header.
    /// </summary>
    float HeaderHeight { get; }

    /// <summary>
    /// Gets the node that should be focused.
    /// </summary>
    ResNode WindowHeaderFocusNode { get; }

    /// <summary>
    /// Sets the nodes title and subtitle.
    /// </summary>
    void SetTitle(string title, string? subtitle = null);
}
