using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace KamiToolKit.Nodes;

public class TreeListNode : SimpleComponentNode {

    private readonly SimpleComponentNode childContainer;

    private readonly List<TreeListCategoryNode> children = [];

    public ReadOnlyCollection<TreeListCategoryNode> CategoryNodes => children.AsReadOnly();

    public TreeListNode() {
        childContainer = new SimpleComponentNode();
        childContainer.AttachNode(this);
    }

    public float CategoryVerticalSpacing { get; set; } = 4.0f;

    public Action<float>? OnLayoutUpdate { get; set; }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        childContainer.Width = Width;
    }

    public void AddCategoryNode(TreeListCategoryNode node) {
        RefreshLayout();

        children.Add(node);

        node.NodeId = (uint)children.Count + 1;
        node.Width = childContainer.Width;
        node.Y = childContainer.Height;
        node.AttachNode(childContainer);
        node.ParentTreeListNode = this;

        childContainer.Height += node.Height + CategoryVerticalSpacing;
    }

    public void RefreshLayout() {
        childContainer.Height = 0.0f;

        foreach (var child in children) {
            if (!child.IsVisible) continue;

            child.Y = childContainer.Height;
            childContainer.Height += child.Height + CategoryVerticalSpacing;
            child.UpdateChildrenNodeId();
        }

        OnLayoutUpdate?.Invoke(childContainer.Height);
    }
}
