using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace KamiToolKit.Nodes;

/// <summary>
/// Custom implementation of a tree list node. Kinda ghetto.
/// </summary>
/// <remarks>
/// Needs reworking, it sorta works but is buggy at the moment.
/// </remarks>
[Experimental("KamiToolKit_Experimental")]
public class TreeListNode : ResNode {

    /// <summary>
    /// Gets or sets the spacing between category nodes.
    /// </summary>
    public float CategoryVerticalSpacing { get; set; } = 4.0f;

    /// <summary>
    /// Action that is called on a layout update.
    /// </summary>
    public Action<float>? OnLayoutUpdate { get; set; }

    /// <summary>
    /// Gets a readonly list of contained category nodes.
    /// </summary>
    public ReadOnlyCollection<TreeListCategoryNode> CategoryNodes => children.AsReadOnly();

    /// <summary>
    /// Adds a category node to the tree.
    /// </summary>
    public void AddCategoryNode(TreeListCategoryNode node) {
        RefreshLayout();

        children.Add(node);

        node.Width = childContainer.Width;
        node.Y = childContainer.Height;
        node.AttachNode(childContainer);
        node.ParentTreeListNode = this;

        childContainer.Height += node.Height + CategoryVerticalSpacing;
    }

    /// <summary>
    /// Recalculates the tree's layout.
    /// </summary>
    public void RefreshLayout() {
        childContainer.Height = 0.0f;

        foreach (var child in children) {
            if (!child.IsVisible) continue;

            child.Y = childContainer.Height;
            childContainer.Height += child.Height + CategoryVerticalSpacing;
        }

        OnLayoutUpdate?.Invoke(childContainer.Height);
    }

    /// <summary>
    /// Constructs a new <see cref="TreeListNode"/>
    /// </summary>
    public TreeListNode() {
        childContainer = new ResNode();
        childContainer.AttachNode(this);
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();
        childContainer.Width = Width;
    }

    private readonly ResNode childContainer;
    private readonly List<TreeListCategoryNode> children = [];
}
