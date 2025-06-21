using System;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.TimelineBuilding;
using KamiToolKit.System;

namespace KamiToolKit.Nodes;

public abstract unsafe class ButtonBase : ComponentNode<AtkComponentButton, AtkUldComponentDataButton> {

	protected ButtonBase() {
		SetInternalComponentType(ComponentType.Button);
		AddEvent(AddonEventType.ButtonClick, ClickHandler);
	}
	
	public Action? OnClick { get; set; }
	
	private void ClickHandler(AddonEventData data) {
		OnClick?.Invoke();
	}

	public bool IsEnabled {
		get => Component->IsEnabled;
		set => Component->SetEnabledState(value);
	}

	public bool IsChecked {
		get => Component->IsChecked;
		set => Component->SetChecked(value);
	}

	protected void LoadTwoPartTimelines(NodeBase parent, NodeBase foreground) {
		parent.AddTimeline(new TimelineBuilder()
			.BeginFrameSet(1, 59)
			.AddLabelPair(1, 9, 1)
			.AddLabelPair(10, 19, 2)
			.AddLabelPair(20, 29, 3)
			.AddLabelPair(30, 39, 7)
			.AddLabelPair(40, 49, 6)
			.AddLabelPair(50, 59, 4)
			.EndFrameSet()
			.Build());
		
		foreground.AddTimeline(new TimelineBuilder()
			.AddFrameSetWithFrame(1, 9, 1, position: Vector2.Zero, alpha: 255, multiplyColor: new Vector3(100.0f))
			.BeginFrameSet(10, 19)
			.AddFrame(10, position: Vector2.Zero, alpha: 255, multiplyColor: new Vector3(100.0f))
			.AddFrame(12, position: Vector2.Zero, alpha: 255, multiplyColor: new Vector3(100.0f), addColor: new Vector3(16.0f))
			.EndFrameSet()
			.AddFrameSetWithFrame(20, 29, 20, position: new Vector2(0.0f, 1.0f), alpha: 255, multiplyColor: new Vector3(100.0f), addColor: new Vector3(16.0f))
			.AddFrameSetWithFrame(30, 39, 30, position: Vector2.Zero, alpha: 178, multiplyColor: new Vector3(50.0f))
			.AddFrameSetWithFrame(40, 49, 40, position: Vector2.Zero, alpha: 255, multiplyColor: new Vector3(100.0f), addColor: new Vector3(16.0f))
			.BeginFrameSet(50, 59)
			.AddFrame(50, position: Vector2.Zero, alpha: 255, multiplyColor: new Vector3(100.0f), addColor: new Vector3(16.0f))
			.AddFrame(52, position: Vector2.Zero, alpha: 255, multiplyColor: new Vector3(100.0f))
			.EndFrameSet()
			.AddFrameSetWithFrame(130, 139, 130, position: Vector2.Zero, alpha: 255, addColor: new Vector3(16.0f), multiplyColor: new Vector3(100.0f))
			.AddFrameSetWithFrame(140, 149, 140, position: Vector2.Zero, alpha: 255, multiplyColor: new Vector3(100.0f))
			.AddFrameSetWithFrame(150, 159, 150, position: Vector2.Zero, alpha: 255, multiplyColor: new Vector3(100.0f))
			.Build());
	}

	protected void LoadThreePartTimelines(NodeBase parent, NodeBase background, NodeBase foreground, Vector2 foregroundPositionOffset) {
		parent.AddTimeline(new TimelineBuilder()
			.BeginFrameSet(1, 53)
			.AddLabelPair(1, 10, 1)
			.AddLabelPair(11, 17, 2)
			.AddLabelPair(18, 26, 3)
			.AddLabelPair(27, 36, 7)
			.AddLabelPair(37, 46, 6)
			.AddLabelPair(47, 53, 4)
			.EndFrameSet()
			.Build());

		background.AddTimeline(new TimelineBuilder()
			.AddFrameSetWithFrame(1, 10, 1, position: Vector2.Zero, alpha: 255, multiplyColor: new Vector3(100.0f))
			.BeginFrameSet(11, 17)
			.AddFrame(11, position: Vector2.Zero, alpha: 255, multiplyColor: new Vector3(100.0f))
			.AddFrame(13, position: Vector2.Zero, alpha: 255, multiplyColor: new Vector3(100.0f), addColor: new Vector3(16.0f))
			.EndFrameSet()
			.AddFrameSetWithFrame(18, 26,18, position: new Vector2(0.0f, 1.0f), alpha: 255, addColor: new Vector3(16.0f))
			.AddFrameSetWithFrame(27, 36, 27, position: Vector2.Zero, alpha: 178, multiplyColor: new Vector3(50.0f))
			.AddFrameSetWithFrame(37, 46, 37, position: Vector2.Zero, alpha: 255, multiplyColor: new Vector3(100.0f), addColor: new Vector3(16.0f))
			.BeginFrameSet(47, 53)
			.AddFrame(47, position: Vector2.Zero, alpha: 255, multiplyColor: new Vector3(100.0f), addColor: new Vector3(16.0f))
			.AddFrame(53, position: Vector2.Zero, alpha: 255, multiplyColor: new Vector3(100.0f))
			.EndFrameSet()
			.Build());
		
		foreground.AddTimeline(new TimelineBuilder()
			.AddFrameSetWithFrame(1, 10, 1, position: foregroundPositionOffset, alpha: 255, multiplyColor: new Vector3(100.0f))
			.AddFrameSetWithFrame(11, 17, 11, position: foregroundPositionOffset, alpha: 255, multiplyColor: new Vector3(100.0f))
			.AddFrameSetWithFrame(18, 26, 18, position: foregroundPositionOffset + new Vector2(0.0f, 1.0f), alpha: 255, multiplyColor: new Vector3(100.0f))
			.AddFrameSetWithFrame(27, 36, 27, position: foregroundPositionOffset, alpha: 153, multiplyColor: new Vector3(80.0f))
			.AddFrameSetWithFrame(37, 46, 37, position: foregroundPositionOffset, alpha: 255, multiplyColor: new Vector3(100.0f))
			.AddFrameSetWithFrame(47, 53, 47, position: foregroundPositionOffset, alpha: 255, multiplyColor: new Vector3(100.0f))
			.Build());
	}
}