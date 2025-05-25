using System.Numerics;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.NodeParts;
using KamiToolKit.Nodes.ComponentNodes.Abstract;

namespace KamiToolKit.Nodes.ComponentNodes;

public unsafe class TextButton : ButtonBase {

	private readonly TextNode labelNode;
	private readonly NineGridNode backgroundNode;

	public TextButton() {
		Data->Nodes[0] = 3;
		Data->Nodes[1] = 2;

		backgroundNode = new SimpleNineGridNode {
			TexturePath = "ui/uld/ButtonA_hr1.tex",
			TextureSize = new Vector2(100.0f, 28.0f),
			LeftOffset = 16.0f,
			RightOffset = 16.0f,
			NodeId = 2,
		};

		backgroundNode.AttachNode(this, NodePosition.AfterAllSiblings);
		
		labelNode = new TextNode {
			AlignmentType = AlignmentType.Center,
			Position = new Vector2(16.0f, 3.0f),
			NodeId = 3,
		};
		
		labelNode.AttachNode(this, NodePosition.AfterAllSiblings);
		
		LoadTimelines();
		
		InitializeComponentEvents();
	}

	public SeString Label {
		get => labelNode.Text;
		set => labelNode.Text = value;
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			labelNode.DetachNode();
			labelNode.Dispose();
			
			base.Dispose(disposing);
		}
	}

	public new float Width {
		get => InternalResNode->Width;
		set {
			InternalResNode->SetWidth((ushort) value);
			backgroundNode.Width = value;
			labelNode.Width = value - backgroundNode.LeftOffset - backgroundNode.RightOffset;
			CollisionNode.Width = value;
			Component->UldManager.RootNodeWidth = (ushort) value;
		}
	}

	public new float Height {
		get => InternalResNode->Height;
		set {
			InternalResNode->SetHeight((ushort) value);
			backgroundNode.Height = value;
			labelNode.Height = value - 8.0f;
			CollisionNode.Height = value;
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
						new TimelineKeyFrame { FrameIndex = 1, Label = new AtkTimelineLabel{ JumpBehavior = AtkTimelineJumpBehavior.Start, LabelId = 1, JumpLabelId = 0 }},
						new TimelineKeyFrame { FrameIndex = 10, Label = new AtkTimelineLabel{ JumpBehavior = AtkTimelineJumpBehavior.PlayOnce, LabelId = 0, JumpLabelId = 0 }},
						new TimelineKeyFrame { FrameIndex = 11, Label = new AtkTimelineLabel{ JumpBehavior = AtkTimelineJumpBehavior.Start, LabelId = 2, JumpLabelId = 0 }},
						new TimelineKeyFrame { FrameIndex = 17, Label = new AtkTimelineLabel{ JumpBehavior = AtkTimelineJumpBehavior.PlayOnce, LabelId = 0, JumpLabelId = 0 }},
						new TimelineKeyFrame { FrameIndex = 18, Label = new AtkTimelineLabel{ JumpBehavior = AtkTimelineJumpBehavior.Start, LabelId = 3, JumpLabelId = 0 }},
						new TimelineKeyFrame { FrameIndex = 26, Label = new AtkTimelineLabel{ JumpBehavior = AtkTimelineJumpBehavior.PlayOnce, LabelId = 0, JumpLabelId = 0 }},
						new TimelineKeyFrame { FrameIndex = 27, Label = new AtkTimelineLabel{ JumpBehavior = AtkTimelineJumpBehavior.Start, LabelId = 7, JumpLabelId = 0 }},
						new TimelineKeyFrame { FrameIndex = 36, Label = new AtkTimelineLabel{ JumpBehavior = AtkTimelineJumpBehavior.PlayOnce, LabelId = 0, JumpLabelId = 0 }},
						new TimelineKeyFrame { FrameIndex = 37, Label = new AtkTimelineLabel{ JumpBehavior = AtkTimelineJumpBehavior.Start, LabelId = 6, JumpLabelId = 0 }},
						new TimelineKeyFrame { FrameIndex = 46, Label = new AtkTimelineLabel{ JumpBehavior = AtkTimelineJumpBehavior.PlayOnce, LabelId = 0, JumpLabelId = 0 }},
						new TimelineKeyFrame { FrameIndex = 47, Label = new AtkTimelineLabel{ JumpBehavior = AtkTimelineJumpBehavior.Start, LabelId = 4, JumpLabelId = 0 }},
						new TimelineKeyFrame { FrameIndex = 53, Label = new AtkTimelineLabel{ JumpBehavior = AtkTimelineJumpBehavior.PlayOnce, LabelId = 0, JumpLabelId = 0 }},
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
		
		labelNode.AddTimeline(new Timeline {
			Animations = [ 
				new TimelineAnimation {
					StartFrameId = 1, EndFrameId = 10, KeyFrames = [ 
						new TimelineKeyFrame { FrameIndex = 1, Position = new Vector2(16.0f, 3.0f) }, 
						new TimelineKeyFrame { FrameIndex = 1, Alpha = 255 }, 
						new TimelineKeyFrame { FrameIndex = 1, NodeTint = new NodeTint { MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f) } },
					],
				},
				new TimelineAnimation {
					StartFrameId = 11, EndFrameId = 17, KeyFrames = [ 
						new TimelineKeyFrame { FrameIndex = 11, Position = new Vector2(16.0f, 3.0f) }, 
						new TimelineKeyFrame { FrameIndex = 11, Alpha = 255 }, 
						new TimelineKeyFrame { FrameIndex = 11, NodeTint = new NodeTint { MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f) } },
					],
				},
				new TimelineAnimation {
					StartFrameId = 18, EndFrameId = 26, KeyFrames = [
						new TimelineKeyFrame { FrameIndex = 18, Position = new Vector2(16.0f, 4.0f) }, 
						new TimelineKeyFrame { FrameIndex = 18, Alpha = 255 }, 
						new TimelineKeyFrame { FrameIndex = 18, NodeTint = new NodeTint { MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f) } },
					],
				},
				new TimelineAnimation {
					StartFrameId = 27, EndFrameId = 36, KeyFrames = [
						new TimelineKeyFrame { FrameIndex = 27, Position = new Vector2(16.0f, 3.0f) }, 
						new TimelineKeyFrame { FrameIndex = 27, Alpha = 153 }, 
						new TimelineKeyFrame { FrameIndex = 27, NodeTint = new NodeTint { MultiplyColor = new Vector3(80.0f, 80.0f, 80.0f) } },
					],
				},
				new TimelineAnimation {
					StartFrameId = 37, EndFrameId = 46, KeyFrames = [
						new TimelineKeyFrame { FrameIndex = 37, Position = new Vector2(16.0f, 3.0f) }, 
						new TimelineKeyFrame { FrameIndex = 37, Alpha = 255 }, 
						new TimelineKeyFrame { FrameIndex = 37, NodeTint = new NodeTint { MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f) } },
					],
				},
				new TimelineAnimation {
					StartFrameId = 47, EndFrameId = 53, KeyFrames = [
						new TimelineKeyFrame { FrameIndex = 47, Position = new Vector2(16.0f, 3.0f) }, 
						new TimelineKeyFrame { FrameIndex = 47, Alpha = 255 }, 
						new TimelineKeyFrame { FrameIndex = 47, NodeTint = new NodeTint { MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f) } },
					],
				},
			],
		});
	}
}