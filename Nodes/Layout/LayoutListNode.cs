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
/// Abstract base class for layout nodes that manage a collection of child nodes.
/// Provides the shared ILayoutListNode contract: navigation properties, child management
/// methods that automatically trigger layout and navigation updates, and the layout pipeline.
///
/// Subclasses only need to provide their internal data structure and the layout algorithms.
/// </summary>
public abstract class LayoutListNode : LayoutNodeBase, ILayoutListNode {

    // ── IControllerNavigable ──────────────────────────────────────────────────────

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

    // ── ILayoutNode ───────────────────────────────────────────────────────────────

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
    public float ItemSpacing { get; set; }

    /// <inheritdoc/>
    public float FirstItemSpacing { get; set; }

    /// <inheritdoc/>
    public ICollection<NodeBase> InitialNodes {
        set => AddNode(value);
    }

    /// <inheritdoc/>
    public virtual IEnumerable<T> GetNodes<T>() where T : NodeBase
        => Nodes.OfType<T>();

    /// <inheritdoc/>
    public virtual IReadOnlyList<NodeBase> Nodes => NodeList;

    // ── Child management ──────────────────────────────────────────────────────────

    /// <summary>
    /// Gets the list of nodes that this <see cref="LayoutListNode"/> is managing.
    /// </summary>
    protected List<NodeBase> NodeList { get; } = [];

    /// <summary>
    /// Adds a single node to the list. Override this to implement the actual addition.
    /// </summary>
    protected virtual void AddNodeInternal(NodeBase node) {
        NodeList.Add(node);
        node.AttachNode(this);
    }

    /// <summary>
    /// Removes a single node from the list. Override this to implement the actual removal.
    /// </summary>
    protected virtual bool RemoveNodeInternal(NodeBase node) {
        if (!NodeList.Contains(node)) return false;
        NodeList.Remove(node);
        return true;
    }

    /// <summary>
    /// Clears all nodes from the list. Override this to implement the actual clear.
    /// </summary>
    protected virtual void ClearInternal() {
        foreach (var node in NodeList.ToArray()) {
            node.Dispose();
        }
        NodeList.Clear();
    }

    /// <summary>
    /// Reorders the nodes using the provided comparison. Override this to implement the actual reorder.
    /// </summary>
    protected virtual void ReorderInternal(Comparison<NodeBase> comparison) {
        NodeList.Sort(comparison);
    }

    /// <inheritdoc/>
    public virtual void AddNode(IEnumerable<NodeBase> nodes) {
        using (SuppressLayoutRecalculation()) {
            foreach (var node in nodes) {
                AddNodeInternal(node);
            }
        }
        UpdateLayout();
        UpdateNavigation();
    }

    /// <inheritdoc/>
    public virtual void AddNode(NodeBase? node) {
        if (node is null) return;
        AddNodeInternal(node);
        UpdateLayout();
        UpdateNavigation();
    }

    /// <inheritdoc/>
    public void RemoveNode(IEnumerable<NodeBase> items) {
        using (SuppressLayoutRecalculation()) {
            foreach (var node in items) {
                RemoveNodeInternal(node);
            }
        }
        UpdateLayout();
        UpdateNavigation();
    }

    /// <inheritdoc/>
    public virtual void RemoveNode(NodeBase node) {
        if (!RemoveNodeInternal(node)) return;
        node.Dispose();
        UpdateLayout();
        UpdateNavigation();
    }

    /// <inheritdoc/>
    public void AddDummy(float size = 0.0f) {
        AddNode(new ResNode { Size = new Vector2(size, size) });
    }

    /// <inheritdoc/>
    public void Clear() {
        ClearInternal();
        UpdateLayout();
        UpdateNavigation();
    }

    /// <inheritdoc/>
    public void ReorderNodes(Comparison<NodeBase> comparison) {
        ReorderInternal(comparison);
        UpdateLayout();
        UpdateNavigation();
    }

    // ── Navigation ────────────────────────────────────────────────────────────────

    /// <inheritdoc/>
    public void UpdateNavigation() {
        foreach (var node in Nodes) {
            if (node is INavigationUpdatable controllerNavigable) {
                controllerNavigable.UpdateNavigation();
            }
        }

        OnUpdateControllerNav();
    }

    // ── Layout pipeline ───────────────────────────────────────────────────────────

    /// <inheritdoc/>
    protected override void DoUpdateLayout() {
        // Save old sizes to check if sizes were changed later for recursing.
        var oldSizes = NodeList.Select(n => n.Size).ToArray();

        UpdateLayoutFillerNodes();
        UpdateChildrenLayout(oldSizes);
    }

    /// <summary>
    /// If any children are ILayoutFiller, resize them.
    /// </summary>
    protected virtual void UpdateLayoutFillerNodes() {
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
    protected virtual void UpdateChildrenLayout(Vector2[] oldSizes) {
        if (NodeList.Count is 0) return;

        OnUpdateSizes();
        OnUpdateLayout();

        foreach (var (node, oldSize) in NodeList.Zip(oldSizes)) {
            if (node is not ILayoutUpdatable subNode) continue;

            var newSize = node.Size;
            if (Vector2.Distance(oldSize, newSize) > 0.5f) {
                subNode.UpdateLayout();
            }
        }

        // Reapply sizes in case children changed to fit something.
        OnUpdateSizes();
    }

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
}
