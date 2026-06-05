using System;
using System.Collections.Generic;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing the capabilities of a LayoutListNode.
/// </summary>
public interface ILayoutListNode {

    /// <summary>
    /// Nav index for use with setting contained nodes controller nav values.
    /// </summary>
    /// <remarks>
    /// Must be non-zero to apply nav to contained nodes.
    /// </remarks>
    int NavIndex { get; set; }

    /// <summary>
    /// Get a readonly list of the contained nodes.
    /// </summary>
    IReadOnlyList<NodeBase> Nodes { get; }

    /// <summary>
    /// When true, will clip the nodes contents, preventing any contained nodes
    /// outside the area of this node from being visible.
    /// </summary>
    /// <remarks>
    /// If a node is being partially clipped, it will be un-interactable.
    /// </remarks>
    bool ClipListContents { get; set; }

    /// <summary>
    /// Spacing between items, does not apply to the first item.
    /// </summary>
    float ItemSpacing { get; set; }

    /// <summary>
    /// Spacing to apply before the first item.
    /// </summary>
    float FirstItemSpacing { get; set; }

    /// <summary>
    /// An init only collection of nodes, to add a predefined amount of nodes to the list.
    /// This is the preferred way of adding nodes.
    /// </summary>
    ICollection<NodeBase> InitialNodes { set; }

    /// <summary>
    /// Get a readonly enumerable of the contained nodes of the specified type.
    /// </summary>
    /// <typeparam name="T">The NodeType to search for.</typeparam>
    /// <returns>An IEnumerable of Nodes.</returns>
    IEnumerable<T> GetNodes<T>() where T : NodeBase;

    /// <summary>
    /// Recalculates the contained layout, and controller navigation values if applicable.
    /// </summary>
    void RecalculateLayout();

    /// <summary>
    /// Adds multiple nodes to the list. Added nodes are considered to be owned by the list.
    /// </summary>
    /// <param name="nodes">The nodes to add.</param>
    void AddNode(IEnumerable<NodeBase> nodes);

    /// <summary>
    /// Adds a single node to the list.
    /// </summary>
    /// <param name="node">Node to add.</param>
    /// <remarks>
    /// While this function accepts a nullable node, that's just for convenience, if the node is null it will not be added.
    /// </remarks>
    void AddNode(NodeBase? node);

    /// <summary>
    /// Removes multiple nodes from the list. Removed nodes are disposed by the list.
    /// </summary>
    /// <param name="items">Nodes to remove.</param>
    void RemoveNode(IEnumerable<NodeBase> items);

    /// <summary>
    /// Remove a single node from the list. Removed nodes are disposed by the list.
    /// </summary>
    /// <param name="node">Node to remove.</param>
    void RemoveNode(NodeBase node);

    /// <summary>
    /// Adds a dummy node to the list, a standard ResNode with no contents for spacing/positioning.
    /// </summary>
    /// <param name="size">The size of the dummy to add.</param>
    void AddDummy(float size = 0.0f);

    /// <summary>
    /// Removes all nodes from the list. All nodes are disposed.
    /// </summary>
    void Clear();

    /// <summary>
    /// Sorts the contained nodes using the provided comparison.
    /// </summary>
    /// <param name="comparison"></param>
    void ReorderNodes(Comparison<NodeBase> comparison);
}
