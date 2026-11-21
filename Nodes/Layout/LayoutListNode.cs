using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Enums;
using KamiToolKit.Interfaces;

namespace KamiToolKit.Nodes;

/// <summary>
/// Abstract base class for nodes that are intended to help with laying out other nodes.
/// </summary>
public abstract class LayoutListNode : ResNode, ILayoutListNode {

    /// <inheritdoc/>
    public int NavIndex { get; set; }

    /// <inheritdoc/>
    public int NavLeft { get; set; }

    /// <inheritdoc/>
    public int NavRight { get; set; }

    /// <inheritdoc/>
    public int NavUp { get; set; }

    /// <inheritdoc/>
    public int NavDown { get; set; }

    /// <inheritdoc/>
    public IEnumerable<T> GetNodes<T>() where T : NodeBase => NodeList.OfType<T>();

    /// <inheritdoc/>
    public IReadOnlyList<NodeBase> Nodes => NodeList;

    /// <inheritdoc/>
    public bool ClipContents {
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
    public float ItemSpacing { get; set; }

    /// <inheritdoc/>
    public float FirstItemSpacing { get; set; }

    /// <inheritdoc/>
    public void UpdateLayout() {
        if (suppressRecalculateLayout) return;

        // Save old sizes to check if sizes were changed later for recursing.
        var oldSizes = NodeList.Select(n => n.Size).ToArray();

        UpdateFillerNodes();
        UpdateChildrenLayout(oldSizes);
        UpdateNavigation();
    }

    // If any children are ILayoutFiller, resize them.
    private void UpdateFillerNodes() {
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

    private void UpdateChildrenLayout(Vector2[] oldSizes) {
        if (NodeList.Count is 0) return;

        OnUpdateSizes();
        OnUpdateLayout();

        // update children, but only if their sizes are different.
        foreach(var (node, oldSize) in NodeList.Zip(oldSizes)) {
            if (node is not ILayoutUpdatable subNode) continue;

            var newSize = node.Size;
            if (Vector2.Distance(oldSize, newSize) > 0.5f) {
                subNode.UpdateLayout();
            }
        }

        // Reapply sizes in case children changed to fit something.
        OnUpdateSizes();
    }

    /// <inheritdoc/>
    public void UpdateNavigation() {
        if (suppressRecalculateLayout) return;

        foreach (var node in NodeList) {
            if (node is INavigationUpdatable controllerNavigable) {
                controllerNavigable.UpdateNavigation();
            }
        }

        OnUpdateControllerNav();
    }

    /// <inheritdoc/>
    public ICollection<NodeBase> InitialNodes {
        set => AddNode(value);
    }

    /// <inheritdoc/>
    public virtual void AddNode(IEnumerable<NodeBase> nodes) {
        using (SuppressLayoutRecalculation()) {
            foreach (var node in nodes) {
                AddNode(node);
            }
        }
        UpdateLayout();
    }

    /// <inheritdoc/>
    public virtual void AddNode(NodeBase? node) {
        if (node is null) return;

        NodeList.Add(node);
        node.AttachNode(this);

        UpdateLayout();
    }

    /// <inheritdoc/>
    public void RemoveNode(IEnumerable<NodeBase> items) {
        using (SuppressLayoutRecalculation()) {
            foreach (var node in items) {
                RemoveNode(node);
            }
        }
        UpdateLayout();
    }

    /// <inheritdoc/>
    public virtual void RemoveNode(NodeBase node) {
        if (!NodeList.Contains(node)) return;

        NodeList.Remove(node);
        node.Dispose();

        UpdateLayout();
    }

    /// <inheritdoc/>
    public void AddDummy(float size = 0.0f) {
        var dummyNode = new ResNode {
            Size = new Vector2(size, size),
        };

        AddNode(dummyNode);
    }

    /// <inheritdoc/>
    public virtual void Clear() {
        using (SuppressLayoutRecalculation()) {
            foreach (var node in NodeList.ToList()) {
                RemoveNode(node);
            }
        }
        UpdateLayout();
    }

    /// <inheritdoc/>
    public void ReorderNodes(Comparison<NodeBase> comparison) {
        NodeList.Sort(comparison);
        UpdateLayout();
    }

    /// <summary>
    /// Gets the list of nodes that this <see cref="LayoutListNode"/> is managing.
    /// </summary>
    protected List<NodeBase> NodeList { get; } = [];

    /// <summary>
    /// Temporarily suppresses layout recalculation while performing batch operations.
    /// </summary>
    private ScopeGuard SuppressLayoutRecalculation() {
        suppressRecalculateLayout = true;
        return new ScopeGuard(() => suppressRecalculateLayout = false);
    }

    private readonly struct ScopeGuard(Action action) : IDisposable {
        public void Dispose() => action();
    }

    /// <summary>
    /// Whether layout recalculation is currently suppressed during batch operations.
    /// </summary>
    private bool suppressRecalculateLayout;

    /// <summary>
    /// Invoked when this node needs to update child node sizes.
    /// </summary>
    protected abstract void OnUpdateSizes();

    /// <summary>
    /// Invoked when this node needs to update child node positions.
    /// </summary>
    protected abstract void OnUpdateLayout();

    /// <summary>
    /// Invoked when this node needs to update controller navigation layout.
    /// </summary>
    protected abstract void OnUpdateControllerNav();

    /// <summary>
    /// Invoked during layout to allow subclasses to adjust individual node properties.
    /// </summary>
    protected virtual void AdjustNode(NodeBase node) { }
}
