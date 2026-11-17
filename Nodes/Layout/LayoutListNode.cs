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

        foreach (var node in NodeList) {
            if (node is ILayoutUpdatable subNode) {
                subNode.UpdateLayout();
            }
        }

        if (NodeList.Count > 0) {
            OnUpdateSizes();
            OnUpdateLayout();
        }

        foreach (var node in NodeList) {
            if (node is ILayoutUpdatable subNode) {
                subNode.UpdateLayout();
            }
        }

        // Reapply sizes in case children changed to fit something.
        if (NodeList.Count > 0) {
            OnUpdateSizes();
        }

        UpdateNavigation();
    }

    /// <inheritdoc/>
    public void UpdateNavigation() {
        if (suppressRecalculateLayout) return;

        // Propagate navigation update to children first, so nested containers can resolve
        // their internal navigation before the parent sets its own navigation indices.
        foreach (var node in NodeList) {
            if (node is INavigationUpdatable controllerNavigable) {
                controllerNavigable.UpdateNavigation();
            }
        }

        // Then update this container's own navigation (NavIndex, NavUp, NavDown, etc.).
        OnUpdateControllerNav();
    }

    /// <inheritdoc/>
    public ICollection<NodeBase> InitialNodes {
        set => AddNode(value);
    }

    /// <inheritdoc/>
    public virtual void AddNode(IEnumerable<NodeBase> nodes) {
        suppressRecalculateLayout = true;
        try {
            foreach (var node in nodes) {
                AddNode(node);
            }
        } finally {
            suppressRecalculateLayout = false;
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
        suppressRecalculateLayout = true;
        try {
            foreach (var node in items) {
                RemoveNode(node);
            }
        } finally {
            suppressRecalculateLayout = false;
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
        suppressRecalculateLayout = true;
        try {
            foreach (var node in NodeList.ToList()) {
                RemoveNode(node);
            }
        } finally {
            suppressRecalculateLayout = false;
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
    protected List<NodeBase> NodeList { get; }= [];

    /// <summary>
    /// Function that is invoked when this node needs to update child node sizes.
    /// </summary>
    protected abstract void OnUpdateSizes();

    /// <summary>
    /// Function that is invoked when this node needs to update child node positions.
    /// </summary>
    protected abstract void OnUpdateLayout();

    /// <summary>
    /// Function that is invoked when this node needs to update controller navigation layout.
    /// </summary>
    protected abstract void OnUpdateControllerNav();

    /// <summary>
    /// Adjusts the nodes properties when <see cref="UpdateLayout"/> is invoked.
    /// </summary>
    protected virtual void AdjustNode(NodeBase node) { }

    private bool suppressRecalculateLayout;
}
