using System;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing elements that receive hover feedback via navigation input.
/// </summary>
public interface INavHoverable {

    /// <summary>
    /// Action that is invoked when this node is hovered via vertical navigation.
    /// </summary>
    Action? OnHoverStart { get; set; }

    /// <summary>
    /// Action that is invoked when this node is no longer hovered via navigation.
    /// </summary>
    Action? OnHoverEnd { get; set; }
}
