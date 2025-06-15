using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Nodes;

public unsafe class TreeListNode : ResNode {

	private List<TreeListCategoryNode> children = [];
	
	private readonly ResNode childContainer;

	public float CategoryVerticalSpacing { get; set; } = 4.0f;

	private const float ScrollBarWidth = 8.0f;
	
	public TreeListNode() {
		childContainer = new ResNode {
			IsVisible = true,
		};
		
		childContainer.AttachNode(this);
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			childContainer.Dispose();
			
			foreach (var child in children) {
				child.Dispose();
			}
			
			base.Dispose(disposing);
		}
	}

	public override void EnableEvents(AtkUnitBase* addon) {
		base.EnableEvents(addon);
		
		childContainer.EnableEvents(addon);
		
		foreach (var child in children) {
			child.EnableEvents(addon);
		}
	}

	public override void DisableEvents() {
		base.DisableEvents();
		
		childContainer.DisableEvents();
		
		foreach (var child in children) {
			child.DisableEvents();
		}
	}

	public override float Width {
		get => base.Width;
		set {
			base.Width = value;
			childContainer.Width = value - ScrollBarWidth;
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