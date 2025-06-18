using System;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.System;

namespace KamiToolKit.Nodes;

public unsafe class ScrollBarNode : ComponentNode<AtkComponentScrollBar, AtkUldComponentDataScrollBar> {

	protected readonly ScrollBarBackgroundButtonNode BackgroundButtonNode;
	protected readonly ScrollBarForegroundButtonNode ForegroundButtonNode;
	
	public ScrollBarNode() {
		SetInternalComponentType(ComponentType.ScrollBar);

		BackgroundButtonNode = new ScrollBarBackgroundButtonNode {
			NodeId = 3,
			Size = new Vector2(8.0f, 306.0f),
			IsVisible = true,
		};
		
		Log.Debug("Attaching BackgroundButtonNode");
		BackgroundButtonNode.AttachNode(this);

		ForegroundButtonNode = new ScrollBarForegroundButtonNode {
			NodeId = 2,
			Size = new Vector2(8.0f, 306.0f), 
			IsVisible = true,
		};
		
		Log.Debug("Attaching ForegroundButtonNode");
		ForegroundButtonNode.AttachNode(this);
		
		Data->Nodes[0] = ForegroundButtonNode.NodeId;
		Data->Nodes[1] = 0; // Arrow Up Button
		Data->Nodes[2] = 0; // Arrow Down Button
		Data->Nodes[3] = BackgroundButtonNode.NodeId;

		Data->Vertical = 1;
		Data->Margin = 0;

		InitializeComponentEvents();

		Component->MouseDownScreenPos = 0;
		Component->MouseWheelSpeed = 24;

		AddEvent(AddonEventType.ValueUpdate, UpdateHandler);
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			BackgroundButtonNode.Dispose();
			ForegroundButtonNode.Dispose();
			
			base.Dispose(disposing);
		}
	}

	public Action<int>? OnValueChanged { get; set; }
	
	private void UpdateHandler(AddonEventData obj) {
		OnValueChanged?.Invoke(Component->PendingScrollPosition);
	}

	public NodeBase ContentNode {
		set {
			Component->ContentNode = value.InternalResNode;
			UpdateScrollParams();
		}
	}

	public CollisionNode ContentCollisionNode {
		set {
			Component->ContentCollisionNode = value.InternalNode;
			UpdateScrollParams();
		}
	}

	public override float Height {
		get => base.Height;
		set {
			base.Height = value;
			BackgroundButtonNode.Height = value;
			ForegroundButtonNode.Height = value;
		}
	}

	public override float Width {
		get => base.Width;
		set {
			base.Width = value;
			BackgroundButtonNode.Width = value;
			ForegroundButtonNode.Width = value;
		}
	}

	public int ScrollPosition {
		get => Component->ScrollPosition;
		set => Component->SetScrollPosition(value);
	}

	/// <summary>
	/// Updates from attached Content and Collision nodes
	/// </summary>
	public void UpdateScrollParams() {
		if (Component->ContentNode is null) return;
		if (Component->ContentCollisionNode is null) return;

		var content = Component->ContentNode;
		var collision = Component->ContentCollisionNode;

		UpdateScrollParams(collision->Height, content->Height);
	}

	public void UpdateScrollParams(int barHeight, int offScreenHeight) {
		var distance = offScreenHeight - barHeight;

		Component->ScrollbarLength = (short) barHeight;
		Component->ScrollMaxPosition = distance;
		Component->ContentNodeOffScreenLength = (short) distance;
		Component->EmptyLength = barHeight - (int) ((float) barHeight / offScreenHeight * barHeight);
		ForegroundButtonNode.Height = barHeight - Component->EmptyLength;
	}
}