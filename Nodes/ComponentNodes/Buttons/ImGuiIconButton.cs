using System.Numerics;
using Dalamud.Interface.Textures.TextureWraps;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.NodeParts;
using KamiToolKit.Nodes.ComponentNodes.Abstract;

namespace KamiToolKit.Nodes.ComponentNodes;

public unsafe class ImGuiIconButton : ButtonBase {
	private readonly ImGuiImageNode imageNode;

	public ImGuiIconButton() {
		imageNode = new ImGuiImageNode {
			IsVisible = true,
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

	public new float Width {
		get => InternalResNode->Width;
		set {
			InternalResNode->SetWidth((ushort) value);
			CollisionNode.Width = value;
			imageNode.Width = value;
			Component->UldManager.RootNodeWidth = (ushort) value;
		}
	}

	public new float Height {
		get => InternalResNode->Height;
		set {
			InternalResNode->SetHeight((ushort) value);
			CollisionNode.Height = value;
			imageNode.Height = value;
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

	public void LoadTexture(IDalamudTextureWrap texture)
		=> imageNode.LoadTexture(texture);
	
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