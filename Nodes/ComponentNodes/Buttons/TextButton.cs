using System.Numerics;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.NodeParts;
using KamiToolKit.Nodes.ComponentNodes.Abstract;
using KamiToolKit.System;
using Newtonsoft.Json;

namespace KamiToolKit.Nodes.ComponentNodes;

[JsonObject(MemberSerialization.OptIn)]
public unsafe class TextButton : ButtonBase {

	protected override NodeBase DecorationNode => LabelNode;
	public readonly TextNode LabelNode;

	public TextButton() {
		Data->Nodes[0] = 3;

		LabelNode = new TextNode {
			IsVisible = true,
			AlignmentType = AlignmentType.Center,
			Position = new Vector2(16.0f, 3.0f),
			NodeId = 3,
		};
		
		LabelNode.AddTimeline(new Timeline {
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

		AddFlags(NodeFlags.AnchorTop, NodeFlags.AnchorLeft, NodeFlags.Visible, NodeFlags.Enabled, NodeFlags.EmitsEvents);
		LabelNode.AddFlags(NodeFlags.AnchorLeft, NodeFlags.AnchorRight, NodeFlags.Visible, NodeFlags.Enabled, NodeFlags.EmitsEvents);
		
		LabelNode.AttachNode(this, NodePosition.AfterAllSiblings);
		
		InitializeComponentEvents();
		
		LabelNode.InternalResNode->EnableTimeline();
		LabelNode.InternalResNode->Timeline->PlayAnimation(AtkTimelineJumpBehavior.Initialize, 1);
		LabelNode.InternalResNode->Timeline->PlayAnimation(AtkTimelineJumpBehavior.LoopForever, 1);
	}

	public SeString Label {
		get => LabelNode.Text;
		set => LabelNode.Text = value;
	}

	[JsonProperty] public string String {
		get => LabelNode.Text.ToString();
		set => LabelNode.Text = value;
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			LabelNode.DetachNode();
			LabelNode.Dispose();
			
			base.Dispose(disposing);
		}
	}

	public new float Width {
		get => InternalResNode->Width;
		set {
			InternalResNode->SetWidth((ushort) value);
			BackgroundNode.Width = value;
			DecorationNode.Width = value - BackgroundNode.LeftOffset - BackgroundNode.RightOffset;
			CollisionNode.Width = value;
		}
	}

	public new float Height {
		get => InternalResNode->Height;
		set {
			InternalResNode->SetHeight((ushort) value);
			BackgroundNode.Height = value;
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