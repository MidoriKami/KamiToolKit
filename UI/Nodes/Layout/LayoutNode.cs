using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Core;
using KamiToolKit.Core.Enums;
using KamiToolKit.Core.Interfaces;
using KamiToolKit.Core.Layout;
using KamiToolKit.Core.Primitives;

namespace KamiToolKit.UI.Nodes.Layout;

/// <summary>
/// Abstract base for layouts that own ordered child nodes and provide a size-aware update lifecycle.
/// </summary>
public abstract class LayoutNode : ResNode, ILayoutListNode {

    /// <inheritdoc/>
    public virtual bool ClipContents {
        get => NodeFlags.HasFlag(NodeFlags.Clip);
        set {
            if (value) {
                AddNodeFlags(NodeFlags.Clip);
            }
            else {
                RemoveNodeFlags(NodeFlags.Clip);
            }
        }
    }

    /// <inheritdoc/>
    public float ItemSpacing {
        get;
        set {
            field = value;
            UpdateLayout();
        }
    }

    /// <inheritdoc/>
    public float FirstItemSpacing {
        get;
        set {
            field = value;
            UpdateLayout();
        }
    }

    /// <inheritdoc/>
    public ICollection<NodeBase> InitialNodes {
        set => AddNode(value);
    }

    /// <inheritdoc/>
    public virtual IEnumerable<T> GetNodes<T>() where T : NodeBase
        => Nodes.OfType<T>();

    /// <inheritdoc/>
    public virtual IReadOnlyList<NodeBase> Nodes => NodeList;

    /// <summary>
    /// Gets the list of nodes managed by this layout.
    /// </summary>
    protected List<NodeBase> NodeList { get; } = [];

    /// <summary>
    /// Updates the contained layout. Subclasses should override <see cref="DoUpdateLayout"/>
    /// to perform the actual layout work.
    /// </summary>
    public virtual void UpdateLayout() {
        if (suppressRecalculateLayout) return;

        // If we're already inside a layout update, just mark dirty for the final pass.
        if (isUpdatingLayout) {
            layoutDirty = true;
            return;
        }

        isUpdatingLayout = true;
        try {
            DoUpdateLayout();
        }
        finally {
            isUpdatingLayout = false;
        }

        // If a nested call marked us dirty, do one more pass to propagate the changes.
        if (layoutDirty) {
            layoutDirty = false;
            DoUpdateLayout();
        }

        RefreshNavigation();
    }

    /// <summary>
    /// Adds a single node to the managed collection. Override this to implement specialized storage.
    /// </summary>
    protected virtual void AddNodeInternal(NodeBase node) {
        NodeList.Add(node);
        node.AttachNode(this);
    }

    /// <summary>
    /// Removes a single node from the managed collection. Override this to implement specialized storage.
    /// </summary>
    protected virtual bool RemoveNodeInternal(NodeBase node) {
        if (!NodeList.Contains(node)) return false;
        NodeList.Remove(node);
        return true;
    }

    /// <summary>
    /// Clears all managed nodes. Override this to implement specialized storage.
    /// </summary>
    protected virtual void ClearInternal() {
        foreach (var node in NodeList.ToArray()) {
            node.Dispose();
        }
        NodeList.Clear();
    }

    /// <summary>
    /// Reorders managed nodes. Override this to implement specialized storage.
    /// </summary>
    private void ReorderInternal(Comparison<NodeBase> comparison)
        => NodeList.Sort(comparison);

    /// <inheritdoc/>
    public virtual void AddNode(IEnumerable<NodeBase> nodes) {
        using (SuppressLayoutRecalculation()) {
            foreach (var node in nodes) {
                AddNodeInternal(node);
            }
        }
        UpdateLayout();
    }

    /// <inheritdoc/>
    public virtual void AddNode(NodeBase? node) {
        if (node is null) return;
        AddNodeInternal(node);
        UpdateLayout();
    }

    /// <inheritdoc/>
    public void RemoveNode(IEnumerable<NodeBase> items) {
        using (SuppressLayoutRecalculation()) {
            foreach (var node in items) {
                RemoveNodeInternal(node);
            }
        }
        UpdateLayout();
    }

    /// <inheritdoc/>
    public virtual void RemoveNode(NodeBase node) {
        if (!RemoveNodeInternal(node)) return;
        node.Dispose();
        UpdateLayout();
    }

    /// <inheritdoc/>
    public void AddSpacer(float size = 0.0f)
        => AddNode(new ResNode { Size = new Vector2(size, size) });

    /// <inheritdoc/>
    public void Clear() {
        ClearInternal();
        UpdateLayout();
    }

    /// <inheritdoc/>
    public void ReorderNodes(Comparison<NodeBase> comparison) {
        ReorderInternal(comparison);
        UpdateLayout();
    }

    private void DoUpdateLayout() {
        var oldSizes = NodeList.Select(node => node.Size).ToArray();
        UpdateLayoutFillerNodes();
        UpdateChildrenLayout(oldSizes);
    }

    /// <summary>
    /// Resizes children implementing <see cref="ILayoutFiller"/> to the remaining layout space.
    /// </summary>
    private void UpdateLayoutFillerNodes() {
        var remainingHeight = Height - FirstItemSpacing;
        var remainingWidth = Width - FirstItemSpacing;

        foreach (var node in NodeList) {
            if (node is ILayoutFiller filler) {
                if (filler.LayoutFill.HasFlag(LayoutFill.Vertical)) {
                    node.Height = remainingHeight;
                }

                if (filler.LayoutFill.HasFlag(LayoutFill.Horizontal)) {
                    node.Width = remainingWidth;
                }
            }

            remainingHeight -= node.Height + ItemSpacing;
            remainingWidth -= node.Width + ItemSpacing;
        }
    }

    /// <summary>
    /// Updates child layout and recursively updates children whose sizes changed.
    /// </summary>
    private void UpdateChildrenLayout(Vector2[] oldSizes) {
        if (NodeList.Count is 0) return;

        OnUpdateSizes();
        OnUpdateLayout();

        foreach (var (node, oldSize) in NodeList.Zip(oldSizes)) {
            if (node is not ILayoutUpdatable subNode) continue;

            if (Vector2.Distance(oldSize, node.Size) > 0.5f) {
                subNode.UpdateLayout();
            }
        }

        OnUpdateSizes();
    }

    /// <summary>
    /// Invoked when this layout needs to update child node sizes.
    /// </summary>
    /// <remarks>
    /// Optional override as not all layout containers need the capability to update sizes.
    /// </remarks>
    protected virtual void OnUpdateSizes() { }

    /// <summary>
    /// Invoked when this layout needs to update child node positions.
    /// </summary>
    protected abstract void OnUpdateLayout();

    /// <summary>
    /// Invoked when this layout needs to update its navigation layout.
    /// </summary>
    protected abstract void OnNavigationUpdate(NavigationContext context);

    private void RefreshNavigation() {
        var root = this;
        for (var parent = ParentNode; parent is not null; parent = parent.ParentNode) {
            if (parent is LayoutNode layout) {
                root = layout;
            }
        }

        root.UpdateNavigationTree(new NavigationContext { CurrentIndex = 1 });
    }

    private void UpdateNavigationTree(NavigationContext context) {
        foreach (var node in NodeList) {
            UpdateNavigationNode(node, context);
        }

        OnNavigationUpdate(context);
    }

    private static void UpdateNavigationNode(NodeBase node, NavigationContext context) {
        if (node is INavigationTarget target) {
            var index = context.CurrentIndex;
            target.NavIndex = index;
            target.NavLeft = index is 0 ? (byte)0 : context.Left;
            target.NavRight = index is 0 ? (byte)0 : context.Right;
            target.NavUp = index is 0 ? (byte)0 : context.Up;
            target.NavDown = index is 0 ? (byte)0 : context.Down;

            context.CurrentIndex = index switch {
                0 or byte.MaxValue => 0,
                _ => (byte)(index + 1),
            };
            return;
        }

        if (node is LayoutNode layout) {
            layout.UpdateNavigationTree(context);
            return;
        }

        foreach (var child in node.ChildNodes) {
            UpdateNavigationNode(child, context);
        }
    }

    /// <summary>
    /// Whether a layout update is currently in progress.
    /// </summary>
    private bool isUpdatingLayout;

    /// <summary>
    /// Set when a nested UpdateLayout call needs the parent to update again after completion.
    /// </summary>
    private bool layoutDirty;

    /// <summary>
    /// Whether layout recalculation is currently suppressed during batch operations.
    /// </summary>
    private bool suppressRecalculateLayout;

    /// <summary>
    /// Temporarily suppresses layout recalculation while performing batch operations.
    /// </summary>
    protected ScopeGuard SuppressLayoutRecalculation() {
        suppressRecalculateLayout = true;
        return new ScopeGuard(() => suppressRecalculateLayout = false);
    }

    /// <summary>
    /// Automatically updates the layout when this node's size changes.
    /// </summary>
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        // Only update layout if the size has actually changed, to prevent unnecessary updates.
        if (Size != lastLayoutSize) {
            lastLayoutSize = Size;
            UpdateLayout();
        }
    }

    /// <summary>
    /// The last size that triggered a layout update.
    /// </summary>
    private Vector2 lastLayoutSize;
}
