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
public class TabbedVerticalListNode : ResNode, ILayoutListNode, IStretchWidth {

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
    public IReadOnlyList<NodeBase> Nodes => nodeList.Select(node => node.Node).ToList();

    /// <inheritdoc/>
    public bool ClipContents { get; set; }

    /// <inheritdoc/>
    public float ItemSpacing { get; set; }

    /// <inheritdoc/>
    public float FirstItemSpacing { get; set; }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public IEnumerable<T> GetNodes<T>() where T : NodeBase
        => Nodes.OfType<T>();

    /// <inheritdoc/>
    public void RecalculateLayout() {
        if (suppressRecalculateLayout) return;

        foreach (var node in Nodes) {
            if (node is ILayoutUpdatable subNode) {
                subNode.RecalculateLayout();
            }
        }

        OnRecalculateLayout();

        if (NavIndex is not 0) {
            OnRecalculateNavigation();
        }

        foreach (var node in Nodes) {
            if (node is ILayoutUpdatable subNode) {
                subNode.RecalculateLayout();
            }
        }
    }

    /// <inheritdoc/>
    public void RecalculateNavigation() {
        if (suppressRecalculateLayout) return;

        foreach (var node in Nodes) {
            if (node is INavigationRecalculable subNode) {
                subNode.RecalculateNavigation();
            }
        }

        OnRecalculateNavigation();

        foreach (var node in Nodes) {
            if (node is INavigationRecalculable subNode) {
                subNode.RecalculateNavigation();
            }
        }
    }

    /// <summary>
    /// How much space to add for each tab value.
    /// </summary>
    public float TabSize { get; set; } = 18.0f;

    /// <inheritdoc />
    public bool StretchWidth { get; set; }

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

    /// <inheritdoc/>
    public void AddNode(NodeBase? node) {
        if (node is null) return;

        AddNode(0, node);
    }

    /// <inheritdoc/>
    public void AddNode(IEnumerable<NodeBase> nodes) {
        suppressRecalculateLayout = true;
        AddNode(0, nodes);
        suppressRecalculateLayout = false;
        RecalculateLayout();
    }

    /// <inheritdoc/>
    public void RemoveNode(IEnumerable<NodeBase> items) {
        suppressRecalculateLayout = true;
        foreach (var node in items) {
            RemoveNode(node);
        }
        suppressRecalculateLayout = false;
        RecalculateLayout();
    }

    /// <inheritdoc/>
    public void RemoveNode(NodeBase node) {
        if (nodeList.All(entry => entry.Node != node)) return;

        nodeList.RemoveAll(entry => entry.Node == node);
        node.Dispose();

        RecalculateLayout();
    }

    /// <inheritdoc/>
    public void AddDummy(float size = 0)
        => AddNode(new ResNode{ Width = size, Height = size });

    /// <inheritdoc/>
    public void Clear() {
        foreach (var nodeEntry in nodeList) {
            nodeEntry.Node.Dispose();
        }

        nodeList.Clear();
        RecalculateLayout();
    }

    /// <inheritdoc/>
    public void ReorderNodes(Comparison<NodeBase> comparison)
        => nodeList.Sort((left, right) => comparison(left.Node, right.Node));

    private void OnRecalculateLayout() {
        var startY = 0.0f + FirstItemSpacing;

        foreach (var (node, tab) in nodeList) {
            if (!node.IsVisible) continue;

            node.Y = startY;
            node.X = tab * TabSize;

            if (StretchWidth) {
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
