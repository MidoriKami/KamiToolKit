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
    protected override void OnUpdateSizes() {
        var contentSize = new Vector2(Width, Height);
        foreach (var node in NodeList) {
            node.Size = contentSize;
        }
    }

    /// <inheritdoc />
    protected override void OnUpdateLayout() {
        foreach (var node in NodeList) {
            node.Position = new Vector2(0.0f, 0.0f);
        }
    }

    /// <inheritdoc />
    protected override void OnUpdateControllerNav() {
        // Not implemented yet.
    }
}
