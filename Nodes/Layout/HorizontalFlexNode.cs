using System.Collections.Generic;
using System.Linq;
using KamiToolKit.Classes;
using KamiToolKit.System;

namespace KamiToolKit.Nodes;

public class HorizontalFlexNode<T> : SimpleComponentNode where T : NodeBase {

	private readonly List<T> nodeList = [];

	public FlexFlags AlignmentFlags { get; set; } = FlexFlags.FitContentHeight;
	
	public float FitPadding { get; set; } = 4.0f;

	public void RecalculateLayout() {
		var step = Width / nodeList.Count;

		if (nodeList.Count != 0 && AlignmentFlags.HasFlag(FlexFlags.FitContentHeight)) {
			Height = nodeList.Max(node => node.Height);
		}
		
		foreach (var index in Enumerable.Range(0, nodeList.Count)) {

			if (AlignmentFlags.HasFlag(FlexFlags.CenterHorizontally)) {
				nodeList[index].X = step * index + step / 2.0f - nodeList[index].Width / 2.0f;
			}
			else {
				nodeList[index].X = step * index;
			}

			if (AlignmentFlags.HasFlag(FlexFlags.FitHeight)) {
				nodeList[index].Height = Height;
			}
			
			if (AlignmentFlags.HasFlag(FlexFlags.CenterVertically)) {
				nodeList[index].Y = Height / 2 - nodeList[index].Height / 2;
			}

			if (AlignmentFlags.HasFlag(FlexFlags.FitWidth)) {
				nodeList[index].Width = step - FitPadding;
			}
		}
	}
	
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