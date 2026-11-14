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
    public void RecalculateLayout() {
        if (suppressRecalculateLayout) return;

        var remainingHeight = Height + FirstItemSpacing;
        var remainingWidth = Width + FirstItemSpacing;

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

            if (node is ILayoutUpdatable subNode) {
                subNode.RecalculateLayout();
            }
        }

        OnRecalculateLayout();

        // todo: temp until callsites can be updated
        RecalculateNavigation();

        foreach (var node in NodeList) {
            if (node is ILayoutUpdatable subNode) {
                subNode.RecalculateLayout();
            }
        }
    }

    /// <inheritdoc/>
    public void RecalculateNavigation() {
        if (suppressRecalculateLayout) return;

        foreach (var node in NodeList) {
            if (node is INavigationRecalculable controllerNavigable) {
                controllerNavigable.RecalculateNavigation();
            }
        }

        OnRecalculateNavigation();

        foreach (var node in NodeList) {
            if (node is INavigationRecalculable controllerNavigable) {
                controllerNavigable.RecalculateNavigation();
            }
        }
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
        RecalculateLayout();
    }

    /// <inheritdoc/>
    public virtual void AddNode(NodeBase? node) {
        if (node is null) return;

        NodeList.Add(node);

        node.AttachNode(this);

        RecalculateLayout();
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
        RecalculateLayout();
    }

    /// <inheritdoc/>
    public virtual void RemoveNode(NodeBase node) {
        if (!NodeList.Contains(node)) return;

        NodeList.Remove(node);
        node.Dispose();

        RecalculateLayout();
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
        RecalculateLayout();
    }

    /// <inheritdoc/>
    public void ReorderNodes(Comparison<NodeBase> comparison) {
        NodeList.Sort(comparison);
        RecalculateLayout();
    }

    /// <summary>
    /// Gets the list of nodes that this <see cref="LayoutListNode"/> is managing.
    /// </summary>
    protected List<NodeBase> NodeList { get; }= [];

    /// <summary>
    /// Function that is invoked when this node needs to rebuild its layout.
    /// </summary>
    protected abstract void OnRecalculateLayout();

    /// <summary>
    /// Function that is invoked when this node needs to rebuild its controller navigation layout.
    /// </summary>
    protected abstract void OnRecalculateNavigation();

    /// <summary>
    /// Adjusts the nodes properties when <see cref="RecalculateLayout"/> is invoked.
    /// </summary>
    protected virtual void AdjustNode(NodeBase node) { }

    private bool suppressRecalculateLayout;
}
