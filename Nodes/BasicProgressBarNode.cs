using System.Numerics;

namespace KamiToolKit.Nodes;

public class BasicProgressBarNode : ResNode {
	private readonly NineGridNode backgroundNode;
	private readonly NineGridNode foregroundNode;

	public BasicProgressBarNode() {
		backgroundNode = new SimpleNineGridNode {
			TexturePath = "ui/uld/ToDoList.tex",
			TextureCoordinates = new Vector2(108.0f, 8.0f),
			TextureSize = new Vector2(44.0f, 12.0f),
			IsVisible = true,
			LeftOffset = 6,
			RightOffset = 6,
		};
		
		backgroundNode.AttachNode(this);

		foregroundNode = new SimpleNineGridNode {
			TexturePath = "ui/uld/ToDoList.tex", 
			TextureCoordinates = new Vector2(112.0f, 0.0f), 
			TextureSize = new Vector2(40.0f, 8.0f),
			IsVisible = true,
			LeftOffset = 4,
			RightOffset = 4,
		};

		foregroundNode.AttachNode(this);
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			backgroundNode.Dispose();
			foregroundNode.Dispose();
			
			base.Dispose(disposing);
		}
	}
	
	public override float Width {
		get => base.Width;
		set {
			backgroundNode.Width = value;
			foregroundNode.Width = value;
			base.Width = value;
		}
	}

	public override float Height {
		get => base.Height;
		set {
			backgroundNode.Height = value;
			foregroundNode.Height = value;
			base.Height = value;
		}
	}
	
	public Vector4 BackgroundColor {
		get => backgroundNode.Color;
		set => backgroundNode.Color = value;
	}

	public Vector4 BarColor {
		get => foregroundNode.Color;
		set => foregroundNode.Color = value;
	}
	
	public float Progress {
		get => foregroundNode.Width / Width;
		set => foregroundNode.Width = Width * value;
	}
}