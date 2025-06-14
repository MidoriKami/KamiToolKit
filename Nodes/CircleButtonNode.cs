using System;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.TimelineBuilding;

namespace KamiToolKit.Nodes;

public class CircleButtonNode : ComponentNode<AtkComponentButton, AtkUldComponentDataButton> {
	protected SimpleImageNode ImageNode;
	private ButtonIcon currentIcon;

	public CircleButtonNode() {
		SetInternalComponentType(ComponentType.Button);
		
		ImageNode = new SimpleImageNode {
			TexturePath = "ui/uld/CircleButtons.tex",
			TextureSize = new Vector2(24.0f, 24.0f),
			TextureCoordinates = new Vector2(0.0f, 112.0f),
			IsVisible = true,
			WrapMode = 2,
			ImageNodeFlags = 0,
		};

		ImageNode.AttachNode(this);

		LoadTimelines();
		
		InitializeComponentEvents();
		
		AddEvent(AddonEventType.MouseClick, ClickHandler);
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			ImageNode.DetachNode();
			ImageNode.Dispose();
			
			base.Dispose(disposing);
		}
	}
	
	public Action? OnClick { get; set; }
	
	private void ClickHandler() {
		OnClick?.Invoke();
	}

	public ButtonIcon Icon {
		get => currentIcon;
		set {
			var uldInfo = GetTextureCoordinateForIcon(value);
			ImageNode.TextureCoordinates = uldInfo.TextureCoordinates;
			ImageNode.TextureSize = uldInfo.TextureSize;
			currentIcon = value;
		}
	}

	public override float Width {
		get => base.Width;
		set {
			ImageNode.Width = value;
			base.Width = value;
		}
	}

	public override float Height {
		get => base.Height;
		set {
			ImageNode.Height = value;
			base.Height = value;
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

	private void LoadTimelines() {
		AddTimeline(new TimelineBuilder()
				.BeginFrameSet(1, 59)
				.AddLabel(1, 1, AtkTimelineJumpBehavior.Start, 0)
				.AddLabel(9, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
				.AddLabel(10, 2, AtkTimelineJumpBehavior.Start, 0)
				.AddLabel(19, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
				.AddLabel(20, 3, AtkTimelineJumpBehavior.Start, 0)
				.AddLabel(29, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
				.AddLabel(30, 7, AtkTimelineJumpBehavior.Start, 0)
				.AddLabel(39, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
				.AddLabel(40, 6, AtkTimelineJumpBehavior.Start, 0)
				.AddLabel(49, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
				.AddLabel(50, 4, AtkTimelineJumpBehavior.Start, 0)
				.AddLabel(59, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
				.EndFrameSet()
				.Build()
			);
		
		ImageNode.AddTimeline(new TimelineBuilder()
				.BeginFrameSet(1, 9)
				.AddFrame(1, position: new Vector2(0,0))
				.AddFrame(1, alpha: 255)
				.AddFrame(1, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
				.EndFrameSet()
				.BeginFrameSet(10, 19)
				.AddFrame(10, position: new Vector2(0,0))
				.AddFrame(12, position: new Vector2(0,0))
				.AddFrame(10, alpha: 255)
				.AddFrame(12, alpha: 255)
				.AddFrame(10, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
				.AddFrame(12, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
				.EndFrameSet()
				.BeginFrameSet(20, 29)
				.AddFrame(20, position: new Vector2(0,1))
				.AddFrame(20, alpha: 255)
				.AddFrame(20, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
				.EndFrameSet()
				.BeginFrameSet(30, 39)
				.AddFrame(30, position: new Vector2(0,0))
				.AddFrame(30, alpha: 178)
				.AddFrame(30, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(50, 50, 50))
				.EndFrameSet()
				.BeginFrameSet(40, 49)
				.AddFrame(40, position: new Vector2(0,0))
				.AddFrame(40, alpha: 255)
				.AddFrame(40, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
				.EndFrameSet()
				.BeginFrameSet(50, 59)
				.AddFrame(50, position: new Vector2(0,0))
				.AddFrame(52, position: new Vector2(0,0))
				.AddFrame(50, alpha: 255)
				.AddFrame(52, alpha: 255)
				.AddFrame(50, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
				.AddFrame(52, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
				.EndFrameSet()
				.BeginFrameSet(130, 139)
				.AddFrame(130, position: new Vector2(0,0))
				.AddFrame(130, alpha: 255)
				.AddFrame(130, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
				.EndFrameSet()
				.BeginFrameSet(140, 149)
				.AddFrame(140, position: new Vector2(0,0))
				.AddFrame(140, alpha: 255)
				.AddFrame(140, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
				.EndFrameSet()
				.BeginFrameSet(150, 159)
				.AddFrame(150, position: new Vector2(0,0))
				.AddFrame(150, alpha: 255)
				.AddFrame(150, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
				.EndFrameSet()
				.Build()
		);
	}
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