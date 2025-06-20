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
	
	private void RecalculateLayout() {
		var startY = Alignment switch {
			VerticalListAnchor.Top => 0.0f,
			VerticalListAnchor.Bottom => Height,
			_ => 0.0f,
		};

		foreach (var node in nodeList) {
			node.Y = startY;

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

	public void AddNode(T node) {
		nodeList.Add(node);
		
		node.AttachNode(this);
		node.NodeId = (uint) nodeList.Count + 1;
		
		RecalculateLayout();
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

	public override float Height {
		get => base.Height;
		set {
			base.Height = value;
			RecalculateLayout();
		}
	}
}