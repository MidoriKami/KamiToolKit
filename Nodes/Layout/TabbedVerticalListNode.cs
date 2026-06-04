using System.Collections.Generic;
using System.Linq;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

/// <summary>
/// A vertical list node with features for adding nodes with specified tab values.
/// </summary>
public class TabbedVerticalListNode : LayoutListNode {

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
    public void AddTab(int tabAmount) {
        TabStep += tabAmount;
    }

    /// <summary>
    /// Adds <see cref="tabAmount"/> to <see cref="TabStep"/> causing all
    /// following nodes tab to be decreased by the specified amount.
    /// </summary>
    /// <param name="tabAmount">Tab value to decrease.</param>
    public void SubtractTab(int tabAmount) {
        TabStep -= tabAmount;
    }

    // <inheritdoc/>
    public override void AddNode(NodeBase? node) {
        base.AddNode(node);
        if (node is null) return;

        AddNode(0, node);
    }

    // <inheritdoc/>
    public override void AddNode(IEnumerable<NodeBase> nodes) {
        nodes = nodes.ToList();
        base.AddNode(nodes);

        suppressRecalculate = true;
        AddNode(0, nodes);
        suppressRecalculate = false;
        RecalculateLayout();
    }

    /// <summary>
    /// Adds several nodes with the specified tab index to the list.
    /// </summary>
    /// <param name="tabIndex">Tab index to use</param>
    /// <param name="nodes">Nodes to add</param>
    /// <remarks>
    /// Tab index provided will be <em>added</em> to the current accumulated <see cref="TabStep"/>.
    /// </remarks>
    public void AddNode(int tabIndex, IEnumerable<NodeBase> nodes) {
        suppressRecalculate = true;
        foreach (var node in nodes) {
            AddNode(tabIndex, node);
        }
        suppressRecalculate = false;
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
        node.NodeId = (uint)nodeList.Count + 1;

        if (!suppressRecalculate) {
            RecalculateLayout();
        }
    }

    // <inheritdoc/>
    public override void Clear() {
        foreach (var nodeEntry in nodeList) {
            nodeEntry.Node.Dispose();
        }

        nodeList.Clear();
        RecalculateLayout();
    }

    protected override void OnRecalculateLayout() {
        var startY = 0.0f + FirstItemSpacing;

        foreach (var (node, tab) in nodeList) {
            if (!node.IsVisible) continue;

            node.Y = startY;
            node.X = tab * TabSize;

            if (FitWidth) {
                node.Width = Width - node.X - ItemSpacing;

                // Also update layout of any contained nodes
                if (node is LayoutListNode layoutNode) {
                    layoutNode.RecalculateLayout();
                }
            }

            startY += node.Height + ItemSpacing;
        }

        if (FitContents) {
            Height = startY + ItemSpacing;
        }
    }

    protected override void OnRecalculateNavigation() {
        // todo: this !
    }

    private readonly List<TabbedNodeEntry<NodeBase>> nodeList = [];
    private bool suppressRecalculate = false;
}
