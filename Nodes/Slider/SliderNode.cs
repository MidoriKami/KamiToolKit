using System.Numerics;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.TimelineBuilding;

namespace KamiToolKit.Nodes.Slider;

public unsafe class SliderNode : ComponentNode<AtkComponentSlider, AtkUldComponentDataSlider> {

	protected readonly SliderBackgroundButtonNode SliderBackgroundButtonNode;
	protected readonly NineGridNode ProgressTextureNode;
	protected readonly SliderForegroundButtonNode SliderForegroundButtonNode;
	protected readonly TextNode ValueNode;
	
	public SliderNode() {
		SetInternalComponentType(ComponentType.Slider);
		
		SliderBackgroundButtonNode = new SliderBackgroundButtonNode {
			NodeId = 5,
			IsVisible = true,
			Position = new Vector2(0.0f, 4.0f),
		};

		SliderBackgroundButtonNode.AttachNode(this);

		ProgressTextureNode = new SimpleNineGridNode {
			NodeId = 4,
			TexturePath = "ui/uld/SliderGaugeHorizontalA.tex",
			TextureCoordinates = new Vector2(16.0f, 8.0f),
			TextureSize = new Vector2(40.0f, 7.0f),
			Height = 7.0f,
			Y = 4.0f,
			LeftOffset = 8,
			RightOffset = 8,
			IsVisible = true,
		};
		
		ProgressTextureNode.AttachNode(this);

		SliderForegroundButtonNode = new SliderForegroundButtonNode {
			NodeId = 3,
			Size = new Vector2(16.0f, 16.0f), 
			IsVisible = true,
		};
		
		SliderForegroundButtonNode.AttachNode(this);

		ValueNode = new TextNode {
			NodeId = 2,
			Size = new Vector2(24.0f, 16.0f),
			IsVisible = true,
			FontType = FontType.Axis,
			FontSize = 12,
			AlignmentType = AlignmentType.Center,
		};
		
		ValueNode.AttachNode(this);

		Data->Step = 1;
		Data->Min = 0;
		Data->Max = 100;
		Data->OfffsetL = 4;
		Data->OffsetR = 28;

		Data->Nodes[0] = ProgressTextureNode.NodeId;
		Data->Nodes[1] = SliderForegroundButtonNode.NodeId;
		Data->Nodes[2] = ValueNode.NodeId;
		Data->Nodes[3] = SliderBackgroundButtonNode.NodeId;
		
		BuildTimelines();
		
		InitializeComponentEvents();
		
		Marshal.WriteInt16((nint)Component, 0xFC, 220);
		Marshal.WriteInt16((nint)Component, 0xFA, 28);
		Marshal.WriteInt16((nint)Component, 0xF8, 4);
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			SliderBackgroundButtonNode.Dispose();
			ProgressTextureNode.Dispose();
			SliderForegroundButtonNode.Dispose();
			ValueNode.Dispose();
			
			base.Dispose(disposing);
		}
	}

	public override float Width {
		get => base.Width;
		set {
			base.Width = value;
			SliderBackgroundButtonNode.Width = value - 24.0f;
			ProgressTextureNode.Width = value;
			ValueNode.X = value - 24.0f;
			Marshal.WriteInt16((nint)Component, 0xFC, (short) Width);
		}
	}

	public override float Height {
		get => base.Height;
		set {
			base.Height = value;
			SliderBackgroundButtonNode.Height = value / 2.0f;
			ProgressTextureNode.Height = value / 2.0f - 1.0f;
		}
	}

	public required int Min {
		get => Component->MinValue;
		set => Component->SetMinValue(value);
	}

	public required int Max {
		get => Component->MaxValue;
		set => Component->SetMaxValue(value);
	}

	public int Step {
		get => Component->Steps;
		set => Component->Steps = value;
	}

	public int Value {
		get => Component->Value;
		set => Component->Value = value;
	}
	
	private void BuildTimelines() {
		AddTimeline(new TimelineBuilder()
				.BeginFrameSet(1, 30)
				.AddLabel(1, 17, AtkTimelineJumpBehavior.PlayOnce, 0)
				.AddLabel(11, 18, AtkTimelineJumpBehavior.PlayOnce, 0)
				.AddLabel(21, 7, AtkTimelineJumpBehavior.PlayOnce, 0)
				.EndFrameSet()
				.Build()
			);
		
		ProgressTextureNode.AddTimeline(new TimelineBuilder()
				.BeginFrameSet(1, 20)
				.AddFrame(1, alpha: 255)
				.EndFrameSet()
				.BeginFrameSet(21, 30)
				.AddFrame(21, alpha: 127)
				.EndFrameSet()
				.Build()
			);
		
		ValueNode.AddTimeline(new TimelineBuilder()
				.BeginFrameSet(1, 20)
				.AddFrame(1, alpha: 255)
				.EndFrameSet()
				.BeginFrameSet(21, 30)
				.AddFrame(21, alpha: 153)
				.EndFrameSet()
				.Build()
			);
	}
}