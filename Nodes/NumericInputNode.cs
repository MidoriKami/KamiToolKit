using System;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.TimelineBuilding;

namespace KamiToolKit.Nodes;

public unsafe class NumericInputNode : ComponentNode<AtkComponentNumericInput, AtkUldComponentDataNumericInput> {

	protected readonly NineGridNode BackgroundNode;
	protected readonly ButtonBase AddButton;
	protected readonly ButtonBase SubtractButton;
	protected readonly TextNode ValueTextNode;
	protected readonly NineGridNode FocusBorderNode;
	protected readonly CursorNode CursorNode;
	
	public NumericInputNode() {
		SetInternalComponentType(ComponentType.NumericInput);

		BackgroundNode = new SimpleNineGridNode {
			NodeId = 8,
			Position = new Vector2(0.0f, 1.0f),
			TexturePath = "ui/uld/NumericStepperB.tex",
			TextureCoordinates = new Vector2(56.0f, 0.0f),
			TextureSize = new Vector2(24.0f, 24.0f),
			Height = 24.0f,
			Offsets = new Vector4(10.0f),
			IsVisible = true,
		};

		BackgroundNode.AttachNode(this);

		AddButton = new TextureButtonNode {
			NodeId = 7,
			TexturePath = "ui/uld/NumericStepperB.tex",
			TextureCoordinates = new Vector2(28.0f, 0.0f),
			TextureSize = new Vector2(28.0f, 28.0f),
			Size = new Vector2(28.0f, 28.0f),
			IsVisible = true,
		};
		
		AddButton.AttachNode(this);
		
		SubtractButton = new TextureButtonNode {
			NodeId = 6,
			TexturePath = "ui/uld/NumericStepperB.tex",
			TextureCoordinates = new Vector2(0.0f, 0.0f),
			TextureSize = new Vector2(28.0f, 28.0f),
			Size = new Vector2(28.0f, 28.0f),
			IsVisible = true,
		};
		
		SubtractButton.AttachNode(this);

		ValueTextNode = new TextNode {
			NodeId = 5,
			Position = new Vector2(6.0f, 6.0f),
			FontType = FontType.Axis,
			FontSize = 12,
			AlignmentType = AlignmentType.Top,
			IsVisible = true,
			Text = "999",
		};
		
		ValueTextNode.AttachNode(this);

		FocusBorderNode = new SimpleNineGridNode {
			NodeId = 4,
			TexturePath = "ui/uld/TextInputA.tex",
			TextureCoordinates = new Vector2(0.0f, 0.0f),
			TextureSize = new Vector2(24.0f, 24.0f),
			Position = new Vector2(-3.0f, -2.0f),
			Offsets = new Vector4(10.0f),
		};
		
		FocusBorderNode.AttachNode(this);

		CursorNode = new CursorNode {
			NodeId = 2,
			Size = new Vector2(4.0f, 24.0f),
			IsVisible = true,
			OriginY = 4.0f,
		};
		
		CursorNode.AttachNode(this);

		BuildTimelines();
		
		Data->Nodes[0] = ValueTextNode.NodeId;
		Data->Nodes[1] = 0;
		Data->Nodes[2] = CursorNode.NodeId;
		Data->Nodes[3] = AddButton.NodeId;
		Data->Nodes[4] = SubtractButton.NodeId;

		Data->Max = 99999;
		
		InitializeComponentEvents();
		
		AddEvent(AddonEventType.ValueUpdate, ValueUpdateHandler);
	}

	public override float Width {
		get => base.Width;
		set {
			base.Width = value;
			BackgroundNode.Width = value - 46.0f;
			AddButton.X = value - 50.0f;
			SubtractButton.X = value - 28.0f;
			ValueTextNode.Width = value - 58.0f;
			FocusBorderNode.Width = value - 40.0f;
		}
	}

	public override float Height {
		get => base.Height;
		set {
			base.Height = value;
			ValueTextNode.Height = value / 2.0f;
			FocusBorderNode.Height = value + 4.0f;
		}
	}
	
	// These are likely somewhere in the component, some RE is required
	
	// todo: min number
	
	// todo: max number
	
	// todo: add/subtract amount

	public int Value {
		get => Component->Value;
		set => Component->Value = value;
	}
	
	public Action<int>? OnValueUpdate { get; set; }

	private void ValueUpdateHandler(AddonEventData data) {
		OnValueUpdate?.Invoke(Value);
	}
	
	private void BuildTimelines() {
		AddTimeline(new TimelineBuilder()
				.BeginFrameSet(1, 29)
				.AddLabel(1, 17, AtkTimelineJumpBehavior.Start, 0)
				.AddLabel(9, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
				.AddLabel(10, 18, AtkTimelineJumpBehavior.Start, 0)
				.AddLabel(19, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
				.AddLabel(20, 7, AtkTimelineJumpBehavior.Start, 0)
				.AddLabel(29, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
				.EndFrameSet()
				.Build()
			);
		
		BackgroundNode.AddTimeline(new TimelineBuilder()
				.BeginFrameSet(1, 9)
				.AddFrame(1, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
				.EndFrameSet()
				.BeginFrameSet(10, 19)
				.AddFrame(10, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
				.AddFrame(12, addColor: new Vector3(20, 20, 20), multiplyColor: new Vector3(100, 100, 100))
				.EndFrameSet()
				.BeginFrameSet(20, 29)
				.AddFrame(20, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
				.EndFrameSet()
				.Build()
			);
		
		ValueTextNode.AddTimeline(new TimelineBuilder()
				.BeginFrameSet(1, 19)
				.AddFrame(1, alpha: 255)
				.AddFrame(1, textColor: new Vector3(255.0f, 255.0f, 255.0f) * 255.0f)
				.EndFrameSet()
				.BeginFrameSet(20, 29)
				.AddFrame(20, alpha: 127)
				.AddFrame(20, textColor: new Vector3(255.0f, 255.0f, 255.0f) * 255.0f)
				.EndFrameSet()
				.Build()
			);
		
		FocusBorderNode.AddTimeline(new TimelineBuilder()
				.BeginFrameSet(10, 19)
				.AddFrame(10, alpha: 0)
				.AddFrame(12, alpha: 255)
				.EndFrameSet()
				.Build()
			);
		
		CursorNode.AddTimeline(new TimelineBuilder()
				.BeginFrameSet(1, 15)
				.AddLabel(1, 101, AtkTimelineJumpBehavior.Start, 0)
				.AddLabel(15, 0, AtkTimelineJumpBehavior.LoopForever, 101)
				.EndFrameSet()
				.BeginFrameSet(1, 19)
				.AddEmptyFrame(1)
				.EndFrameSet()
				.Build()
			);
	}
}