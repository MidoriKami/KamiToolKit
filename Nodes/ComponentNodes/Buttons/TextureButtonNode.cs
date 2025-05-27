using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.NodeParts;
using KamiToolKit.Nodes.ComponentNodes.Abstract;

namespace KamiToolKit.Nodes.ComponentNodes;

public class TextureButtonNode : ButtonBase {
	private readonly SimpleImageNode imageNode;

	public TextureButtonNode() {
		imageNode = new ImGuiImageNode {
			IsVisible = true,
			NodeId = 3,
			WrapMode = 1,
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

	internal override bool SuppressDispose {
		get => base.SuppressDispose;
		set {
			base.SuppressDispose = value;
			imageNode.SuppressDispose = value;
		}
	}

	public string TexturePath {
		get => imageNode.TexturePath;
		set => imageNode.TexturePath = value;
	}

	public Vector2 TextureCoordinates {
		get => imageNode.TextureCoordinates;
		set => imageNode.TextureCoordinates = value;
	}

	public Vector2 TextureSize {
		get => imageNode.TextureSize;
		set => imageNode.TextureSize = value;
	}
	
	public override float Width {
		get => base.Width;
		set {
			imageNode.Width = value;
			base.Width = value;
		}
	}

	public override float Height {
		get => base.Height;
		set {
			imageNode.Height = value;
			base.Height = value;
		}
	}

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