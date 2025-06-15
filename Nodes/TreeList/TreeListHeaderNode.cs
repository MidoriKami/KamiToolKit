using System.Numerics;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public class TreeListHeaderNode : ResNode {

	protected readonly NineGridNode DecorationNode;
	protected readonly TextNode LabelNode;
	
	public TreeListHeaderNode() {
		DecorationNode = new SimpleNineGridNode {
			TexturePath = "ui/uld/journal_Separator.tex",
			TextureCoordinates = new Vector2(0.0f, 0.0f),
			TextureSize = new Vector2(424.0f, 24.0f),
			Size = new Vector2(24.0f, 24.0f),
			IsVisible = true,
			LeftOffset = 25.0f,
			RightOffset = 20.0f,
		};
		
		DecorationNode.AttachNode(this);

		LabelNode = new TextNode {
			Position = new Vector2(22.0f, 1.0f),
			TextColor = ColorHelper.GetColor(7),
			IsVisible = true,
			AlignmentType = AlignmentType.Left,
			FontSize = 12,
			FontType = FontType.Axis,
		};
		
		LabelNode.AttachNode(this);
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			DecorationNode.Dispose();
			LabelNode.Dispose();
			
			base.Dispose(disposing);
		}
	}

	public SeString Label {
		get => LabelNode.Text;
		set => LabelNode.Text = value;
	}

	public override float Width {
		get => base.Width;
		set {
			DecorationNode.Width = value;
			LabelNode.Width = value - 22.0f;
			base.Width = value;
		}
	}

	public override float Height {
		get => base.Height;
		set {
			DecorationNode.Height = value;
			LabelNode.Height = value;
			base.Height = value;
		}
	}
}