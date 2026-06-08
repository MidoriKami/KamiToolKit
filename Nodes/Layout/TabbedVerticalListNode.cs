using System;
using System.Collections.Generic;
using System.Linq;
using KamiToolKit.BaseTypes;
using KamiToolKit.BaseTypes.ComponentNode;
using KamiToolKit.Classes;
using KamiToolKit.Interfaces;

namespace KamiToolKit.Nodes;

/// <summary>
/// A specialized vertical list node with features for adding nodes with specified tab values.
/// </summary>
public class TabbedVerticalListNode : ResNode, ILayoutListNode {

    // <inheritdoc/>
    public int NavIndex { get; set; }

    /// <summary>
    /// Gets or sets the left nav index for this list.
    /// </summary>
    public int NavLeft { get; set; }

    /// <summary>
    /// Gets or sets the right nav index for this list.
    /// </summary>
    public int NavRight { get; set; }

    // <inheritdoc/>
    public IReadOnlyList<NodeBase> Nodes => nodeList.Select(node => node.Node).ToList();

    // <inheritdoc/>
    public bool ClipListContents { get; set; }

    // <inheritdoc/>
    public float ItemSpacing { get; set; }

    // <inheritdoc/>
    public float FirstItemSpacing { get; set; }

    // <inheritdoc/>
    public ICollection<NodeBase> InitialNodes {
        set => AddNode(value);
    }

    /// <summary>
    /// Special implementation for allowing initial tabbed node entries.
    /// </summary>
    public ICollection<TabbedListEntry> InitialTabbedNodes {
        set {
            foreach (var entry in value) {
                AddNode(entry.TabIndex, entry.Node);
            }
        }
    }

    // <inheritdoc/>
    public IEnumerable<T> GetNodes<T>() where T : NodeBase
        => Nodes.OfType<T>();

    // <inheritdoc/>
    public void RecalculateLayout() {
        if (suppressRecalculateLayout) return;

        OnRecalculateLayout();

        if (NavIndex is not 0) {
            OnRecalculateNavigation();
        }

        foreach (var node in Nodes) {
            if (node is LayoutListNode subNode) {
                subNode.RecalculateLayout();
            }
        }
    }

    /// <summary>
    /// How much space to add for each tab value.
    /// </summary>
    public float TabSize { get; set; } = 18.0f;

    /// <summary>
    /// If true, adjusts the width of all contained nodes
    /// to match the width of this node when recalculating layout.
    /// </summary>
    public bool FitWidth { get; set; }

    /// <summary>
    /// Resizes this layout node to fit the height of the contained nodes.
    /// </summary>
    public bool FitContents { get; set; } = true;

    /// <summary>
    /// The current Tab amount.
    /// </summary>
    public int TabStep { get; set; }

    /// <summary>
    /// Adds <see cref="tabAmount"/> to <see cref="TabStep"/> causing all
    /// following nodes tab to be increased by the specified amount.
    /// </summary>
    /// <param name="tabAmount">Tab value to increase.</param>
    public void AddTab(int tabAmount)
        => TabStep += tabAmount;

    /// <summary>
    /// Adds <see cref="tabAmount"/> to <see cref="TabStep"/> causing all
    /// following nodes tab to be decreased by the specified amount.
    /// </summary>
    /// <param name="tabAmount">Tab value to decrease.</param>
    public void SubtractTab(int tabAmount)
        => TabStep -= tabAmount;

    /// <summary>
    /// Adds several nodes with the specified tab index to the list.
    /// </summary>
    /// <param name="tabIndex">Tab index to use</param>
    /// <param name="nodes">Nodes to add</param>
    /// <remarks>
    /// Tab index provided will be <em>added</em> to the current accumulated <see cref="TabStep"/>.
    /// </remarks>
    public void AddNode(int tabIndex, IEnumerable<NodeBase> nodes) {
        suppressRecalculateLayout = true;
        foreach (var node in nodes) {
            AddNode(tabIndex, node);
        }
        suppressRecalculateLayout = false;
    }

    /// <summary>
    /// Adds a single node with the specified tab index to the list.
    /// </summary>
    /// <param name="tabIndex">Tab index to use</param>
    /// <param name="node">Node to add</param>
    /// <remarks>
    /// Tab index provided will be <em>added</em> to the current accumulated <see cref="TabStep"/>
    /// </remarks>
    public void AddNode(int tabIndex, NodeBase node) {
        nodeList.Add(new TabbedNodeEntry<NodeBase>(node, tabIndex + TabStep));

        node.AttachNode(this);
        if (!suppressRecalculateLayout) {
            RecalculateLayout();
        }
    }

    // <inheritdoc/>
    public void AddNode(NodeBase? node) {
        if (node is null) return;

        AddNode(0, node);
    }

    // <inheritdoc/>
    public void AddNode(IEnumerable<NodeBase> nodes) {
        suppressRecalculateLayout = true;
        AddNode(0, nodes);
        suppressRecalculateLayout = false;
        RecalculateLayout();
    }

    // <inheritdoc/>
    public void RemoveNode(IEnumerable<NodeBase> items) {
        suppressRecalculateLayout = true;
        foreach (var node in items) {
            RemoveNode(node);
        }
        suppressRecalculateLayout = false;
        RecalculateLayout();
    }

    // <inheritdoc/>
    public void RemoveNode(NodeBase node) {
        if (nodeList.All(entry => entry.Node != node)) return;

        nodeList.RemoveAll(entry => entry.Node == node);
        node.Dispose();

        RecalculateLayout();
    }

    // <inheritdoc/>
    public void AddDummy(float size = 0)
        => AddNode(new ResNode{ Width = size, Height = size });

    // <inheritdoc/>
    public void Clear() {
        foreach (var nodeEntry in nodeList) {
            nodeEntry.Node.Dispose();
        }

        nodeList.Clear();
        RecalculateLayout();
    }

    // <inheritdoc/>
    public void ReorderNodes(Comparison<NodeBase> comparison)
        => nodeList.Sort((left, right) => comparison(left.Node, right.Node));

    private void OnRecalculateLayout() {
        var startY = 0.0f + FirstItemSpacing;

        foreach (var (node, tab) in nodeList) {
            if (!node.IsVisible) continue;

            node.Y = startY;
            node.X = tab * TabSize;

            if (FitWidth) {
                node.Width = Width - node.X - ItemSpacing;
            }

            startY += node.Height + ItemSpacing;
        }

        if (FitContents) {
            Height = startY + ItemSpacing;
        }
    }

    private void OnRecalculateNavigation() {
        var componentNodes = nodeList.Select(nodeEntry => nodeEntry.Node).OfType<ComponentNode>().ToList();
        if (componentNodes.Count is 0) return;

        foreach (var (index, node) in componentNodes.Index()) {
            node.NavIndex = (byte) (index + NavIndex);
            node.NavLeft = NavLeft;
            node.NavRight = NavRight;

            // First Element
            if (index is 0) {
                node.NavUp = (byte) (componentNodes.Count - 1 + NavIndex);
            }
            else {
                node.NavUp = (byte) (index - 1 + NavIndex);
            }

            // Last Element
            if (index == componentNodes.Count - 1) {
                node.NavDown = (byte) NavIndex;
            }
            else {
                node.NavDown = (byte) (index + 1 + NavIndex);
            }
        }
    }

    private readonly List<TabbedNodeEntry<NodeBase>> nodeList = [];
    private bool suppressRecalculateLayout;
}
