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

    /// <summary>
    /// Gets or sets the base nav index used for this node.
    /// </summary>
    public int NavIndex { get; set; }

    /// <summary>
    /// Gets or sets the left nav index for this list.
    /// </summary>
    public int NavLeft { get; set; }

    /// <summary>
    /// Gets or sets the right nav index for this list.
    /// </summary>
    public int NavRight { get; set; }

    /// <summary>
    /// Gets a readonly list of the nodes managed by this node.
    /// </summary>
    public IReadOnlyList<NodeBase> Nodes => nodeList.Select(node => node.Node).ToList();

    /// <summary>
    /// Gets or sets whether the contents of this list should be clipped out.
    /// </summary>
    public bool ClipListContents { get; set; }

    /// <summary>
    /// Gets or sets the spacing used between items, excluding the first item.
    /// </summary>
    public float ItemSpacing { get; set; }

    /// <summary>
    /// Gets or sets the first items spacing from the start of this node.
    /// </summary>
    public float FirstItemSpacing { get; set; }

    /// <summary>
    /// Sets the initial nodes used by this layout node.
    /// </summary>
    public ICollection<NodeBase> InitialNodes {
        set => AddNode(value);
    }

    /// <summary>
    /// Special implementation for allowing initial tabbed node entries.
    /// </summary>
    public ICollection<TabbedListEntry> InitialTabbedNodes {
        set {
            foreach (var entry in value) {
                if (entry.Node is { } singleNode) {
                    AddNode(entry.TabIndex, singleNode);
                }

                if (entry.Nodes.Count is not 0) {
                    foreach (var node in entry.Nodes) {
                        AddNode(entry.TabIndex, node);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Gets the contained nodes of the provided type.
    /// </summary>
    public IEnumerable<T> GetNodes<T>() where T : NodeBase
        => Nodes.OfType<T>();

    /// <summary>
    /// Trigger recalculating layout for this node and any sub contained layout list nodes.
    /// </summary>
    public void RecalculateLayout() {
        if (suppressRecalculateLayout) return;

        foreach (var node in Nodes) {
            if (node is ILayoutListNode subNode) {
                subNode.RecalculateLayout();
            }
        }

        OnRecalculateLayout();

        if (NavIndex is not 0) {
            OnRecalculateNavigation();
        }

        foreach (var node in Nodes) {
            if (node is ILayoutListNode subNode) {
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
    /// Adds tabAmount to TabStep causing all
    /// following nodes tab to be increased by the specified amount.
    /// </summary>
    public void AddTab(int tabAmount)
        => TabStep += tabAmount;

    /// <summary>
    /// Adds tabAmount to TabStep causing all
    /// following nodes tab to be decreased by the specified amount.
    /// </summary>
    public void SubtractTab(int tabAmount)
        => TabStep -= tabAmount;

    /// <summary>
    /// Adds several nodes with the specified tab index to the list.
    /// </summary>
    /// <remarks>
    /// Tab index provided will be <em>added</em> to the current accumulated TabStep.
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

    /// <summary>
    /// Add a node to this layout node.
    /// </summary>
    public void AddNode(NodeBase? node) {
        if (node is null) return;

        AddNode(0, node);
    }

    /// <summary>
    /// Add a node to this layout node.
    /// </summary>
    public void AddNode(IEnumerable<NodeBase> nodes) {
        suppressRecalculateLayout = true;
        AddNode(0, nodes);
        suppressRecalculateLayout = false;
        RecalculateLayout();
    }

    /// <summary>
    /// Removes a node from this layout node.
    /// </summary>
    public void RemoveNode(IEnumerable<NodeBase> items) {
        suppressRecalculateLayout = true;
        foreach (var node in items) {
            RemoveNode(node);
        }
        suppressRecalculateLayout = false;
        RecalculateLayout();
    }

    /// <summary>
    /// Removes a node from this layout node.
    /// </summary>
    public void RemoveNode(NodeBase node) {
        if (nodeList.All(entry => entry.Node != node)) return;

        nodeList.RemoveAll(entry => entry.Node == node);
        node.Dispose();

        RecalculateLayout();
    }

    /// <summary>
    /// Adds a dummy node used for spacing things out even more.
    /// </summary>
    public void AddDummy(float size = 0)
        => AddNode(new ResNode{ Width = size, Height = size });

    /// <summary>
    /// Removes and disposes all contained nodes.
    /// </summary>
    public void Clear() {
        foreach (var nodeEntry in nodeList) {
            nodeEntry.Node.Dispose();
        }

        nodeList.Clear();
        RecalculateLayout();
    }

    /// <summary>
    /// Sorts the contained node list to reorder the nodes into the specified order.
    /// </summary>
    /// <remarks>
    /// This sorting will not apply to any nodes added afterwards.
    /// </remarks>
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
