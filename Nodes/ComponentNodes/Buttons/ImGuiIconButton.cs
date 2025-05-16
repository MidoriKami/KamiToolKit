using System.Numerics;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Plugin.Services;
using KamiToolKit.Classes;
using KamiToolKit.System;

namespace KamiToolKit.Nodes.ComponentNodes;

public class ImGuiIconButton : ButtonBase {
	protected override NodeBase DecorationNode => imageNode;
	private readonly ImGuiImageNode imageNode;

	public ImGuiIconButton() {
		imageNode = new ImGuiImageNode {
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
	
	public void LoadTexture(ITextureProvider textureProvider, IDalamudTextureWrap texture)
		=> imageNode.LoadTexture(textureProvider, texture);
}