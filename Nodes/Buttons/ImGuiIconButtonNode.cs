using Dalamud.Interface.Textures.TextureWraps;

namespace KamiToolKit.Nodes;

public class ImGuiIconButtonNode : ButtonBase {
	protected readonly ImGuiImageNode ImageNode;

	public ImGuiIconButtonNode() {
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

	public string TexturePath {
		get => ImageNode.TexturePath;
		set => ImageNode.TexturePath = value;
	}

	public void LoadTexture(IDalamudTextureWrap texture)
		=> ImageNode.LoadTexture(texture);
	
	public void LoadTextureFromFile(string path)
		=> ImageNode.LoadTextureFromFile(path);
	
	private void LoadTimelines()
		=> LoadTwoPartTimelines(this, ImageNode);
}