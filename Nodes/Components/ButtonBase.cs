using System;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using Dalamud.Interface;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public abstract unsafe class ButtonBase : ComponentNode<AtkComponentButton, AtkUldComponentDataButton> {
	protected readonly NineGridNode BackgroundNode;
	protected abstract NodeBase DecorationNode { get; }

	public Action? OnClick { get; set; }
	private IAddonEventHandle? OnClickHandle { get; set; }

	protected override NodeBase EventTargetNode => CollisionNode;

	protected ButtonBase() {
		SetInternalComponentType(ComponentType.Button);
		Data = NativeMemoryHelper.UiAlloc<AtkUldComponentDataButton>();
		Data->Nodes[0] = 2;
		Data->Nodes[1] = 3;
		
		NodeType = (NodeType) 1001;
		
		BackgroundNode = new SimpleNineGridNode {
			TexturePath = "ui/uld/ButtonA_hr1.tex",
			IsVisible = true,
			TextureSize = new Vector2(100.0f, 28.0f),
			LeftOffset = 16.0f,
			RightOffset = 16.0f,
			PartsRenderType = (PartsRenderType)88,
			NodeID = 2,
		};

		BackgroundNode.AttachNode(this, NodePosition.AfterAllSiblings);
		
		CollisionNode.MouseOver += OnMouseOver;
		CollisionNode.MouseOut += OnMouseOut;
		CollisionNode.MouseDown += OnMouseDown;
		CollisionNode.MouseUp += OnMouseUp;
		CollisionNode.MouseClick += OnMouseClick;
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			BackgroundNode.Dispose();
			DecorationNode.Dispose();

			NativeMemoryHelper.UiFree(Data);
			Data = null;

			base.Dispose(disposing);
		}
	}
	
	private void OnMouseClick() {
		OnClick?.Invoke();
	}

	public override void EnableEvents(IAddonEventManager eventManager, AtkUnitBase* addon) {
		base.EnableEvents(eventManager, addon);

		eventManager.AddEvent((nint) addon, (nint) EventTargetNode.InternalResNode, AddonEventType.ButtonClick, OnClickHandler);
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
		BackgroundNode.Position += new Vector2(0.0f, 1.0f);
		DecorationNode.Position += new Vector2(0.0f, 1.0f);
	}
	
	private void OnMouseUp() {
		BackgroundNode.Position -= new Vector2(0.0f, 1.0f);
		DecorationNode.Position -= new Vector2(0.0f, 1.0f);
	}

	private void OnMouseOver() {
		BackgroundNode.AddColor = new Vector3(16.0f, 16.0f, 16.0f).AsVector4().NormalizeToUnitRange().AsVector3();
		SetCursor(AddonCursorType.Clickable);
	}
	
	private void OnMouseOut() {
		BackgroundNode.AddColor = Vector3.Zero;
		ResetCursor();
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
}