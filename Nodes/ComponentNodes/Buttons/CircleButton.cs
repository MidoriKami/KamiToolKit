using System.Numerics;
using Dalamud.Game.Addon.Events;
using Dalamud.Interface;
using KamiToolKit.Classes;
using KamiToolKit.Nodes.ComponentNodes.Abstract;
using KamiToolKit.System;

namespace KamiToolKit.Nodes.ComponentNodes;

public unsafe class CircleButton : ButtonBase {
	protected override NodeBase DecorationNode => imageNode;
	private SimpleImageNode imageNode;
	private ButtonIcon currentIcon;

	public CircleButton() {
		BackgroundVisible = false;
		imageNode = new SimpleImageNode {
			TexturePath = "ui/uld/CircleButtons_hr1.tex",
			TextureSize = new Vector2(24.0f, 24.0f),
			TextureCoordinates = new Vector2(0.0f, 112.0f),
			IsVisible = true,
			WrapMode = 2,
			ImageNodeFlags = 0,
		};

		imageNode.AttachNode(this, NodePosition.AfterAllSiblings);
		
		InitializeComponentEvents();
		
		AddEvent(AddonEventType.MouseOver,  OnMouseOver);
		AddEvent(AddonEventType.MouseOut,  OnMouseOut);
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			imageNode.DetachNode();
			imageNode.Dispose();
			
			RemoveEvent(AddonEventType.MouseOver, OnMouseOver);
			RemoveEvent(AddonEventType.MouseOut, OnMouseOut);
			
			base.Dispose(disposing);
		}
	}

	private void OnMouseOver() {
		imageNode.AddColor = new Vector3(16.0f, 16.0f, 16.0f).AsVector4().NormalizeToUnitRange().AsVector3();
	}
	
	private void OnMouseOut() {
		imageNode.AddColor = Vector3.Zero;
	}

	public ButtonIcon Icon {
		get => currentIcon;
		set {
			var uldInfo = GetTextureCoordinateForIcon(value);
			imageNode.TextureCoordinates = uldInfo.TextureCoordinates;
			imageNode.TextureSize = uldInfo.TextureSize;
			currentIcon = value;
		}
	}

	public new float Width {
		get => InternalResNode->Width;
		set {
			InternalResNode->SetWidth((ushort) value);
			BackgroundNode.Width = value;
			CollisionNode.Width = value;
			imageNode.Width = value;
		}
	}

	public new float Height {
		get => InternalResNode->Height;
		set {
			InternalResNode->SetHeight((ushort) value);
			BackgroundNode.Height = value;
			CollisionNode.Height = value;
			imageNode.Height = value;
		}
	}

	public new Vector2 Size {
		get => new(Width, Height);
		set {
			Width = value.X;
			Height = value.Y;
		}
	}

	private UldTextureInfo GetTextureCoordinateForIcon(ButtonIcon icon) => icon switch {
		ButtonIcon.GearCog => new UldTextureInfo(0.0f, 0.0f, 28.0f, 28.0f),
		ButtonIcon.Filter => new UldTextureInfo(28.0f, 0.0f, 28.0f, 28.0f),
		ButtonIcon.Sort => new UldTextureInfo(56.0f, 0.0f, 28.0f, 28.0f),
		ButtonIcon.QuestionMark => new UldTextureInfo(84.0f, 0.0f, 28.0f, 28.0f),
		ButtonIcon.Refresh => new UldTextureInfo(112.0f, 0.0f, 28.0f, 28.0f),
		ButtonIcon.ChatBubble => new UldTextureInfo(140.0f, 0.0f, 28.0f, 28.0f),
		ButtonIcon.LeftArrow => new UldTextureInfo(168.0f, 0.0f, 28.0f, 28.0f),
		ButtonIcon.UpArrow => new UldTextureInfo(196.0f, 0.0f, 28.0f, 28.0f),
		ButtonIcon.Chest => new UldTextureInfo(224.0f, 0.0f, 28.0f, 28.0f),
		
		ButtonIcon.Document => new UldTextureInfo(0.0f, 28.0f, 28.0f, 28.0f),
		ButtonIcon.Edit => new UldTextureInfo(28.0f, 28.0f, 28.0f, 28.0f),
		ButtonIcon.Add => new UldTextureInfo(56.0f, 28.0f, 28.0f, 28.0f),
		ButtonIcon.RightArrow => new UldTextureInfo(84.0f, 28.0f, 28.0f, 28.0f),
		ButtonIcon.MusicNote => new UldTextureInfo(112.0f, 28.0f, 28.0f, 28.0f),
		ButtonIcon.Sprout => new UldTextureInfo(140.0f, 28.0f, 28.0f, 28.0f),
		ButtonIcon.Dice => new UldTextureInfo(168.0f, 28.0f, 28.0f, 28.0f),
		ButtonIcon.ArrowDown => new UldTextureInfo(196.0f, 28.0f, 28.0f, 28.0f),
		
		ButtonIcon.Eye => new UldTextureInfo(0.0f, 56.0f, 28.0f, 28.0f),
		ButtonIcon.Envelope => new UldTextureInfo(28.0f, 56.0f, 28.0f, 28.0f),
		ButtonIcon.Volume => new UldTextureInfo(56.0f, 56.0f, 28.0f, 28.0f),
		ButtonIcon.Mute => new UldTextureInfo(84.0f, 56.0f, 28.0f, 28.0f),
		ButtonIcon.WavePulse => new UldTextureInfo(112.0f, 56.0f, 28.0f, 28.0f),
		ButtonIcon.CheckedBox => new UldTextureInfo(140.0f, 56.0f, 28.0f, 28.0f),
		ButtonIcon.Cross => new UldTextureInfo(168.0f, 56.0f, 28.0f, 28.0f),
		ButtonIcon.Globe => new UldTextureInfo(196.0f, 56.0f, 28.0f, 28.0f),
		
		ButtonIcon.ActiveGearCog => new UldTextureInfo(0.0f, 84.0f, 28.0f, 28.0f),
		ButtonIcon.ActiveFilter => new UldTextureInfo(28.0f, 84.0f, 28.0f, 28.0f),
		ButtonIcon.Update => new UldTextureInfo(56.0f, 84.0f, 28.0f, 28.0f),
		ButtonIcon.ActiveRing => new UldTextureInfo(84.0f, 84.0f, 28.0f, 28.0f),
		ButtonIcon.Exclamation => new UldTextureInfo(112.0f, 84.0f, 28.0f, 28.0f),
		ButtonIcon.InsetDocument => new UldTextureInfo(140.0f, 84.0f, 28.0f, 28.0f),
		ButtonIcon.GearCogWithChatBubble => new UldTextureInfo(168.0f, 84.0f, 28.0f, 28.0f),
		ButtonIcon.FlatbedCartBoxes => new UldTextureInfo(196.0f, 84.0f, 28.0f, 28.0f),
		
		ButtonIcon.MagnifyingGlass => new UldTextureInfo(0.0f, 128.0f, 24.0f, 24.0f),
		ButtonIcon.EditSmall => new UldTextureInfo(24.0f, 112.0f, 24.0f, 24.0f),
		ButtonIcon.WeaponDraw => new UldTextureInfo(48.0f, 112.0f, 24.0f, 24.0f),
		ButtonIcon.Headgear => new UldTextureInfo(72.0f, 112.0f, 24.0f, 24.0f),
		ButtonIcon.Sword => new UldTextureInfo(96.0f, 112.0f, 24.0f, 24.0f),
		ButtonIcon.Emotes => new UldTextureInfo(120.0f, 112.0f, 24.0f, 24.0f),
		ButtonIcon.PersonStanding => new UldTextureInfo(144.0f, 112.0f, 24.0f, 24.0f),
		
		ButtonIcon.PaintBucket => new UldTextureInfo(0.0f, 136.0f, 24.0f, 24.0f),
		ButtonIcon.EyeSmall => new UldTextureInfo(24.0f, 136.0f, 24.0f, 24.0f),
		ButtonIcon.Undo => new UldTextureInfo(48.0f, 136.0f, 24.0f, 24.0f),
		ButtonIcon.PinPaper => new UldTextureInfo(72.0f, 136.0f, 24.0f, 24.0f),
		ButtonIcon.CrossSmall => new UldTextureInfo(96.0f, 136.0f, 24.0f, 24.0f),
		
		_ => new UldTextureInfo(0.0f, 0.0f, 28.0f, 28.0f),
	};
}

public enum ButtonIcon {
	GearCog,
	Filter,
	Sort,
	QuestionMark,
	Refresh,
	ChatBubble,
	LeftArrow,
	UpArrow,
	Chest,
	Document,
	Edit,
	Add,
	RightArrow,
	MusicNote,
	Sprout,
	Dice,
	ArrowDown,
	Eye,
	Envelope,
	Volume,
	Mute,
	WavePulse,
	CheckedBox,
	Cross,
	Globe,
	ActiveGearCog,
	ActiveFilter,
	Update,
	ActiveRing,
	Exclamation,
	InsetDocument,
	GearCogWithChatBubble,
	FlatbedCartBoxes,
	MagnifyingGlass,
	EditSmall,
	WeaponDraw,
	Headgear,
	Sword,
	Emotes,
	PersonStanding,
	PaintBucket,
	EyeSmall,
	Undo,
	PinPaper,
	CrossSmall,
}

internal record UldTextureInfo(float PositionX = 0.0f, float PositionY = 0.0f, float Width = 0.0f, float Height = 0.0f) {
	public Vector2 TextureCoordinates => new(PositionX, PositionY);
	public Vector2 TextureSize => new(Width, Height);
}