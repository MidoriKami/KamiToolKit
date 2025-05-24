using System;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.System;

namespace KamiToolKit.Nodes.ComponentNodes.Abstract;

public abstract unsafe class ButtonBase : ComponentNode<AtkComponentButton, AtkUldComponentDataButton> {
	protected readonly NineGridNode BackgroundNode;
	protected abstract NodeBase DecorationNode { get; }

	private bool buttonHeld;

	protected AtkComponentButton* ButtonNode => (AtkComponentButton*) InternalNode;

	protected ButtonBase() {
		SetInternalComponentType(ComponentType.Button);
		Data->Nodes[1] = 2;
		
		BackgroundNode = new SimpleNineGridNode {
			TexturePath = "ui/uld/ButtonA_hr1.tex",
			IsVisible = true,
			TextureSize = new Vector2(100.0f, 28.0f),
			LeftOffset = 16.0f,
			RightOffset = 16.0f,
			PartsRenderType = 88,
			NodeId = 2,
		};

		BackgroundNode.AttachNode(this, NodePosition.AfterAllSiblings);
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			BackgroundNode.DetachNode();
			BackgroundNode.Dispose();

			NativeMemoryHelper.UiFree(Data);
			Data = null;

			base.Dispose(disposing);
		}
	}

	public override void EnableEvents(IAddonEventManager eventManager, AtkUnitBase* addon) {
		base.EnableEvents(eventManager, addon);
		
		CollisionNode.EnableEvents(eventManager, addon);
	}

	public override void DisableEvents(IAddonEventManager eventManager) {
		base.DisableEvents(eventManager);
		
		CollisionNode.DisableEvents(eventManager);
	}
	
	private Action? InternalOnClick { get; set; }

	public Action? OnClick {
		get => InternalOnClick;
		set {
			if (value is null ) {
				if (InternalOnClick is not null) {
					RemoveEvent(AddonEventType.ButtonClick, InternalOnClick);
					InternalOnClick = null;
				}
			}
			else {
				if (InternalOnClick is not null) {
					AddEvent(AddonEventType.ButtonClick, InternalOnClick);
					AddEvent(AddonEventType.ButtonClick, value);
					InternalOnClick = value;
				}
				else {
					AddEvent(AddonEventType.ButtonClick, value);
					InternalOnClick = value;
				}
			}
		}
	}

	public new float Width {
		get => InternalResNode->Width;
		set {
			InternalResNode->SetWidth((ushort) value);
			BackgroundNode.Width = value;
			CollisionNode.Width = value;
		}
	}

	public new float Height {
		get => InternalResNode->Height;
		set {
			InternalResNode->SetHeight((ushort) value);
			BackgroundNode.Height = value;
			CollisionNode.Height = value;
		}
	}

	public new Vector2 Size {
		get => new(Width, Height);
		set {
			Width = value.X;
			Height = value.Y;
		}
	}

	public bool BackgroundVisible {
		get => BackgroundNode.IsVisible;
		set => BackgroundNode.IsVisible = value;
	}
}