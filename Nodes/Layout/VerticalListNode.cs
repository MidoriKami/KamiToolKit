using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.System;
using Newtonsoft.Json;

namespace KamiToolKit.Nodes;

public class VerticalListNode : VerticalListNode<NodeBase>;

[JsonObject(MemberSerialization.OptIn)]
public class VerticalListNode<T> : LayoutListNode<T> where T : NodeBase {
	
	[JsonProperty] public VerticalListAnchor Alignment {
		get; set {
			field = value;
			RecalculateLayout();
		}
	}
	
	[JsonProperty] public bool ClipListContents {
		get => NodeFlags.HasFlag(NodeFlags.Clip);
		set {
			if (value) {
				AddFlags(NodeFlags.Clip);
			}
			else {
				RemoveFlags(NodeFlags.Clip);
			}
		}
	}
	
	[JsonProperty] public float ItemVerticalSpacing { get; set; }
	
	[JsonProperty] public float FirstItemSpacing { get; set; }
	
	// Resizes this node to fit all elements
	public bool FitContents { get; set; }

	public override void RecalculateLayout() {
		var startY = Alignment switch {
			VerticalListAnchor.Top => 0.0f + FirstItemSpacing,
			VerticalListAnchor.Bottom => Height,
			_ => 0.0f,
		};

		foreach (var node in NodeList) {
			if (!node.IsVisible) continue;

			if (Alignment is VerticalListAnchor.Bottom) {
				startY -= node.Height + ItemVerticalSpacing;
			}

			node.Y = startY;
			AdjustNode(node);

			if (Alignment is VerticalListAnchor.Top) {
				startY += node.Height + ItemVerticalSpacing;
			}
		}

		if (FitContents) {
			Height = NodeList.Sum(node => node.IsVisible ? node.Height + ItemVerticalSpacing : 0.0f) + FirstItemSpacing;
		}
	}

	public void AddDummy(T dummyNode, float height) {
		dummyNode.Width = Width;
		dummyNode.Height = height;
		dummyNode.IsVisible = true;
		AddNode(dummyNode);
	}
}