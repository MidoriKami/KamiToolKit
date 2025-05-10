using System.Numerics;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public class IconButton : ButtonBaseComponent {

	protected override NodeBase DecorationNode => imageNode;
	private readonly IconImageNode imageNode;

	public IconButton() {
		imageNode = new IconImageNode {
			IsVisible = true,
			IconId = 60071,
			Position = new Vector2(16.0f, 3.0f),
			Size = new Vector2(24.0f, 24.0f),
			NodeID = 3,
		};
		
		imageNode.AttachNode(this, NodePosition.AfterAllSiblings);
	}

	public uint IconId {
		get => imageNode.IconId;
		set => imageNode.IconId = value;
	}
}