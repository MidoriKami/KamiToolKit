using System.Collections.Generic;
using KamiToolKit.Core.Primitives.ComponentNode;

namespace KamiToolKit.Core.Layout;

/// <summary>
/// Describes the focusable components exposed at the four edges of a navigation group.
/// </summary>
public sealed class ControllerNavigationBoundary(
    IReadOnlyList<ComponentNode> top,
    IReadOnlyList<ComponentNode> bottom,
    IReadOnlyList<ComponentNode> left,
    IReadOnlyList<ComponentNode> right,
    IReadOnlyList<ComponentNode>? topExit = null,
    IReadOnlyList<ComponentNode>? rightEntryTargets = null) {

    /// <summary>
    /// Gets the focusable components on the top edge of the group.
    /// </summary>
    public IReadOnlyList<ComponentNode> Top { get; } = top;

    /// <summary>
    /// Gets the focusable components that can navigate upward out of the group.
    /// </summary>
    public IReadOnlyList<ComponentNode> TopExit { get; } = topExit ?? top;

    /// <summary>
    /// Gets the focusable components on the bottom edge of the group.
    /// </summary>
    public IReadOnlyList<ComponentNode> Bottom { get; } = bottom;

    /// <summary>
    /// Gets the focusable components on the left edge of the group.
    /// </summary>
    public IReadOnlyList<ComponentNode> Left { get; } = left;

    /// <summary>
    /// Gets the focusable components on the right edge of the group.
    /// </summary>
    public IReadOnlyList<ComponentNode> Right { get; } = right;

    /// <summary>
    /// Gets the focusable components that should receive navigation when entering the group from the right.
    /// </summary>
    public IReadOnlyList<ComponentNode> RightEntryTargets { get; } = rightEntryTargets ?? right;

    /// <summary>
    /// Gets an empty boundary for groups without navigable components.
    /// </summary>
    public static ControllerNavigationBoundary Empty { get; } = new([], [], [], []);
}