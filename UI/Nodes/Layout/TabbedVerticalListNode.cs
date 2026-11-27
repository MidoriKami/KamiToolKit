using System;
using System.Collections.Generic;
using System.Linq;
using KamiToolKit.Core.Interfaces.Nodes;
using KamiToolKit.Core.Layout;
using KamiToolKit.Core.Primitives.NodeBase;

namespace KamiToolKit.UI.Nodes.Layout;

/// <summary>
/// A specialized vertical list node with features for adding nodes with specified tab values.
/// </summary>
public class TabbedVerticalListNode : LayoutListNode, IStretchWidth {

    /// <inheritdoc/>
    public override IReadOnlyList<NodeBase> Nodes => nodeList.Select(node => node.Node).ToList();

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
    protected override void DoUpdateLayout() {
        if (nodeList.Count > 0) {
            OnUpdateSizes();
            OnUpdateLayout();
        }

        if (nodeList.Count > 0) {
            OnUpdateSizes();
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
        using (SuppressLayoutRecalculation()) {
            foreach (var node in nodes) {
                AddNode(tabIndex, node);
            }
        }
        UpdateLayout();
        UpdateNavigation();
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
        SetNavigationParentForChild(node, this);
        UpdateLayout();
        UpdateNavigation();
    }

    /// <inheritdoc/>
    public override void AddNode(NodeBase? node) {
        if (node is null) return;
        AddNode(0, node);
    }

    /// <inheritdoc/>
    public override void AddNode(IEnumerable<NodeBase> nodes) {
        using (SuppressLayoutRecalculation()) {
            AddNode(0, nodes);
        }
        UpdateLayout();
        UpdateNavigation();
    }

    /// <inheritdoc/>
    protected override void AddNodeInternal(NodeBase node) {
        nodeList.Add(new TabbedNodeEntry<NodeBase>(node, TabStep));
        node.AttachNode(this);
        SetNavigationParentForChild(node, this);
    }

    /// <inheritdoc/>
    protected override bool RemoveNodeInternal(NodeBase node) {
        if (nodeList.All(entry => entry.Node != node)) return false;
        nodeList.RemoveAll(entry => entry.Node == node);
        SetNavigationParentForChild(node, null);
        return true;
    }

    /// <inheritdoc/>
    protected override void ClearInternal() {
        foreach (var nodeEntry in nodeList) {
            SetNavigationParentForChild(nodeEntry.Node, null);
            nodeEntry.Node.Dispose();
        }
        nodeList.Clear();
    }

    /// <inheritdoc/>
    protected override void ReorderInternal(Comparison<NodeBase> comparison) {
        nodeList.Sort((left, right) => comparison(left.Node, right.Node));
    }

    /// <inheritdoc/>
    protected override void OnUpdateSizes() {
        foreach (var (node, tab) in nodeList) {
            if (!node.IsVisible) continue;

            if (StretchWidth) {
                node.Width = Width - tab * TabSize - ItemSpacing;
            }
        }

        if (FitContents) {
            Height = CalculateFitContentsHeight();
        }
    }

    /// <inheritdoc/>
    protected override void OnUpdateLayout() {
        var startY = 0.0f + FirstItemSpacing;

        foreach (var (node, tab) in nodeList) {
            if (!node.IsVisible) continue;

            node.Y = startY;
            node.X = tab * TabSize;
            startY += node.Height + ItemSpacing;
        }
    }

    /// <inheritdoc/>
    protected override void OnUpdateControllerNav() {
        var navigationGroups = GetNavigationChildGroups();
        ConnectVerticalNavigationGroups(navigationGroups);

        var boundary = CreateNavigationBoundary(navigationGroups);
        SetNavigationBoundary(boundary);
        LinkNavigationBoundaryToIndex(boundary.Top, ControllerNavigationDirection.Up, NavUp);
        LinkNavigationBoundaryToIndex(boundary.Bottom, ControllerNavigationDirection.Down, NavDown);

        foreach (var group in navigationGroups) {
            LinkNavigationBoundaryToIndex(group.Boundary.Left, ControllerNavigationDirection.Left, NavLeft);
            LinkNavigationBoundaryToIndex(group.Boundary.Right, ControllerNavigationDirection.Right, NavRight);
        }
    }

    /// <summary>
    /// Calculates the height needed to fit all visible child nodes when FitContents is enabled.
    /// </summary>
    private float CalculateFitContentsHeight() {
        var height = FirstItemSpacing;

        foreach (var (node, _) in nodeList) {
            if (!node.IsVisible) continue;

            height += node.Height + ItemSpacing;
        }

        return height;
    }

    private readonly List<TabbedNodeEntry<NodeBase>> nodeList = [];
}
