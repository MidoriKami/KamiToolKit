using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.System;
using Newtonsoft.Json;

namespace KamiToolKit.Nodes;

[JsonObject(MemberSerialization.OptIn)]
public class VerticalListNode<T> : SimpleComponentNode where T : NodeBase {
	
	private readonly List<T> nodeList = [];
	
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
	
	public void RecalculateLayout() {
		var startY = Alignment switch {
			VerticalListAnchor.Top => 0.0f + FirstItemSpacing,
			VerticalListAnchor.Bottom => Height - FirstItemSpacing,
			_ => 0.0f,
		};

		foreach (var node in nodeList) {
			node.Y = startY;
			node.X = 0.0f;

			switch (Alignment) {
				case VerticalListAnchor.Top:
					startY += node.Height + ItemVerticalSpacing;
					break;
				
				case VerticalListAnchor.Bottom:
					startY -= node.Height + ItemVerticalSpacing;
					break;
			}
		}
	}

	public void Add(params T[] items) {
		foreach (var node in items) {
			Add(node);
		}
	}
	
	public void Add(T node) {
		nodeList.Add(node);
		
		node.AttachNode(this);
		node.NodeId = (uint) nodeList.Count + 1;
		
		RecalculateLayout();
	}

	public void AddDummy(T dummyNode, float height) {
		dummyNode.Width = Width;
		dummyNode.Height = height;
		dummyNode.IsVisible = true;
		Add(dummyNode);
	}
	
	public void Remove(params T[] items) {
		foreach (var node in items) {
			Remove(node);
		}
	}

	public void Remove(T node) {
		node.DetachNode();
		nodeList.Remove(node);
		RecalculateLayout();
	}

	public void Clear() {
		foreach (var node in nodeList) {
			node.DetachNode();
		}
		
		nodeList.Clear();
		RecalculateLayout();
	}

	public override float Height {
		get => base.Height;
		set {
			base.Height = value;
			RecalculateLayout();
		}
	}
}