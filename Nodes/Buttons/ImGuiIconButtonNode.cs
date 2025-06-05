using Dalamud.Interface.Textures.TextureWraps;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public class ImGuiIconButtonNode : ButtonBase {
	private readonly ImGuiImageNode imageNode;

	public ImGuiIconButtonNode() {
		imageNode = new ImGuiImageNode {
			IsVisible = true,
			NodeId = 3,
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

	public void LoadTexture(IDalamudTextureWrap texture)
		=> imageNode.LoadTexture(texture);
	
	public void LoadTextureFromFile(string path)
		=> imageNode.LoadTextureFromFile(path);
	
	private void LoadTimelines()
		=> LoadTwoPartTimelines(this, imageNode);
}