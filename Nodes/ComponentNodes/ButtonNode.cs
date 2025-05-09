using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes.ComponentNodes;

public unsafe class ButtonNode : ComponentNode<AtkComponentButton, AtkUldComponentDataButton> {

	private NineGridNode backgroundNode;
	private TextNode labelNode;
	
	public ButtonNode() {
		SetInternalComponentType(ComponentType.Button);
		
		backgroundNode = new SimpleNineGridNode {
			TexturePath = "ui/uld/ButtonA_hr1.tex",
			IsVisible = true,
			TextureSize = new Vector2(100.0f, 28.0f),
			LeftOffset = 16.0f,
			RightOffset = 16.0f,
			PartsRenderType = (PartsRenderType)88,
		};

		backgroundNode.AttachNode(CollisionNode, NodePosition.AfterAllSiblings);

		labelNode = new TextNode {
			IsVisible = true,
			Text = "Label",
			AlignmentType = AlignmentType.Center,
			Position = new Vector2(16.0f, 3.0f),
		};
		
		labelNode.AttachNode(CollisionNode, NodePosition.AfterAllSiblings);
		Component->UldManager.UpdateDrawNodeList();
	}

	protected override void Dispose(bool disposing) {
		backgroundNode.Dispose();
		labelNode.Dispose();
		
		base.Dispose(disposing);
	}
	
	public new float Width {
		get => InternalResNode->Width;
		set {
			InternalResNode->SetWidth((ushort) value);
			backgroundNode.Width = value;
			labelNode.Width = value - backgroundNode.LeftOffset - backgroundNode.RightOffset;
			CollisionNode.Width = value;
		}
	}

	public new float Height {
		get => InternalResNode->Height;
		set {
			InternalResNode->SetHeight((ushort) value);
			backgroundNode.Height = value;
			labelNode.Height = value - 8.0f;
			CollisionNode.Height = value;
		}
	}

	public new Vector2 Size {
		get => new(Width, Height);
		set {
			Width = value.X;
			Height = value.Y;
		}
	}
}