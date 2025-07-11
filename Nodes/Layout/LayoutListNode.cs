using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.System;
using Newtonsoft.Json;

namespace KamiToolKit.Nodes;

public abstract class LayoutListNode : LayoutListNode<NodeBase>;

[JsonObject(MemberSerialization.OptIn)]
public abstract class LayoutListNode<T> : SimpleComponentNode where T : NodeBase {
	protected readonly List<T> NodeList = [];

	public abstract void RecalculateLayout();

	protected virtual void AdjustNode(T node) { }
	
		
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

	public void AddNode(params T[] items) {
		foreach (var node in items) {
			AddNode(node);
		}
	}

	public void AddNode(T node) {
		NodeList.Add(node);
		
		node.AttachNode(this);
		node.NodeId = (uint) NodeList.Count + 1;
		
		RecalculateLayout();
	}
	
	public void RemoveNode(params T[] items) {
		foreach (var node in items) {
			RemoveNode(node);
		}
	}
	
	public void RemoveNode(T node) {
		node.DetachNode();
		NodeList.Remove(node);
		RecalculateLayout();
	}
	
	public void Clear() {
		foreach (var node in NodeList) {
			node.DetachNode();
		}
		
		NodeList.Clear();
		RecalculateLayout();
	}
}