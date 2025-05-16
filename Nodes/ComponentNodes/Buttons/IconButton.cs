using System.Numerics;
using KamiToolKit.Classes;
using KamiToolKit.System;

namespace KamiToolKit.Nodes.ComponentNodes;

/// <summary>
/// Uses a GameIconId to display that icon as the decorator for the button.
/// </summary>
public class IconButton : ButtonBase {

	protected override NodeBase DecorationNode => ImageNode;
	public readonly IconImageNode ImageNode;

	public IconButton() {
		ImageNode = new IconImageNode {
			IsVisible = true,
			NodeId = 3,
		};
		
		ImageNode.AttachNode(this, NodePosition.AfterAllSiblings);
	}
	
	protected override void Dispose(bool disposing) {
		if (disposing) {
			ImageNode.DetachNode();
			ImageNode.Dispose();
			
			base.Dispose(disposing);
		}
	}

	public uint IconId {
		get => ImageNode.IconId;
		set => ImageNode.IconId = value;
	}

	public float IconPositionX {
		get => ImageNode.X;
		set => ImageNode.X = value;
	}

	public float IconPositionY {
		get => ImageNode.Y;
		set => ImageNode.Y = value;
	}

	public Vector2 IconPosition {
		get => ImageNode.Position;
		set => ImageNode.Position = value;
	}

	public float IconWidth {
		get => ImageNode.Width;
		set => ImageNode.Width = value;
	}

	public float IconHeight {
		get => ImageNode.Height;
		set => ImageNode.Height = value;
	}

	public Vector2 IconSize {
		get => ImageNode.Size;
		set => ImageNode.Size = value;
	}
}