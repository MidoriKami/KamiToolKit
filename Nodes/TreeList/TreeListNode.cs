using System;
using System.Collections.Generic;

namespace KamiToolKit.Nodes;

public class TreeListNode : ResNode {

    private readonly ResNode childContainer;

    private List<TreeListCategoryNode> children = [];

    public TreeListNode() {
        childContainer = new ResNode {
            IsVisible = true,
        };

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
        }

        OnLayoutUpdate?.Invoke(childContainer.Height);
    }
}
