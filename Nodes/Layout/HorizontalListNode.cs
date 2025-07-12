using KamiToolKit.Classes;
using KamiToolKit.System;
using Newtonsoft.Json;

namespace KamiToolKit.Nodes;

public class HorizontalListNode : HorizontalListNode<NodeBase>;

[JsonObject(MemberSerialization.OptIn)]
public abstract class HorizontalListNode<T> : LayoutListNode<T> where T : NodeBase {
	
	[JsonProperty] public HorizontalListAnchor Alignment {
		get; set {
			field = value;
			RecalculateLayout();
		}
	}
	
	public override void RecalculateLayout() {
		var startX = Alignment switch {
			HorizontalListAnchor.Left => 0.0f + FirstItemSpacing,
			HorizontalListAnchor.Right => Width - FirstItemSpacing,
			_ => 0.0f,
		};

		foreach (var node in NodeList) {
			if (!node.IsVisible) continue;

			if (Alignment is HorizontalListAnchor.Right) {
				startX -= node.Width + ItemSpacing;
			}

			node.X = startX;
			AdjustNode(node);

			if (Alignment is HorizontalListAnchor.Left) {
				startX += node.Width + ItemSpacing;
			}
		}
	}

	public void AddDummy(T dummyNode, float width) {
		dummyNode.Width = width;
		dummyNode.Height = Height;
		dummyNode.IsVisible = true;
		AddNode(dummyNode);
	}

	public override float Width {
		get => base.Width;
		set {
			base.Width = value;
			RecalculateLayout();
		}
	}
}