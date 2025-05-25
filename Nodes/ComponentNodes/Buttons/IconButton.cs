using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.NodeParts;
using KamiToolKit.Nodes.ComponentNodes.Abstract;

namespace KamiToolKit.Nodes.ComponentNodes;

/// <summary>
/// Uses a GameIconId to display that icon as the decorator for the button.
/// </summary>
public unsafe class IconButton : ButtonBase {
	private IconImageNode imageNode;
	private readonly NineGridNode backgroundNode;

	public IconButton() {
		backgroundNode = new SimpleNineGridNode {
			TexturePath = "ui/uld/BgParts_hr1.tex",
			TextureSize = new Vector2(32.0f, 32.0f),
			TextureCoordinates = new Vector2(33.0f, 65.0f),
			TopOffset = 8.0f,
			LeftOffset = 8.0f,
			RightOffset = 8.0f,
			BottomOffset = 8.0f,
			NodeId = 2,
			IsVisible = true,
		};

		backgroundNode.AttachNode(this, NodePosition.AfterAllSiblings);
		
		imageNode = new IconImageNode {
			IsVisible = true,
			TextureSize = new Vector2(32.0f, 32.0f),
			NodeId = 3,
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

	public uint IconId {
		get => imageNode.IconId;
		set => imageNode.IconId = value;
	}

	public new float Width {
		get => InternalResNode->Width;
		set {
			InternalResNode->SetWidth((ushort) value);
			backgroundNode.Width = value;
			CollisionNode.Width = value;
			imageNode.Width = value - backgroundNode.LeftOffset - backgroundNode.RightOffset;
			imageNode.Position = imageNode.Position with { X = backgroundNode.Position.X + backgroundNode.LeftOffset };
			Component->UldManager.RootNodeWidth = (ushort) value;
		}
	}

	public new float Height {
		get => InternalResNode->Height;
		set {
			InternalResNode->SetHeight((ushort) value);
			backgroundNode.Height = value;
			CollisionNode.Height = value;
			imageNode.Height = value - backgroundNode.TopOffset - backgroundNode.BottomOffset;
			imageNode.Position = imageNode.Position with { Y = backgroundNode.Position.Y + backgroundNode.TopOffset };
			Component->UldManager.RootNodeHeight = (ushort) value;
		}
	}

	public new Vector2 Size {
		get => new(Width, Height);
		set {
			Width = value.X;
			Height = value.Y;
		}
	}

	private void LoadTimelines() {
		AddTimeline(new Timeline {
			Mask = (AtkTimelineMask) 0xFF,
			LabelEndFrameIdx = 53,
			LabelFrameIdxDuration = 52,
			LabelSets = [
				new TimelineLabelSet {
					StartFrameId = 1, EndFrameId = 53, Labels = [
						new TimelineLabelFrame { FrameIndex = 1, LabelId = 1, },
						new TimelineLabelFrame { FrameIndex = 10, JumpBehavior = AtkTimelineJumpBehavior.PlayOnce },
						new TimelineLabelFrame { FrameIndex = 11, LabelId = 2, },
						new TimelineLabelFrame { FrameIndex = 17, JumpBehavior = AtkTimelineJumpBehavior.PlayOnce },
						new TimelineLabelFrame { FrameIndex = 18, LabelId = 3, },
						new TimelineLabelFrame { FrameIndex = 26, JumpBehavior = AtkTimelineJumpBehavior.PlayOnce },
						new TimelineLabelFrame { FrameIndex = 27, LabelId = 7, },
						new TimelineLabelFrame { FrameIndex = 36, JumpBehavior = AtkTimelineJumpBehavior.PlayOnce },
						new TimelineLabelFrame { FrameIndex = 37, LabelId = 6, },
						new TimelineLabelFrame { FrameIndex = 46, JumpBehavior = AtkTimelineJumpBehavior.PlayOnce },
						new TimelineLabelFrame { FrameIndex = 47, LabelId = 4, },
						new TimelineLabelFrame { FrameIndex = 53, JumpBehavior = AtkTimelineJumpBehavior.PlayOnce },
					],
				},
			],
		});
		
		backgroundNode.AddTimeline(new Timeline {
			Mask = AtkTimelineMask.VendorSpecific2,
			Animations = [ 
				new TimelineAnimation {
					StartFrameId = 1, EndFrameId = 10, KeyFrames = [ 
						new TimelineKeyFrame { FrameIndex = 1, Position = new Vector2(0.0f, 0.0f) }, 
						new TimelineKeyFrame { FrameIndex = 1, Alpha = 255 }, 
						new TimelineKeyFrame { FrameIndex = 1, NodeTint = new NodeTint { MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f) } },
					],
				},
				new TimelineAnimation {
					StartFrameId = 11, EndFrameId = 17, KeyFrames = [ 
						new TimelineKeyFrame { FrameIndex = 11, Position = new Vector2(0.0f, 0.0f) }, 
						new TimelineKeyFrame { FrameIndex = 11, Alpha = 255 }, 
						new TimelineKeyFrame { FrameIndex = 11, NodeTint = new NodeTint { MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f) } },

						new TimelineKeyFrame { FrameIndex = 13, Position = new Vector2(0.0f, 0.0f) }, 
						new TimelineKeyFrame { FrameIndex = 13, Alpha = 255 }, 
						new TimelineKeyFrame { FrameIndex = 13, NodeTint = new NodeTint { MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f), AddColor = new Vector3(16.0f, 16.0f, 16.0f) } },
					],
				},
				new TimelineAnimation {
					StartFrameId = 18, EndFrameId = 26, KeyFrames = [
						new TimelineKeyFrame { FrameIndex = 18, Position = new Vector2(0.0f, 1.0f) }, 
						new TimelineKeyFrame { FrameIndex = 18, Alpha = 255 }, 
						new TimelineKeyFrame { FrameIndex = 18, NodeTint = new NodeTint { MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f), AddColor = new Vector3(16.0f, 16.0f, 16.0f) } },
					],
				},
				new TimelineAnimation {
					StartFrameId = 27, EndFrameId = 36, KeyFrames = [
						new TimelineKeyFrame { FrameIndex = 27, Position = new Vector2(0.0f, 0.0f) }, 
						new TimelineKeyFrame { FrameIndex = 27, Alpha = 178 }, 
						new TimelineKeyFrame { FrameIndex = 27, NodeTint = new NodeTint { MultiplyColor = new Vector3(50.0f, 50.0f, 50.0f) } },
					],
				},
				new TimelineAnimation {
					StartFrameId = 37, EndFrameId = 46, KeyFrames = [
						new TimelineKeyFrame { FrameIndex = 37, Position = new Vector2(0.0f, 0.0f) }, 
						new TimelineKeyFrame { FrameIndex = 37, Alpha = 255 }, 
						new TimelineKeyFrame { FrameIndex = 37, NodeTint = new NodeTint { MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f), AddColor = new Vector3(16.0f, 16.0f, 16.0f) } },
					],
				},
				new TimelineAnimation {
					StartFrameId = 47, EndFrameId = 53, KeyFrames = [
						new TimelineKeyFrame { FrameIndex = 47, Position = new Vector2(0.0f, 0.0f) }, 
						new TimelineKeyFrame { FrameIndex = 47, Alpha = 255 }, 
						new TimelineKeyFrame { FrameIndex = 47, NodeTint = new NodeTint { MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f), AddColor = new Vector3(16.0f, 16.0f, 16.0f) } },

						new TimelineKeyFrame { FrameIndex = 53, Position = new Vector2(0.0f, 0.0f) }, 
						new TimelineKeyFrame { FrameIndex = 53, Alpha = 255 }, 
						new TimelineKeyFrame { FrameIndex = 53, NodeTint = new NodeTint { MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f) } },
					],
				},
			],
		});
		
		imageNode.AddTimeline(new Timeline {
			Animations = [ 
				new TimelineAnimation {
					StartFrameId = 1, EndFrameId = 10, KeyFrames = [ 
						new TimelineKeyFrame { FrameIndex = 1, Position = new Vector2(8.0f, 8.0f) }, 
						new TimelineKeyFrame { FrameIndex = 1, Alpha = 255 }, 
						new TimelineKeyFrame { FrameIndex = 1, NodeTint = new NodeTint { MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f) } },
					],
				},
				new TimelineAnimation {
					StartFrameId = 11, EndFrameId = 17, KeyFrames = [ 
						new TimelineKeyFrame { FrameIndex = 11, Position = new Vector2(8.0f, 8.0f) }, 
						new TimelineKeyFrame { FrameIndex = 11, Alpha = 255 }, 
						new TimelineKeyFrame { FrameIndex = 11, NodeTint = new NodeTint { MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f) } },
					],
				},
				new TimelineAnimation {
					StartFrameId = 18, EndFrameId = 26, KeyFrames = [
						new TimelineKeyFrame { FrameIndex = 18, Position = new Vector2(8.0f, 9.0f) }, 
						new TimelineKeyFrame { FrameIndex = 18, Alpha = 255 }, 
						new TimelineKeyFrame { FrameIndex = 18, NodeTint = new NodeTint { MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f) } },
					],
				},
				new TimelineAnimation {
					StartFrameId = 27, EndFrameId = 36, KeyFrames = [
						new TimelineKeyFrame { FrameIndex = 27, Position = new Vector2(8.0f, 8.0f) }, 
						new TimelineKeyFrame { FrameIndex = 27, Alpha = 153 }, 
						new TimelineKeyFrame { FrameIndex = 27, NodeTint = new NodeTint { MultiplyColor = new Vector3(80.0f, 80.0f, 80.0f) } },
					],
				},
				new TimelineAnimation {
					StartFrameId = 37, EndFrameId = 46, KeyFrames = [
						new TimelineKeyFrame { FrameIndex = 37, Position = new Vector2(8.0f, 8.0f) }, 
						new TimelineKeyFrame { FrameIndex = 37, Alpha = 255 }, 
						new TimelineKeyFrame { FrameIndex = 37, NodeTint = new NodeTint { MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f) } },
					],
				},
				new TimelineAnimation {
					StartFrameId = 47, EndFrameId = 53, KeyFrames = [
						new TimelineKeyFrame { FrameIndex = 47, Position = new Vector2(8.0f, 8.0f) }, 
						new TimelineKeyFrame { FrameIndex = 47, Alpha = 255 }, 
						new TimelineKeyFrame { FrameIndex = 47, NodeTint = new NodeTint { MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f) } },
					],
				},
			],
		});
	}
}