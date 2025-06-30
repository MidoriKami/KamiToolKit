using System.Numerics;
using Dalamud.Interface.Textures.TextureWraps;

namespace KamiToolKit.Nodes;

public class ImGuiIconButtonNode : ButtonBase {
	public readonly NineGridNode BackgroundNode;
	public readonly ImGuiImageNode ImageNode;

	public ImGuiIconButtonNode() {
		BackgroundNode = new SimpleNineGridNode {
			TexturePath = "ui/uld/BgParts.tex",
			TextureSize = new Vector2(32.0f, 32.0f),
			TextureCoordinates = new Vector2(33.0f, 65.0f),
			TopOffset = 8.0f,
			LeftOffset = 8.0f,
			RightOffset = 8.0f,
			BottomOffset = 8.0f,
			NodeId = 2,
			IsVisible = true,
		};
		BackgroundNode.AttachNode(this);
		
		ImageNode = new ImGuiImageNode {
			IsVisible = true,
			NodeId = 3,
		};
		
		ImageNode.AttachNode(this);
				
		LoadTimelines();
		
		InitializeComponentEvents();
	}

	public override float Width {
		get => base.Width;
		set {
			base.Width = value;
			ImageNode.Width = value - 16.0f;
			ImageNode.Position = ImageNode.Position with { X = BackgroundNode.Position.X + BackgroundNode.LeftOffset };
			BackgroundNode.Width = value;
		}
	}

	public override float Height {
		get => base.Height;
		set {
			base.Height = value;
			ImageNode.Height = value - 16.0f;
			ImageNode.Position = ImageNode.Position with { Y = BackgroundNode.Position.Y + BackgroundNode.TopOffset };
			BackgroundNode.Height = value;
		}
	}

	public bool ShowBackground {
		get => BackgroundNode.IsVisible;
		set => BackgroundNode.IsVisible = value;
	}

	public string TexturePath {
		get => ImageNode.TexturePath;
		set => ImageNode.TexturePath = value;
	}

	public void LoadTexture(IDalamudTextureWrap texture)
		=> ImageNode.LoadTexture(texture);
	
	public void LoadTextureFromFile(string path)
		=> ImageNode.LoadTextureFromFile(path);
	
	private void LoadTimelines()
		=> LoadThreePartTimelines(this, BackgroundNode, ImageNode, new Vector2(8.0f, 8.0f));

}