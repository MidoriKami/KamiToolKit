using System;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using Dalamud.Interface;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public abstract unsafe class ButtonBaseComponent : ComponentNode<AtkComponentButton, AtkUldComponentDataButton> {
	private readonly NineGridNode backgroundNode;
	protected abstract NodeBase DecorationNode { get; }

	public Action? OnClick { get; set; }
	private IAddonEventHandle? OnClickHandle { get; set; }

	protected ButtonBaseComponent() {
		SetInternalComponentType(ComponentType.Button);
		Data = NativeMemoryHelper.UiAlloc<AtkUldComponentDataButton>();
		Data->Nodes[0] = 2;
		Data->Nodes[1] = 3;
		
		backgroundNode = new SimpleNineGridNode {
			TexturePath = "ui/uld/ButtonA_hr1.tex",
			IsVisible = true,
			TextureSize = new Vector2(100.0f, 28.0f),
			LeftOffset = 16.0f,
			RightOffset = 16.0f,
			PartsRenderType = (PartsRenderType)88,
			NodeID = 2,
		};

		backgroundNode.AttachNode(this, NodePosition.AfterAllSiblings);
		
		CollisionNode.MouseOver = OnMouseOver;
		CollisionNode.MouseOut = OnMouseOut;
		CollisionNode.MouseDown = OnMouseDown;
		CollisionNode.MouseUp = OnMouseUp;
		CollisionNode.MouseClick = OnMouseClick;
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			backgroundNode.Dispose();
			DecorationNode.Dispose();
		
			NativeMemoryHelper.UiFree(Data);
		
			base.Dispose(disposing);
		}
	}
	
	private void OnMouseClick() {
		OnClick?.Invoke();
	}

	public override void EnableEvents(IAddonEventManager eventManager, AtkUnitBase* addon) {
		base.EnableEvents(eventManager, addon);
		CollisionNode.EnableEvents(eventManager, addon);

		eventManager.AddEvent((nint) addon, (nint) InternalResNode, AddonEventType.ButtonClick, OnClickHandler);
	}

	private void OnClickHandler(AddonEventType atkEventType, IntPtr atkUnitBase, IntPtr atkResNode) {
		if (!IsVisible) return;
		
		if (atkEventType is AddonEventType.ButtonClick) {
			OnClick?.Invoke();
		}
	}

	public override void DisableEvents(IAddonEventManager eventManager) {
		base.DisableEvents(eventManager);
		CollisionNode.DisableEvents(eventManager);

		if (OnClickHandle is not null) {
			eventManager.RemoveEvent(OnClickHandle);
		}
	}

	private void OnMouseDown() {
		backgroundNode.Position += new Vector2(0.0f, 1.0f);
		DecorationNode.Position += new Vector2(0.0f, 1.0f);
	}
	
	private void OnMouseUp() {
		backgroundNode.Position -= new Vector2(0.0f, 1.0f);
		DecorationNode.Position -= new Vector2(0.0f, 1.0f);
	}

	private void OnMouseOver() {
		backgroundNode.AddColor = new Vector3(16.0f, 16.0f, 16.0f).AsVector4().NormalizeToUnitRange().AsVector3();
		SetCursor(AddonCursorType.Clickable);
	}
	
	private void OnMouseOut() {
		backgroundNode.AddColor = Vector3.Zero;
		ResetCursor();
	}
	
	public new float Width {
		get => InternalResNode->Width;
		set {
			InternalResNode->SetWidth((ushort) value);
			backgroundNode.Width = value;
			DecorationNode.Width = value - backgroundNode.LeftOffset - backgroundNode.RightOffset;
			CollisionNode.Width = value;
		}
	}

	public new float Height {
		get => InternalResNode->Height;
		set {
			InternalResNode->SetHeight((ushort) value);
			backgroundNode.Height = value;
			DecorationNode.Height = value - 8.0f;
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
}