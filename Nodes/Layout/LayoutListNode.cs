using System.Collections.Generic;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.System;
using Newtonsoft.Json;

namespace KamiToolKit.Nodes;

[JsonObject(MemberSerialization.OptIn)]
public abstract class LayoutListNode : SimpleComponentNode {
	protected readonly List<NodeBase> NodeList = [];

	public abstract void RecalculateLayout();

	protected virtual void AdjustNode(NodeBase node) { }

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

	[JsonProperty] public float ItemSpacing { get; set; }

	[JsonProperty] public float FirstItemSpacing { get; set; }

	public void AddNode(params NodeBase[] items) {
		foreach (var node in items) {
			AddNode(node);
		}
	}

	public virtual void AddNode(NodeBase node) {
		NodeList.Add(node);
		
		node.AttachNode(this);
		node.NodeId = (uint) NodeList.Count + 1;
		
		RecalculateLayout();
	}
	
	public void RemoveNode(params NodeBase[] items) {
		foreach (var node in items) {
			RemoveNode(node);
		}
	}
	
	public virtual void RemoveNode(NodeBase node) {
		node.DetachNode();
		NodeList.Remove(node);
		RecalculateLayout();
	}
	
	public void AddDummy(float size = 0.0f) {
		var dummyNode = new ResNode {
			Size = new Vector2(size, size),
			IsVisible = true,
		};

		AddNode(dummyNode);
	}

	public virtual void Clear() {
		foreach (var node in NodeList) {
			node.DetachNode();
		}
		
		NodeList.Clear();
		RecalculateLayout();
	}
}