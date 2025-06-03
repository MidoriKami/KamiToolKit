using System;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.NodeParts;
using KamiToolKit.System;

namespace KamiToolKit.Nodes.ComponentNodes.Abstract;

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
		var labelSet = new LabelSetBuilder()
			.AddLabelSet(1, 1, 9)
			.AddLabelSet(2, 10, 19)
			.AddLabelSet(3, 20, 29)
			.AddLabelSet(7, 30, 39)
			.AddLabelSet(6, 40, 49)
			.AddLabelSet(4, 50, 59);
		
		parent.AddTimeline(labelSet.Build());

		var timelineBuilder = new TimelineBuilder(labelSet)
			.AddFrame(1, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 0.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f),
			})
			.AddFrame(10, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 0.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f),
			})
			.AddFrame(12, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 0.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f), AddColor = new Vector3(16.0f, 16.0f, 16.0f),
			})
			.AddFrame(20, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 1.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f), AddColor = new Vector3(16.0f, 16.0f, 16.0f),
			})
			.AddFrame(30, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 0.0f), Alpha = 178, MultiplyColor = new Vector3(50.0f, 50.0f, 50.0f),
			})
			.AddFrame(40, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 0.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f), AddColor = new Vector3(16.0f, 16.0f, 16.0f),
			})
			.AddFrame(50, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 0.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f), AddColor = new Vector3(16.0f, 16.0f, 16.0f),
			})
			.AddFrame(52, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 0.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f),
			});

		foreground.AddTimeline(timelineBuilder.Build());
	}

	protected void LoadThreePartTimelines(NodeBase parent, NodeBase background, NodeBase foreground) {
		var labelSet = new LabelSetBuilder()
			.AddLabelSet(1, 1, 10)
			.AddLabelSet(2, 11, 17)
			.AddLabelSet(3, 18, 26)
			.AddLabelSet(7, 27, 36)
			.AddLabelSet(6, 37, 46)
			.AddLabelSet(4, 47, 53);
		
		parent.AddTimeline(labelSet.Build());
		
		background.AddTimeline(new TimelineBuilder(labelSet)
			.AddFrame(1, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 0.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f),
			})
			.AddFrame(11, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 0.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f),
			})
			.AddFrame(13, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 0.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f), AddColor = new Vector3(16.0f, 16.0f, 16.0f),
			})
			.AddFrame(18, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 1.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f), AddColor = new Vector3(16.0f, 16.0f, 16.0f),
			})
			.AddFrame(27, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 0.0f), Alpha = 178, MultiplyColor = new Vector3(50.0f, 50.0f, 50.0f),
			})
			.AddFrame(37, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 0.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f), AddColor = new Vector3(16.0f, 16.0f, 16.0f),
			})
			.AddFrame(47, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 0.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f), AddColor = new Vector3(16.0f, 16.0f, 16.0f),
			})
			.AddFrame(53, new TimelineKeyFrameSet {
				Position = new Vector2(0.0f, 0.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f),
			})
			.Build()
		);
		
		foreground.AddTimeline(new TimelineBuilder(labelSet)
			.AddFrame(1, new TimelineKeyFrameSet {
				Position = new Vector2(8.0f, 8.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f),
			})
			.AddFrame(11, new TimelineKeyFrameSet {
				Position = new Vector2(8.0f, 8.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f),
			})
			.AddFrame(18, new TimelineKeyFrameSet {
				Position = new Vector2(8.0f, 9.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f),
			})
			.AddFrame(27, new TimelineKeyFrameSet {
				Position = new Vector2(8.0f, 8.0f), Alpha = 153, MultiplyColor = new Vector3(80.0f, 80.0f, 80.0f),
			})
			.AddFrame(37, new TimelineKeyFrameSet {
				Position = new Vector2(8.0f, 8.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f),
			})
			.AddFrame(47, new TimelineKeyFrameSet {
				Position = new Vector2(8.0f, 8.0f), Alpha = 255, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f),
			})
			.Build()
		);
	}
}