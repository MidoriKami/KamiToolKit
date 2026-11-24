using System;
using System.Numerics;
using KamiToolKit.Core.Interfaces;
using KamiToolKit.UI.Nodes;

namespace KamiToolKit.Controls.Builders;

/// <summary>
/// Static class containing various node builders for building UI layouts easier.
/// </summary>
public static class NativeUiBuilder {
    /// <summary>
    /// Creates a new root vertical list node.
    /// </summary>
    public static ILayoutNode Create(Action<NodeBuilder> buildAction)
        => Create(default, default, buildAction);

    /// <summary>
    /// Creates a new root vertical list node with the specified size.
    /// </summary>
    public static ILayoutNode Create(Vector2 size, Action<NodeBuilder> buildAction)
        => Create(size, default, buildAction);

    /// <summary>
    /// Creates a new root vertical list node with the specified size and position.
    /// </summary>
    public static ILayoutNode Create(Vector2 size, Vector2 position, Action<NodeBuilder> buildAction) {
        var node = new VerticalListNode {
            StretchWidth = true,
            Size = size,
            Position = position,
        };

        var builder = new NodeBuilder<VerticalListNode>(node);

        buildAction.Invoke(builder);

        return builder.Build();
    }
}
