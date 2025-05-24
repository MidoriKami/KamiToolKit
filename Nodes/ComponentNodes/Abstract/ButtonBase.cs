using System;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.NodeParts;
using KamiToolKit.System;

namespace KamiToolKit.Nodes.ComponentNodes.Abstract;

public abstract unsafe class ButtonBase : ComponentNode<AtkComponentButton, AtkUldComponentDataButton> {
	protected readonly NineGridNode BackgroundNode;
	protected abstract NodeBase DecorationNode { get; }

	protected AtkComponentButton* ButtonNode => (AtkComponentButton*) InternalNode;

	protected ButtonBase() {
		SetInternalComponentType(ComponentType.Button);
		Data->Nodes[1] = 2;

		BackgroundNode = new SimpleNineGridNode {
			TexturePath = "ui/uld/ButtonA_hr1.tex",
			IsVisible = true,
			TextureSize = new Vector2(100.0f, 28.0f),
			LeftOffset = 16.0f,
			RightOffset = 16.0f,
			NodeId = 2,
		};
		
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
		
		BackgroundNode.AddTimeline(new Timeline {
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

		BackgroundNode.AttachNode(this, NodePosition.AfterAllSiblings);
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			BackgroundNode.DetachNode();
			BackgroundNode.Dispose();

			NativeMemoryHelper.UiFree(Data);
			Data = null;

			base.Dispose(disposing);
		}
	}

	public override void EnableEvents(IAddonEventManager eventManager, AtkUnitBase* addon) {
		base.EnableEvents(eventManager, addon);
		
		CollisionNode.EnableEvents(eventManager, addon);
	}

	public override void DisableEvents(IAddonEventManager eventManager) {
		base.DisableEvents(eventManager);
		
		CollisionNode.DisableEvents(eventManager);
	}
	
	private Action? InternalOnClick { get; set; }

	public Action? OnClick {
		get => InternalOnClick;
		set {
			if (value is null ) {
				if (InternalOnClick is not null) {
					RemoveEvent(AddonEventType.ButtonClick, InternalOnClick);
					InternalOnClick = null;
				}
			}
			else {
				if (InternalOnClick is not null) {
					AddEvent(AddonEventType.ButtonClick, InternalOnClick);
					AddEvent(AddonEventType.ButtonClick, value);
					InternalOnClick = value;
				}
				else {
					AddEvent(AddonEventType.ButtonClick, value);
					InternalOnClick = value;
				}
			}
		}
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

	public bool BackgroundVisible {
		get => BackgroundNode.IsVisible;
		set => BackgroundNode.IsVisible = value;
	}
}