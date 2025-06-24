using System.Collections.Generic;
using System.Linq;
using KamiToolKit.Classes;
using KamiToolKit.System;
using Newtonsoft.Json;

namespace KamiToolKit.Nodes;

public class TabbedVerticalListNode : TabbedVerticalListNode<NodeBase>;

[JsonObject(MemberSerialization.OptIn)]
public class TabbedVerticalListNode<T>  : SimpleComponentNode where T : NodeBase {

	private List<TabbedNodeEntry<T>> nodeList = [];

	[JsonProperty] public float TabSize { get; set; } = 18.0f;
	
	[JsonProperty] public float ItemVerticalSpacing { get; set; }

	private int TabStep { get; set; }

	// Adds tab amount to any following nodes being added
	public void AddTab(int tabAmount) {
		TabStep += tabAmount;
	}

	// Removes tab amount from any following nodes being added
	public void SubtractTab(int tabAmount) {
		TabStep -= tabAmount;
	}

	public void AddNode(params T[] nodes) {
		AddNode(0, nodes);
	}
	
	public void AddNode(int tabIndex, params T[] nodes) {
		foreach (var node in nodes) {
			AddNode(tabIndex, node);
		}
	}

	public void AddNode(int tabIndex, T node) {
		nodeList.Add(new TabbedNodeEntry<T>(node, tabIndex + TabStep));
		
		node.AttachNode(this);
		node.NodeId = (uint) nodeList.Count + 1;
		
		RecalculateLayout();
	}

	public void RemoveNode(params T[] nodes) {
		foreach (var node in nodes) {
			RemoveNode(node);
		}
	}

	public void RemoveNode(T node) {
		var target = nodeList.FirstOrDefault(item => item.Node == node);
		if (target is null) return;
		
		target.Node.DetachNode();
		nodeList.Remove(target);
		RecalculateLayout();
	}

	public void Clea() {
		foreach (var nodeEntry in nodeList) {
			nodeEntry.Node.DetachNode();
		}
		
		nodeList.Clear();
		RecalculateLayout();
	}

	public void RecalculateLayout() {
		var startY = 0.0f;

		foreach (var (node, tab) in nodeList) {
			if (!node.IsVisible) continue;
			
			node.Y = startY;
			node.X = tab * TabSize;
			startY += node.Height + ItemVerticalSpacing;
		}
		
		Height = startY + ItemVerticalSpacing;
	}
}