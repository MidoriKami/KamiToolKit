using System.Numerics;
using KamiToolKit.Classes;
using KamiToolKit.System;

namespace KamiToolKit.Nodes.ComponentNodes;

/// <summary>
/// Uses a GameIconId to display that icon as the decorator for the button.
/// </summary>
public class IconButton : ButtonBase {

	protected override NodeBase DecorationNode => imageNode;
	private readonly IconImageNode imageNode;

	public IconButton() {
		imageNode = new IconImageNode {
			IsVisible = true,
			NodeID = 3,
		};
		
		imageNode.AttachNode(this, NodePosition.AfterAllSiblings);
	}
	
	protected override void Dispose(bool disposing) {
		if (disposing) {
			imageNode.DetachNode();
			imageNode.Dispose();
			
			base.Dispose(disposing);
		}
	}

	public uint IconId {
		get => imageNode.IconId;
		set => imageNode.IconId = value;
	}

	public float IconPositionX {
		get => imageNode.X;
		set => imageNode.X = value;
	}

	public float IconPositionY {
		get => imageNode.Y;
		set => imageNode.Y = value;
	}

	public Vector2 IconPosition {
		get => imageNode.Position;
		set => imageNode.Position = value;
	}

	public float IconWidth {
		get => imageNode.Width;
		set => imageNode.Width = value;
	}

	public float IconHeight {
		get => imageNode.Height;
		set => imageNode.Height = value;
	}

	public Vector2 IconSize {
		get => imageNode.Size;
		set => imageNode.Size = value;
	}
}