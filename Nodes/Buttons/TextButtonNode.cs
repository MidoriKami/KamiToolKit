using System.Numerics;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public unsafe class TextButtonNode : ButtonBase {

	private readonly TextNode labelNode;
	private readonly NineGridNode backgroundNode;

	public TextButtonNode() {
		Data->Nodes[0] = 3;
		Data->Nodes[1] = 2;

		backgroundNode = new SimpleNineGridNode {
			TexturePath = "ui/uld/ButtonA.tex",
			TextureSize = new Vector2(100.0f, 28.0f),
			LeftOffset = 16.0f,
			RightOffset = 16.0f,
			NodeId = 2,
		};

		backgroundNode.AttachNode(this, NodePosition.AfterAllSiblings);
		
		labelNode = new TextNode {
			AlignmentType = AlignmentType.Center,
			Position = new Vector2(16.0f, 3.0f),
			NodeId = 3,
		};
		
		labelNode.AttachNode(this, NodePosition.AfterAllSiblings);
		
		LoadTimelines();
		
		InitializeComponentEvents();
	}

	public SeString Label {
		get => labelNode.Text;
		set => labelNode.Text = value;
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			labelNode.DetachNode();
			labelNode.Dispose();
			
			base.Dispose(disposing);
		}
	}

	public override float Width {
		get => base.Width;
		set {
			backgroundNode.Width = value;
			labelNode.Width = value - backgroundNode.LeftOffset - backgroundNode.RightOffset;
			base.Width = value;
		}
	}

	public override float Height {
		get => base.Height;
		set {
			backgroundNode.Height = value;
			labelNode.Height = value - 8.0f;
			base.Height = value;
		}
	}

	private void LoadTimelines()
		=> LoadThreePartTimelines(this, backgroundNode, labelNode, new Vector2(16.0f, 3.0f));
}