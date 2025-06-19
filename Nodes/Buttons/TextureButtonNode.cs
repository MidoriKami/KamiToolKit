using System.Numerics;

namespace KamiToolKit.Nodes;

public class TextureButtonNode : ButtonBase {
	protected readonly SimpleImageNode ImageNode;

	public TextureButtonNode() {
		ImageNode = new ImGuiImageNode {
			IsVisible = true,
			NodeId = 3,
			WrapMode = 2,
			ImageNodeFlags = 0,
		};
		
		ImageNode.AttachNode(this);
				
		LoadTimelines();
		
		InitializeComponentEvents();
	}

	public string TexturePath {
		get => ImageNode.TexturePath;
		set => ImageNode.TexturePath = value;
	}

	public Vector2 TextureCoordinates {
		get => ImageNode.TextureCoordinates;
		set => ImageNode.TextureCoordinates = value;
	}

	public Vector2 TextureSize {
		get => ImageNode.TextureSize;
		set => ImageNode.TextureSize = value;
	}
	
	public override float Width {
		get => base.Width;
		set {
			ImageNode.Width = value;
			base.Width = value;
		}
	}

	public override float Height {
		get => base.Height;
		set {
			ImageNode.Height = value;
			base.Height = value;
		}
	}

	private void LoadTimelines()
		=> LoadTwoPartTimelines(this, ImageNode);
}