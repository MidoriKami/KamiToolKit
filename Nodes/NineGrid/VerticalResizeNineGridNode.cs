using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Nodes;

public class VerticalResizeNineGridNode : SimpleComponentNode {
	public readonly NineGridNode BorderNode;

	public VerticalResizeNineGridNode() {
		BorderNode = new SimpleNineGridNode {
			TexturePath = "ui/uld/WindowA_line.tex",
			TextureCoordinates = new Vector2(0.0f, 0.0f),
			TextureSize = new Vector2(32.0f, 4.0f),
			LeftOffset = 12,
			RightOffset = 12,
			PartsRenderType = 192,
			DrawFlags = 12,
			NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.AnchorRight | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
		};
		BorderNode.AttachNode(this);
	}

	public bool IsHovered {
		get;
		set {
			field = value;
			if (value) {
				BorderNode.AddColor = new Vector3(100.0f, 100.0f, 100.0f) / 255.0f;
			}
			else {
				BorderNode.AddColor = Vector3.Zero;
			}
		}
	}

	public override float Rotation {
		get => base.Rotation;
		set {
			base.Rotation = value;
			BorderNode.Rotation = value;
			CollisionNode.Rotation = value;
		}
	}

	public override float Height {
		get => base.Height;
		set {
			base.Height = value;
			BorderNode.Width = value;
		}
	}

	public override float Width {
		get => base.Width;
		set {
			base.Width = value;
			BorderNode.Height = value;
		}
	}
}