using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Interfaces;

namespace KamiToolKit.Nodes;

/// <summary>
/// Abstract base class for nodes that are intended to help with laying out other nodes.
/// </summary>
public abstract class LayoutListNode : ResNode, ILayoutListNode {

    /// <summary>
    /// Nav index for use with setting contained nodes controller nav values.
    /// </summary>
    /// <remarks>
    /// Must be non-zero to apply nav to contained nodes.
    /// </remarks>
    public int NavIndex { get; set; }

    /// <summary>
    /// Get a readonly enumerable of the contained nodes of the specified type.
    /// </summary>
    /// <typeparam name="T">The NodeType to search for.</typeparam>
    /// <returns>An IEnumerable of Nodes.</returns>
    public IEnumerable<T> GetNodes<T>() where T : NodeBase => NodeList.AsReadOnly().OfType<T>();

    /// <summary>
    /// Get a readonly list of the contained nodes.
    /// </summary>
    public IReadOnlyList<NodeBase> Nodes => NodeList;

    /// <summary>
    /// When true, will clip the nodes contents, preventing any contained nodes
    /// outside the area of this node from being visible.
    /// </summary>
    /// <remarks>
    /// If a node is being partially clipped, it will be un-interactable.
    /// </remarks>
    public bool ClipListContents {
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

    /// <summary>
    /// Spacing between items, does not apply to the first item.
    /// </summary>
    public float ItemSpacing { get; set; }

    /// <summary>
    /// Spacing to apply before the first item.
    /// </summary>
    public float FirstItemSpacing { get; set; }

    /// <summary>
    /// Recalculates the contained layout, and controller navigation values if applicable.
    /// </summary>
    public void RecalculateLayout() {
        if (suppressRecalculateLayout) return;

        OnRecalculateLayout();

        if (NavIndex is not 0) {
            OnRecalculateNavigation();
        }

        foreach (var node in NodeList) {
            if (node is LayoutListNode subNode) {
                subNode.RecalculateLayout();
            }
        }
    }

    /// <summary>
    /// An init only collection of nodes, to add a predefined amount of nodes to the list.
    /// This is the preferred way of adding nodes.
    /// </summary>
    public ICollection<NodeBase> InitialNodes {
        set => AddNode(value);
    }

    /// <summary>
    /// Adds multiple nodes to the list. Added nodes are considered to be owned by the list.
    /// </summary>
    /// <param name="nodes">The nodes to add.</param>
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

    /// <summary>
    /// Adds a single node to the list.
    /// </summary>
    /// <param name="node">Node to add.</param>
    /// <remarks>
    /// While this function accepts a nullable node, that's just for convenience, if the node is null it will not be added.
    /// </remarks>
    public virtual void AddNode(NodeBase? node) {
        if (node is null) return;

        NodeList.Add(node);

        node.AttachNode(this);

        RecalculateLayout();
    }

    /// <summary>
    /// Removes multiple nodes from the list. Removed nodes are disposed by the list.
    /// </summary>
    /// <param name="items">Nodes to remove.</param>
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

    /// <summary>
    /// Remove a single node from the list. Removed nodes are disposed by the list.
    /// </summary>
    /// <param name="node">Node to remove.</param>
    public virtual void RemoveNode(NodeBase node) {
        if (!NodeList.Contains(node)) return;

        NodeList.Remove(node);
        node.Dispose();

        RecalculateLayout();
    }

    /// <summary>
    /// Adds a dummy node to the list, a standard ResNode with no contents for spacing/positioning.
    /// </summary>
    /// <param name="size">The size of the dummy to add.</param>
    public void AddDummy(float size = 0.0f) {
        var dummyNode = new ResNode {
            Size = new Vector2(size, size),
        };

        AddNode(dummyNode);
    }

    /// <summary>
    /// Removes all nodes from the list. All nodes are disposed.
    /// </summary>
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

    /// <summary>
    /// Sorts the contained nodes using the provided comparison.
    /// </summary>
    /// <param name="comparison"></param>
    public void ReorderNodes(Comparison<NodeBase> comparison) {
        NodeList.Sort(comparison);
        RecalculateLayout();
    }

    protected readonly List<NodeBase> NodeList = [];
    protected abstract void OnRecalculateLayout();
    protected abstract void OnRecalculateNavigation();
    protected virtual void AdjustNode(NodeBase node) { }

    private bool suppressRecalculateLayout;
}
