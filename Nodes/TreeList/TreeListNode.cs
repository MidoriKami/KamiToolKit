using System.Collections.Generic;

namespace KamiToolKit.Nodes;

public class TreeListNode : ResNode {

	private List<TreeListCategoryNode> children = [];
	
	private readonly ResNode childContainer;

	public float CategoryVerticalSpacing { get; set; } = 4.0f;

	public TreeListNode() {
		childContainer = new ResNode {
			IsVisible = true,
		};
		
		childContainer.AttachNode(this);
	}

	public override float Width {
		get => base.Width;
		set {
			base.Width = value;
			childContainer.Width = value;
		}
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
			child.Y = childContainer.Height;
			childContainer.Height += child.Height + CategoryVerticalSpacing;
		}
	}
}