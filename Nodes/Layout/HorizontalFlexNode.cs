using System.Collections.Generic;
using System.Linq;
using KamiToolKit.System;

namespace KamiToolKit.Nodes;

public class HorizontalFlexNode<T> : SimpleComponentNode where T : NodeBase {

	private readonly List<T> nodeList = [];
	
	public bool FitHeight { get; set; }

	public void RecalculateLayout() {
		var step = Width / nodeList.Count;

		if (nodeList.Count != 0) {
			Height = nodeList.Max(node => node.Height);
		}
		
		foreach (var index in Enumerable.Range(0, nodeList.Count)) {
			nodeList[index].X = step * index;

			if (FitHeight) {
				nodeList[index].Height = Height;
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

	public void AddDummy(T dummyNode, float width) {
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

	public override float Width {
		get => base.Width;
		set {
			base.Width = value;
			RecalculateLayout();
		}
	}
}