using System.Collections.Generic;
using System.Linq;

namespace KamiToolKit.Nodes;

/// <summary>
/// This is a combination of a ScrollingAreaNode and a TreeListNode for easy layout
/// </summary>
public class ScrollingTreeNode : SimpleComponentNode {

    private readonly ScrollingAreaNode<TreeListNode> listNode;
    
    public ScrollingTreeNode() {
        listNode = new ScrollingAreaNode<TreeListNode> {
            ContentHeight = 100.0f,
        };
        listNode.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        listNode.Size = Size;
        RecalculateLayout();
    }
    
    public float CategoryVerticalSpacing {
        get => listNode.ContentNode.CategoryVerticalSpacing;
        set => listNode.ContentNode.CategoryVerticalSpacing = value;
    }
    
    public bool AutoHideScrollBar {
        get => listNode.AutoHideScrollBar;
        set => listNode.AutoHideScrollBar = value;
    }

    public int ScrollSpeed {
        get => listNode.ScrollSpeed;
        set => listNode.ScrollSpeed = value;
    }
        
    public IReadOnlyList<TreeListCategoryNode> CategoryNodes => listNode.ContentNode.CategoryNodes;
    
    public void RecalculateLayout() {
        listNode.ContentNode.RefreshLayout();
        listNode.ContentHeight = CategoryNodes.Sum(node => node.IsVisible ? node.Height + CategoryVerticalSpacing : 0.0f);
    }

    public void AddCategoryNode(TreeListCategoryNode node)  => listNode.ContentNode.AddCategoryNode(node);

    public TreeListNode TreeListNode => listNode.ContentNode;
}
