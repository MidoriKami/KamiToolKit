using System.Numerics;

namespace KamiToolKit.Nodes;

/// <summary>
/// Uses a GameIconId to display that icon as the decorator for the button.
/// </summary>
public class IconButtonNode : ButtonBase {
	protected readonly IconImageNode ImageNode;
	protected readonly NineGridNode BackgroundNode;

	public IconButtonNode() {
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
		
		ImageNode = new IconImageNode {
			IsVisible = true,
			TextureSize = new Vector2(32.0f, 32.0f),
			NodeId = 3,
		};
		
		ImageNode.AttachNode(this);

		LoadTimelines();
		
		InitializeComponentEvents();
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			ImageNode.Dispose();
			
			base.Dispose(disposing);
		}
	}
	
	public uint IconId {
		get => ImageNode.IconId;
		set => ImageNode.IconId = value;
	}

	public override float Width {
		get => base.Width;
		set {
			ImageNode.Width = value - 16.0f;
			ImageNode.Position = ImageNode.Position with { X = BackgroundNode.Position.X + BackgroundNode.LeftOffset };
			BackgroundNode.Width = value;
			base.Width = value;
		}
	}

	public override float Height {
		get => base.Height;
		set {
			ImageNode.Height = value - 16.0f;
			ImageNode.Position = ImageNode.Position with { Y = BackgroundNode.Position.Y + BackgroundNode.TopOffset };
			BackgroundNode.Height = value;
			base.Height = value;
		}
	}

	private void LoadTimelines()
		=> LoadThreePartTimelines(this, BackgroundNode, ImageNode, new Vector2(8.0f, 8.0f));
}