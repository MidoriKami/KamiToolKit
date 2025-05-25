using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.NodeParts;
using KamiToolKit.Nodes.ComponentNodes.Abstract;

namespace KamiToolKit.Nodes.ComponentNodes;

public unsafe class CircleButton : ButtonBase {
	private SimpleImageNode imageNode;
	private ButtonIcon currentIcon;

	public CircleButton() {
		imageNode = new SimpleImageNode {
			TexturePath = "ui/uld/CircleButtons_hr1.tex",
			TextureSize = new Vector2(24.0f, 24.0f),
			TextureCoordinates = new Vector2(0.0f, 112.0f),
			IsVisible = true,
			WrapMode = 2,
			ImageNodeFlags = 0,
		};

		imageNode.AttachNode(this, NodePosition.AfterAllSiblings);

		LoadTimelines();
		
		InitializeComponentEvents();
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			imageNode.DetachNode();
			imageNode.Dispose();
			
			base.Dispose(disposing);
		}
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
			CollisionNode.Width = value;
			imageNode.Width = value;
		}
	}

	public new float Height {
		get => InternalResNode->Height;
		set {
			InternalResNode->SetHeight((ushort) value);
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
	
	private void LoadTimelines() {
		AddTimeline(new Timeline {
			Mask = (AtkTimelineMask) 0xFF,
			LabelEndFrameIdx = 59,
			LabelFrameIdxDuration = 58,
			LabelSets = [
				new TimelineLabelSet {
					StartFrameId = 1, EndFrameId = 59, Labels = [
						new TimelineLabelFrame { FrameIndex = 1, LabelId = 1, },
						new TimelineLabelFrame { FrameIndex = 9, JumpBehavior = AtkTimelineJumpBehavior.PlayOnce },
						new TimelineLabelFrame { FrameIndex = 10, LabelId = 2, },
						new TimelineLabelFrame { FrameIndex = 19, JumpBehavior = AtkTimelineJumpBehavior.PlayOnce },
						new TimelineLabelFrame { FrameIndex = 20, LabelId = 3, },
						new TimelineLabelFrame { FrameIndex = 29, JumpBehavior = AtkTimelineJumpBehavior.PlayOnce },
						new TimelineLabelFrame { FrameIndex = 30, LabelId = 7, },
						new TimelineLabelFrame { FrameIndex = 39, JumpBehavior = AtkTimelineJumpBehavior.PlayOnce },
						new TimelineLabelFrame { FrameIndex = 40, LabelId = 6, },
						new TimelineLabelFrame { FrameIndex = 49, JumpBehavior = AtkTimelineJumpBehavior.PlayOnce },
						new TimelineLabelFrame { FrameIndex = 50, LabelId = 4, },
						new TimelineLabelFrame { FrameIndex = 59, JumpBehavior = AtkTimelineJumpBehavior.PlayOnce },
					],
				},
			],
		});
		
		imageNode.AddTimeline(new Timeline {
			Mask = AtkTimelineMask.VendorSpecific2,
			Animations = [
				new TimelineAnimation {
					StartFrameId = 1, EndFrameId = 9, KeyFrames = [
						new TimelineKeyFrame { FrameIndex = 1, Position = new Vector2(0.0f, 0.0f) }, 
						new TimelineKeyFrame { FrameIndex = 1, Alpha = 255 }, 
						new TimelineKeyFrame { FrameIndex = 1, NodeTint = new NodeTint { MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f) } },
					],
				},
				new TimelineAnimation {
					StartFrameId = 10, EndFrameId = 19, KeyFrames = [
						new TimelineKeyFrame { FrameIndex = 10, Position = new Vector2(0.0f, 0.0f) }, 
						new TimelineKeyFrame { FrameIndex = 10, Alpha = 255 }, 
						new TimelineKeyFrame { FrameIndex = 10, NodeTint = new NodeTint { MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f) } },

						new TimelineKeyFrame { FrameIndex = 12, Position = new Vector2(0.0f, 0.0f) }, 
						new TimelineKeyFrame { FrameIndex = 12, Alpha = 255 }, 
						new TimelineKeyFrame { FrameIndex = 12, NodeTint = new NodeTint { MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f), AddColor = new Vector3(16.0f, 16.0f, 16.0f) } },
					],
				},
				new TimelineAnimation {
					StartFrameId = 20, EndFrameId = 29, KeyFrames = [
						new TimelineKeyFrame { FrameIndex = 20, Position = new Vector2(0.0f, 1.0f) }, 
						new TimelineKeyFrame { FrameIndex = 20, Alpha = 255 }, 
						new TimelineKeyFrame { FrameIndex = 20, NodeTint = new NodeTint { MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f), AddColor = new Vector3(16.0f, 16.0f, 16.0f) } },
					],
				},
				new TimelineAnimation {
					StartFrameId = 30, EndFrameId = 39, KeyFrames = [
						new TimelineKeyFrame { FrameIndex = 30, Position = new Vector2(0.0f, 0.0f) }, 
						new TimelineKeyFrame { FrameIndex = 30, Alpha = 178 }, 
						new TimelineKeyFrame { FrameIndex = 30, NodeTint = new NodeTint { MultiplyColor = new Vector3(50.0f, 50.0f, 50.0f) } },
					],
				},
				new TimelineAnimation {
					StartFrameId = 40, EndFrameId = 49, KeyFrames = [
						new TimelineKeyFrame { FrameIndex = 40, Position = new Vector2(0.0f, 0.0f) }, 
						new TimelineKeyFrame { FrameIndex = 40, Alpha = 255 }, 
						new TimelineKeyFrame { FrameIndex = 40, NodeTint = new NodeTint { MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f), AddColor = new Vector3(16.0f, 16.0f, 16.0f) } },
					],
				},
				new TimelineAnimation {
					StartFrameId = 50, EndFrameId = 59, KeyFrames = [
						new TimelineKeyFrame { FrameIndex = 50, Position = new Vector2(0.0f, 0.0f) }, 
						new TimelineKeyFrame { FrameIndex = 50, Alpha = 255 }, 
						new TimelineKeyFrame { FrameIndex = 50, NodeTint = new NodeTint { MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f), AddColor = new Vector3(16.0f, 16.0f, 16.0f) } },

						new TimelineKeyFrame { FrameIndex = 52, Position = new Vector2(0.0f, 0.0f) }, 
						new TimelineKeyFrame { FrameIndex = 52, Alpha = 255 }, 
						new TimelineKeyFrame { FrameIndex = 52, NodeTint = new NodeTint { MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f) } },
					],
				},
				new TimelineAnimation {
					StartFrameId = 130, EndFrameId = 139, KeyFrames = [
						new TimelineKeyFrame { FrameIndex = 130, Position = new Vector2(0.0f, 0.0f) }, 
						new TimelineKeyFrame { FrameIndex = 130, Alpha = 255 }, 
						new TimelineKeyFrame { FrameIndex = 130, NodeTint = new NodeTint { MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f), AddColor = new Vector3(16.0f, 16.0f, 16.0f) } },
					],
				},
				new TimelineAnimation {
					StartFrameId = 140, EndFrameId = 149, KeyFrames = [ 
						new TimelineKeyFrame { FrameIndex = 140, Position = new Vector2(0.0f, 0.0f) }, 
						new TimelineKeyFrame { FrameIndex = 140, Alpha = 255 }, 
						new TimelineKeyFrame { FrameIndex = 140, NodeTint = new NodeTint { MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f) } },
					],
				},
				new TimelineAnimation {
					StartFrameId = 150, EndFrameId = 159, KeyFrames = [ 
						new TimelineKeyFrame { FrameIndex = 150, Position = new Vector2(0.0f, 0.0f) }, 
						new TimelineKeyFrame { FrameIndex = 150, Alpha = 255 }, 
						new TimelineKeyFrame { FrameIndex = 150, NodeTint = new NodeTint { MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f) } },
					],
				},
			],
		});
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