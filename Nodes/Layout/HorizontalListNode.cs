using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.System;
using Newtonsoft.Json;

namespace KamiToolKit.Nodes;

public class HorizontalListNode : HorizontalListNode<NodeBase>;

[JsonObject(MemberSerialization.OptIn)]
public class HorizontalListNode<T> : SimpleComponentNode where T : NodeBase {
	
	private readonly List<T> nodeList = [];
	
	[JsonProperty] public HorizontalListAnchor Alignment {
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
	
	[JsonProperty] public float ItemHorizontalSpacing { get; set; }
	
	[JsonProperty] public float FirstItemSpacing { get; set; }

	
	public void RecalculateLayout() {
		var startX = Alignment switch {
			HorizontalListAnchor.Left => 0.0f + FirstItemSpacing,
			HorizontalListAnchor.Right => Width - FirstItemSpacing,
			_ => 0.0f,
		};

		foreach (var node in nodeList) {
			if (!node.IsVisible) continue;
			
			node.X = startX;
			AdjustNode(node);

			switch (Alignment) {
				case HorizontalListAnchor.Left:
					startX += node.Width + ItemHorizontalSpacing;
					break;
				
				case HorizontalListAnchor.Right:
					startX -= node.Width + ItemHorizontalSpacing;
					break;
			}
		}
	}

	protected virtual void AdjustNode(T node) { }

	public void AddNode(params T[] items) {
		foreach (var node in items) {
			AddNode(node);
		}
	}
	
	public void AddNode(T node) {
		nodeList.Add(node);
		
		node.AttachNode(this);
		node.NodeId = (uint) nodeList.Count + 1;
		
		RecalculateLayout();
	}

	public void AddDummy(T dummyNode, float width) {
		dummyNode.Width = width;
		dummyNode.Height = Height;
		dummyNode.IsVisible = true;
		AddNode(dummyNode);
	}

	public void RemoveNode(params T[] items) {
		foreach (var node in items) {
			RemoveNode(node);
		}
	}

	public void RemoveNode(T node) {
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

	public override float Width {
		get => base.Width;
		set {
			base.Width = value;
			RecalculateLayout();
		}
	}
}