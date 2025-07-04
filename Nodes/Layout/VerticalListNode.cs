using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.System;
using Newtonsoft.Json;

namespace KamiToolKit.Nodes;

public class VerticalListNode : VerticalListNode<NodeBase>;

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
	
	// Resizes this node to fit all elements
	public bool FitContents { get; set; }

	public void RecalculateLayout() {
		var startY = Alignment switch {
			VerticalListAnchor.Top => 0.0f + FirstItemSpacing,
			VerticalListAnchor.Bottom => Height - FirstItemSpacing,
			_ => 0.0f,
		};

		foreach (var node in nodeList) {
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
			Height = startY + FirstItemSpacing;
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

	public void AddDummy(T dummyNode, float height) {
		dummyNode.Width = Width;
		dummyNode.Height = height;
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
}