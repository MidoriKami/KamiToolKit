using System.Collections.Generic;
using KamiToolKit.System;

namespace KamiToolKit.Nodes;

public abstract class LayoutListNode<T> : SimpleComponentNode where T : NodeBase {
	protected readonly List<T> NodeList = [];

	public abstract void RecalculateLayout();

	protected virtual void AdjustNode(T node) { }

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