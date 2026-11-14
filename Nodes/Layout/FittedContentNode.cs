using System.Numerics;
using KamiToolKit.Enums;
using KamiToolKit.Interfaces;

namespace KamiToolKit.Nodes;

/// <summary>
/// A layout node that fits all added child nodes to its own size.
/// </summary>
public sealed class FittedContentNode : LayoutListNode, ILayoutFiller {

    /// <inheritdoc />
    public LayoutFill LayoutFill { get; set; } = LayoutFill.Both;

    /// <inheritdoc />
    protected override void OnRecalculateLayout() {
        var contentSize = new Vector2(Width, Height);

        foreach (var node in NodeList) {
            node.Position = new Vector2(0.0f, 0.0f);
            node.Size = contentSize;

            if (node is ILayoutUpdatable updatable) {
                updatable.RecalculateLayout();
            }
        }
    }

    /// <inheritdoc />
    protected override void OnRecalculateNavigation() {
        // todo: eventually do this.
    }
}
