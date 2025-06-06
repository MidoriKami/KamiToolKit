using System.Numerics;

namespace KamiToolKit.Nodes;

/// <summary>
/// Uses a GameIconId to display that icon as the decorator for the button.
/// </summary>
public class IconButtonNode : ButtonBase {
	private readonly IconImageNode imageNode;
	private readonly NineGridNode backgroundNode;

	public IconButtonNode() {
		backgroundNode = new SimpleNineGridNode {
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

		backgroundNode.AttachNode(this);
		
		imageNode = new IconImageNode {
			IsVisible = true,
			TextureSize = new Vector2(32.0f, 32.0f),
			NodeId = 3,
		};
		
		imageNode.AttachNode(this);

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
	
	public uint IconId {
		get => imageNode.IconId;
		set => imageNode.IconId = value;
	}

	public override float Width {
		get => base.Width;
		set {
			imageNode.Width = value - 16.0f;
			imageNode.Position = imageNode.Position with { X = backgroundNode.Position.X + backgroundNode.LeftOffset };
			backgroundNode.Width = value;
			base.Width = value;
		}
	}

	public override float Height {
		get => base.Height;
		set {
			imageNode.Height = value - 16.0f;
			imageNode.Position = imageNode.Position with { Y = backgroundNode.Position.Y + backgroundNode.TopOffset };
			backgroundNode.Height = value;
			base.Height = value;
		}
	}

	private void LoadTimelines()
		=> LoadThreePartTimelines(this, backgroundNode, imageNode, new Vector2(8.0f, 8.0f));
}