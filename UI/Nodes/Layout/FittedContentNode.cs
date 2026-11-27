using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using KamiToolKit.Core.Enums;
using KamiToolKit.Core.Layout;
using KamiToolKit.Core.Primitives.NodeBase;

namespace KamiToolKit.UI.Nodes.Layout;

/// <summary>
/// A layout node that fits all added child nodes to its own size.
/// </summary>
public sealed class FittedContentNode : LayoutListNode, ILayoutFiller {

    private readonly List<ControllerNavigationContext> childContexts = [];
    private int childContextStartIndex;

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
        SetNavigationBoundary(CreateNavigationBoundary(GetNavigationChildGroups()));
    }

    /// <inheritdoc/>
    public override void UpdateNavigation(ControllerNavigationContext context) {
        ArgumentNullException.ThrowIfNull(context);

        childContexts.Clear();
        childContextStartIndex = context.NextIndex;

        base.UpdateNavigation(context);

        var exclusiveNextIndex = childContexts.Count is 0
            ? childContextStartIndex
            : childContexts.Max(childContext => childContext.NextIndex);
        context.TryReserveThrough(exclusiveNextIndex);
    }

    /// <inheritdoc/>
    protected override ControllerNavigationContext CreateChildNavigationContext(NodeBase child, ControllerNavigationContext context) {
        var childContext = context.CreateLocalScope(childContextStartIndex);
        childContexts.Add(childContext);
        return childContext;
    }
}
