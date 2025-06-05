using System;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.NodeParts;
using KamiToolKit.System;

namespace KamiToolKit.Nodes;

public abstract unsafe class ButtonBase : ComponentNode<AtkComponentButton, AtkUldComponentDataButton> {

	protected AtkComponentButton* ButtonNode => (AtkComponentButton*) InternalNode;

	protected ButtonBase() => SetInternalComponentType(ComponentType.Button);

	protected override void Dispose(bool disposing) {
		if (disposing) {
			NativeMemoryHelper.UiFree(Data);
			Data = null;

			base.Dispose(disposing);
		}
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
	
	public void SetEnabled(bool enable)
		=> ComponentBase->SetEnabledState(enable);

	public void Disable()
		=> ComponentBase->SetEnabledState(false);

	public void Enable()
		=> ComponentBase->SetEnabledState(true);

	public void Toggle()
		=> ComponentBase->SetEnabledState(!Component->IsEnabled);

	protected void LoadTwoPartTimelines(NodeBase parent, NodeBase foreground) {
		var timelineBuilder = new TimelineBuilder()
			.AddLabelSetPair(1, 1, 9)
			.AddLabelSetPair(2, 10, 19)
			.AddLabelSetPair(3, 20, 29)
			.AddLabelSetPair(7, 30, 39)
			.AddLabelSetPair(6, 40, 49)
			.AddLabelSetPair(4, 50, 59);
		
		parent.AddTimeline(timelineBuilder.BuildLabelSets());

		foreground.AddTimeline(timelineBuilder
			.AddAnimation(1, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 0.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f),
			})
			.AddAnimation(10, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 0.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f),
			})
			.AddAnimation(12, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 0.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f), AddColor = new Vector3(16.0f, 16.0f, 16.0f),
			})
			.AddAnimation(20, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 1.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f), AddColor = new Vector3(16.0f, 16.0f, 16.0f),
			})
			.AddAnimation(30, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 0.0f), Alpha = 178, MultiplyColor = new Vector3(50.0f, 50.0f, 50.0f),
			})
			.AddAnimation(40, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 0.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f), AddColor = new Vector3(16.0f, 16.0f, 16.0f),
			})
			.AddAnimation(50, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 0.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f), AddColor = new Vector3(16.0f, 16.0f, 16.0f),
			})
			.AddAnimation(52, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 0.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f),
			})
			.BuildAnimations());
	}

	protected void LoadThreePartTimelines(NodeBase parent, NodeBase background, NodeBase foreground) {
		var timelineBuilder = new TimelineBuilder()
			.AddLabelSetPair(1, 1, 10)
			.AddLabelSetPair(2, 11, 17)
			.AddLabelSetPair(3, 18, 26)
			.AddLabelSetPair(7, 27, 36)
			.AddLabelSetPair(6, 37, 46)
			.AddLabelSetPair(4, 47, 53);
		
		parent.AddTimeline(timelineBuilder.BuildLabelSets());
		
		background.AddTimeline(timelineBuilder
			.AddAnimation(1, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 0.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f),
			})
			.AddAnimation(11, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 0.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f),
			})
			.AddAnimation(13, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 0.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f), AddColor = new Vector3(16.0f, 16.0f, 16.0f),
			})
			.AddAnimation(18, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 1.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f), AddColor = new Vector3(16.0f, 16.0f, 16.0f),
			})
			.AddAnimation(27, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 0.0f), Alpha = 178, MultiplyColor = new Vector3(50.0f, 50.0f, 50.0f),
			})
			.AddAnimation(37, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 0.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f), AddColor = new Vector3(16.0f, 16.0f, 16.0f),
			})
			.AddAnimation(47, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 0.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f), AddColor = new Vector3(16.0f, 16.0f, 16.0f),
			})
			.AddAnimation(53, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 0.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f),
			})
			.BuildAnimations(true)
		);
		
		foreground.AddTimeline(timelineBuilder
			.AddAnimation(1, new TimelineKeyFrameSet {
				Position = new Vector2(8.0f, 8.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f),
			})
			.AddAnimation(11, new TimelineKeyFrameSet {
				Position = new Vector2(8.0f, 8.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f),
			})
			.AddAnimation(18, new TimelineKeyFrameSet {
				Position = new Vector2(8.0f, 9.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f),
			})
			.AddAnimation(27, new TimelineKeyFrameSet {
				Position = new Vector2(8.0f, 8.0f), Alpha = 153, MultiplyColor = new Vector3(80.0f, 80.0f, 80.0f),
			})
			.AddAnimation(37, new TimelineKeyFrameSet {
				Position = new Vector2(8.0f, 8.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f),
			})
			.AddAnimation(47, new TimelineKeyFrameSet {
				Position = new Vector2(8.0f, 8.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f),
			})
			.BuildAnimations()
		);
	}
}