using System;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.TimelineBuilding;

namespace KamiToolKit.Nodes;

public unsafe class TextButtonNode : ComponentNode<AtkComponentButton, AtkUldComponentDataButton> {

	protected readonly TextNode LabelNode;
	protected readonly NineGridNode BackgroundNode;

	public TextButtonNode() {
		SetInternalComponentType(ComponentType.Button);
		
		Data->Nodes[0] = 3;
		Data->Nodes[1] = 2;

		BackgroundNode = new SimpleNineGridNode {
			TexturePath = "ui/uld/ButtonA.tex",
			TextureSize = new Vector2(100.0f, 28.0f),
			LeftOffset = 16.0f,
			RightOffset = 16.0f,
			NodeId = 2,
		};

		BackgroundNode.AttachNode(this);
		
		LabelNode = new TextNode {
			AlignmentType = AlignmentType.Center,
			Position = new Vector2(16.0f, 3.0f),
			NodeId = 3,
		};
		
		LabelNode.AttachNode(this);
		
		LoadTimelines();
		
		InitializeComponentEvents();
		
		AddEvent(AddonEventType.MouseClick, ClickHandler);
	}

	public SeString Label {
		get => LabelNode.Text;
		set => LabelNode.Text = value;
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			LabelNode.DetachNode();
			LabelNode.Dispose();
			
			base.Dispose(disposing);
		}
	}
	
	public Action? OnClick { get; set; }

	private void ClickHandler() {
		OnClick?.Invoke();
	}

	public override float Width {
		get => base.Width;
		set {
			BackgroundNode.Width = value;
			LabelNode.Width = value - BackgroundNode.LeftOffset - BackgroundNode.RightOffset;
			base.Width = value;
		}
	}

	public override float Height {
		get => base.Height;
		set {
			BackgroundNode.Height = value;
			LabelNode.Height = value - 8.0f;
			base.Height = value;
		}
	}

	private void LoadTimelines() {
		AddTimeline(new TimelineBuilder()
				.BeginFrameSet(1, 53)
				.AddLabel(1, 1, AtkTimelineJumpBehavior.Start, 0)
				.AddLabel(10, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
				.AddLabel(11, 2, AtkTimelineJumpBehavior.Start, 0)
				.AddLabel(17, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
				.AddLabel(18, 3, AtkTimelineJumpBehavior.Start, 0)
				.AddLabel(26, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
				.AddLabel(27, 7, AtkTimelineJumpBehavior.Start, 0)
				.AddLabel(36, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
				.AddLabel(37, 6, AtkTimelineJumpBehavior.Start, 0)
				.AddLabel(46, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
				.AddLabel(47, 4, AtkTimelineJumpBehavior.Start, 0)
				.AddLabel(53, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
				.EndFrameSet()
				.Build()
			);
		
		BackgroundNode.AddTimeline(new TimelineBuilder()
				.BeginFrameSet(1, 10)
				.AddFrame(1, position: new Vector2(0,0))
				.AddFrame(1, alpha: 255)
				.AddFrame(1, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
				.EndFrameSet()
				.BeginFrameSet(11, 17)
				.AddFrame(11, position: new Vector2(0,0))
				.AddFrame(13, position: new Vector2(0,0))
				.AddFrame(11, alpha: 255)
				.AddFrame(13, alpha: 255)
				.AddFrame(11, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
				.AddFrame(13, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
				.EndFrameSet()
				.BeginFrameSet(18, 26)
				.AddFrame(18, position: new Vector2(0,1))
				.AddFrame(18, alpha: 255)
				.AddFrame(18, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
				.EndFrameSet()
				.BeginFrameSet(27, 36)
				.AddFrame(27, position: new Vector2(0,0))
				.AddFrame(27, alpha: 178)
				.AddFrame(27, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(50, 50, 50))
				.EndFrameSet()
				.BeginFrameSet(37, 46)
				.AddFrame(37, position: new Vector2(0,0))
				.AddFrame(37, alpha: 255)
				.AddFrame(37, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
				.EndFrameSet()
				.BeginFrameSet(47, 53)
				.AddFrame(47, position: new Vector2(0,0))
				.AddFrame(53, position: new Vector2(0,0))
				.AddFrame(47, alpha: 255)
				.AddFrame(53, alpha: 255)
				.AddFrame(47, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
				.AddFrame(53, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
				.EndFrameSet()
				.Build()
			);
		
		LabelNode.AddTimeline(new TimelineBuilder()
				.BeginFrameSet(1, 10)
				.AddFrame(1, position: new Vector2(16,3))
				.AddFrame(1, alpha: 255)
				.AddFrame(1, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
				.EndFrameSet()
				.BeginFrameSet(11, 17)
				.AddFrame(11, position: new Vector2(16,3))
				.AddFrame(11, alpha: 255)
				.AddFrame(11, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
				.EndFrameSet()
				.BeginFrameSet(18, 26)
				.AddFrame(18, position: new Vector2(16,4))
				.AddFrame(18, alpha: 255)
				.AddFrame(18, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
				.EndFrameSet()
				.BeginFrameSet(27, 36)
				.AddFrame(27, position: new Vector2(16,3))
				.AddFrame(27, alpha: 153)
				.AddFrame(27, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(80, 80, 80))
				.EndFrameSet()
				.BeginFrameSet(37, 46)
				.AddFrame(37, position: new Vector2(16,3))
				.AddFrame(37, alpha: 255)
				.AddFrame(37, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
				.EndFrameSet()
				.BeginFrameSet(47, 53)
				.AddFrame(47, position: new Vector2(16,3))
				.AddFrame(47, alpha: 255)
				.AddFrame(47, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
				.EndFrameSet()
				.Build()
			);
	}
}