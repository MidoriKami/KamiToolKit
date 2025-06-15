using System.Numerics;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Nodes;

public unsafe class TextButtonNode : ButtonBase {

	protected readonly TextNode LabelNode;
	protected readonly NineGridNode BackgroundNode;

	public TextButtonNode() {
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
	}

	public SeString Label {
		get => LabelNode.Text;
		set => LabelNode.Text = value;
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			LabelNode.Dispose();
			BackgroundNode.Dispose();
			
			base.Dispose(disposing);
		}
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

	private void LoadTimelines()
		=> LoadThreePartTimelines(this, BackgroundNode, LabelNode, new Vector2(16.0f, 3.0f));
}