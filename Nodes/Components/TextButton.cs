using System.Numerics;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public unsafe class TextButton : ButtonBaseComponent {

	protected override NodeBase DecorationNode => labelNode;
	private readonly TextNode labelNode;

	public TextButton() {
		labelNode = new TextNode {
			IsVisible = true,
			Text = "uwu",
			AlignmentType = AlignmentType.Center,
			Position = new Vector2(16.0f, 3.0f),
			NodeID = 3,
		};
		
		labelNode.AttachNode(CollisionNode, NodePosition.AfterAllSiblings);
		Component->UldManager.UpdateDrawNodeList();
	}

	public SeString Label {
		get => labelNode.Text;
		set => labelNode.Text = value;
	}
}