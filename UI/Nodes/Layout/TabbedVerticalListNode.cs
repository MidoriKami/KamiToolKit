using System;
using System.Collections.Generic;
using KamiToolKit.Core.Interfaces.Nodes;
using KamiToolKit.Core.Layout;
using KamiToolKit.Core.Primitives;

namespace KamiToolKit.UI.Nodes.Layout;

/// <summary>
/// A specialized vertical list node with features for adding nodes with specified tab values.
/// </summary>
[Obsolete("Todo: add a NodeBuilder for this.")] // todo: marked todo in-case this todo isn't an obvious enough todo
public class TabbedVerticalListNode : LayoutNode, IStretchWidth {

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
                AddNodeInternal(node, tabIndex + TabStep);
            }
        }
        UpdateLayout();
    }

    /// <summary>
    /// Adds a single node with the specified tab index to the list.
    /// </summary>
    /// <remarks>
    /// Tab index provided will be <em>added</em> to the current accumulated <see cref="TabStep"/>
    /// </remarks>
    public void AddNode(int tabIndex, NodeBase node) {
        AddNodeInternal(node, tabIndex + TabStep);
        UpdateLayout();
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
    }

    /// <inheritdoc/>
    protected override void AddNodeInternal(NodeBase node) {
        AddNodeInternal(node, TabStep);
    }

    private void AddNodeInternal(NodeBase node, int tabIndex) {
        tabIndices.Add(node, tabIndex);
        base.AddNodeInternal(node);
    }

    /// <inheritdoc/>
    protected override bool RemoveNodeInternal(NodeBase node) {
        if (!tabIndices.Remove(node)) return false;
        return base.RemoveNodeInternal(node);
    }

    /// <inheritdoc/>
    protected override void ClearInternal() {
        tabIndices.Clear();
        base.ClearInternal();
    }


    /// <inheritdoc/>
    protected override void OnUpdateSizes() {
        foreach (var node in NodeList) {
            if (!node.IsVisible) continue;

            if (StretchWidth) {
                node.Width = Width - tabIndices[node] * TabSize - ItemSpacing;
            }
        }

        if (FitContents) {
            Height = CalculateFitContentsHeight();
        }
    }

    /// <inheritdoc/>
    protected override void OnUpdateLayout() {
        var startY = 0.0f + FirstItemSpacing;

        foreach (var node in NodeList) {
            if (!node.IsVisible) continue;

            node.Y = startY;
            node.X = tabIndices[node] * TabSize;
            startY += node.Height + ItemSpacing;
        }
    }

    /// <inheritdoc />
    protected override void OnNavigationUpdate(NavigationContext context) {
        // todo: this as well
    }

    /// <summary>
    /// Calculates the height needed to fit all visible child nodes when FitContents is enabled.
    /// </summary>
    private float CalculateFitContentsHeight() {
        var height = FirstItemSpacing;

        foreach (var node in NodeList) {
            if (!node.IsVisible) continue;

            height += node.Height + ItemSpacing;
        }

        return height;
    }

    private readonly Dictionary<NodeBase, int> tabIndices = [];
}
