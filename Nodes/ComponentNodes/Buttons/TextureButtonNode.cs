using System.Numerics;
using KamiToolKit.Classes;
using KamiToolKit.Nodes.ComponentNodes.Abstract;

namespace KamiToolKit.Nodes.ComponentNodes;

public class TextureButtonNode : ButtonBase {
	private readonly SimpleImageNode imageNode;

	public TextureButtonNode() {
		imageNode = new ImGuiImageNode {
			IsVisible = true,
			NodeId = 3,
			WrapMode = 1,
			ImageNodeFlags = 0,
		};
		
		imageNode.AttachNode(this, NodePosition.AfterAllSiblings);
				
		LoadTimelines();
		
		InitializeComponentEvents();
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			imageNode.DetachNode();
			imageNode.Dispose();

			base.Dispose(disposing);
		}
	}

	public string TexturePath {
		get => imageNode.TexturePath;
		set => imageNode.TexturePath = value;
	}

	public Vector2 TextureCoordinates {
		get => imageNode.TextureCoordinates;
		set => imageNode.TextureCoordinates = value;
	}

	public Vector2 TextureSize {
		get => imageNode.TextureSize;
		set => imageNode.TextureSize = value;
	}
	
	public override float Width {
		get => base.Width;
		set {
			imageNode.Width = value;
			base.Width = value;
		}
	}

	public override float Height {
		get => base.Height;
		set {
			imageNode.Height = value;
			base.Height = value;
		}
	}

	private void LoadTimelines()
		=> LoadTwoPartTimelines(this, imageNode);
}