using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.System;
using Newtonsoft.Json;

namespace KamiToolKit.Nodes;

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
	
	[JsonProperty] public float HorizontalSpacing { get; set; }
	
	private void RecalculateLayout() {
		var startX = Alignment switch {
			HorizontalListAnchor.Left => 0.0f,
			HorizontalListAnchor.Right => Width,
			_ => 0.0f,
		};

		foreach (var node in nodeList) {
			if (!node.IsVisible) continue;
			
			node.Y = startX;

			switch (Alignment) {
				case HorizontalListAnchor.Left:
					startX += node.Width + HorizontalSpacing;
					break;
				
				case HorizontalListAnchor.Right:
					startX -= node.Width + HorizontalSpacing;
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

	public override float Width {
		get => base.Width;
		set {
			base.Width = value;
			RecalculateLayout();
		}
	}
}