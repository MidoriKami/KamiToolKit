using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.TimelineBuilding;
using KamiToolKit.Extensions;

namespace KamiToolKit.Nodes.InputText;

public unsafe class TextInputNode : ComponentNode<AtkComponentTextInput, AtkUldComponentDataTextInput> {

	private readonly NineGridNode backgroundNode;
	private readonly NineGridNode focusNode;
	private readonly TextNode textLimitsNode;
	private readonly TextNode currentTextNode;
	private readonly TextInputSelectionListNode selectionListNode;
	private readonly CursorNode cursorNode;
	
	public TextInputNode() {
		backgroundNode = new SimpleNineGridNode {
			NodeId = 19,
			TexturePath = "ui/uld/TextInputA.tex",
			TextureCoordinates = new Vector2(24.0f, 0.0f),
			TextureSize = new Vector2(24.0f, 24.0f),
			NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.AnchorBottom | NodeFlags.AnchorRight | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
			Offsets = new Vector4(10.0f),
			Size = new Vector2(152.0f, 28.0f),
		};
		
		backgroundNode.AttachNode(this);

		focusNode = new SimpleNineGridNode {
			NodeId = 18,
			TexturePath = "ui/uld/TextInputA.tex",
			TextureCoordinates = new Vector2(0.0f, 0.0f),
			TextureSize = new Vector2(24.0f, 24.0f),
			NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.AnchorBottom | NodeFlags.AnchorRight | NodeFlags.Enabled | NodeFlags.EmitsEvents,
			Offsets = new Vector4(10.0f),
			Size = new Vector2(152.0f, 28.0f),
			IsVisible = true,
		};
		
		focusNode.AttachNode(this);

		textLimitsNode = new TextNode {
			NodeId = 17,
			Position = new Vector2(-24.0f, 6.0f),
			Size = new Vector2(170.0f, 19.0f),
			FontType = FontType.MiedingerMed,
			FontSize = 14,
			AlignmentType = (AlignmentType) 21,
			NodeFlags = NodeFlags.AnchorBottom | NodeFlags.AnchorRight | NodeFlags.Enabled | NodeFlags.EmitsEvents,
		};
		
		textLimitsNode.AttachNode(this);

		currentTextNode = new TextNode {
			NodeId = 16,
			Position = new Vector2(10.0f, 6.0f),
			Size = new Vector2(132.0f, 18.0f),
			AlignmentType = AlignmentType.TopLeft,
			NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.AnchorBottom | NodeFlags.AnchorRight | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
			TextFlags = TextFlags.AutoAdjustNodeSize,
		};
		
		currentTextNode.AttachNode(this);

		selectionListNode = new TextInputSelectionListNode {
			NodeId = 4,
			Position = new Vector2(0.0f, 22.0f),
			Size = new Vector2(186.0f, 208.0f),
			NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Enabled | NodeFlags.EmitsEvents,
		};
		
		selectionListNode.AttachNode(this);

		cursorNode = new CursorNode {
			NodeId = 2,
			Position = new Vector2(10.0f, 2.0f),
			Size = new Vector2(4.0f, 24.0f),
			OriginY = 4.0f,
			NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
		};
		
		cursorNode.AttachNode(this);
		
		Data->Nodes[0] = currentTextNode.NodeId;
		Data->Nodes[1] = backgroundNode.NodeId;
		Data->Nodes[2] = cursorNode.NodeId;
		Data->Nodes[3] = selectionListNode.NodeId;
		Data->Nodes[4] = selectionListNode.Buttons[8].NodeId;
		Data->Nodes[5] = selectionListNode.Buttons[7].NodeId;
		Data->Nodes[6] = selectionListNode.Buttons[6].NodeId;
		Data->Nodes[7] = selectionListNode.Buttons[5].NodeId;
		Data->Nodes[8] = selectionListNode.Buttons[4].NodeId;
		Data->Nodes[9] = selectionListNode.Buttons[3].NodeId;
		Data->Nodes[10] = selectionListNode.Buttons[2].NodeId;
		Data->Nodes[11] = selectionListNode.Buttons[1].NodeId;
		Data->Nodes[12] = selectionListNode.Buttons[0].NodeId;
		Data->Nodes[13] = selectionListNode.LabelNode.NodeId;
		Data->Nodes[14] = selectionListNode.BackgroundNode.NodeId;
		Data->Nodes[15] = textLimitsNode.NodeId;

		Data->CandidateColor = new ByteColor { R = 66 };
		Data->IMEColor = new ByteColor { R = 67 };
		Data->FocusColor = KnownColor.Black.Vector().ToByteColor();
		
		Data->MaxWidth = 0;
		Data->MaxLine = 1;
		Data->MaxByte = 0;
		Data->MaxChar = 40;
		Data->Flags1 = 208 + 0xC;
		Data->CharSet = 0;
		Data->Flags2 = 3;

		LoadTimelines();

		InitializeComponentEvents();
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			backgroundNode.Dispose();
			focusNode.Dispose();
			textLimitsNode.Dispose();
			currentTextNode.Dispose();
			selectionListNode.Dispose();
			
			base.Dispose(disposing);
		}
	}
	
	public override float Width {
		get => base.Width;
		set {
			backgroundNode.Width = value;
			focusNode.Width = value;
			textLimitsNode.Width = value + 18.0f;
			currentTextNode.Width = value - 20.0f;
			base.Width = value;
		}
	}

	public override float Height {
		get => base.Height;
		set {
			backgroundNode.Height = value;
			focusNode.Height = value;
			textLimitsNode.Height = value - 9.0f;
			currentTextNode.Height = value - 10.0f;
			base.Height = value;
		}
	}
	
	private void LoadTimelines() {
		AddTimeline(new TimelineBuilder()
			.BeginFrameSet(1, 29)
			.AddLabelPair(1, 9, 17)
			.AddLabelPair(10, 19, 18)
			.AddLabelPair(20, 29,  7)
			.EndFrameSet()
			.Build());
		
		backgroundNode.AddTimeline(new TimelineBuilder()
			.AddFrameSetWithFrame(1, 9, 1, alpha: 255)
			.BeginFrameSet(10, 19)
			.AddFrame(10, alpha: 255)
			.AddFrame(12, alpha: 255)
			.EndFrameSet()
			.AddFrameSetWithFrame(20, 29, 20, alpha: 127)
			.Build());
		
		focusNode.AddTimeline(new TimelineBuilder()
			.BeginFrameSet(10, 19)
			.AddFrame(10, alpha: 0)
			.AddFrame(12, alpha: 255)
			.EndFrameSet()
			.Build());
		
		textLimitsNode.AddTimeline(new TimelineBuilder()
			.AddFrameSetWithFrame(1, 9, 1, alpha: 102)
			.BeginFrameSet(10, 19)
			.AddFrame(10, alpha: 102)
			.AddFrame(12, alpha: 127)
			.EndFrameSet()
			.AddFrameSetWithFrame(20, 29, 20, alpha: 76)
			.Build());
		
		cursorNode.AddTimeline(new TimelineBuilder()
			.BeginFrameSet(1, 19)
			.AddEmptyFrame(1)
			.EndFrameSet()
			.Build());
	}
}