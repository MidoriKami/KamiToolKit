using System.Numerics;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.System;

namespace KamiToolKit.Nodes.ComponentNodes;

public unsafe class TextButton : ButtonBase {

	protected override NodeBase DecorationNode => labelNode;
	private readonly TextNode labelNode;

	public TextButton() {
		Data->Nodes[0] = 3;

		labelNode = new TextNode {
			IsVisible = true,
			AlignmentType = AlignmentType.Center,
			Position = new Vector2(16.0f, 3.0f),
			NodeID = 3,
		};
		
		labelNode.AttachNode(this, NodePosition.AfterAllSiblings);
		
		Component->AtkComponentBase.InitializeFromComponentData(&Data->AtkUldComponentDataBase);
	}

	public SeString Label {
		get => labelNode.Text;
		set => labelNode.Text = value;
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			labelNode.DetachNode();
			labelNode.Dispose();
			
			base.Dispose(disposing);
		}
	}

	public new float Width {
		get => InternalResNode->Width;
		set {
			InternalResNode->SetWidth((ushort) value);
			BackgroundNode.Width = value;
			DecorationNode.Width = value - BackgroundNode.LeftOffset - BackgroundNode.RightOffset;
			CollisionNode.Width = value;
		}
	}

	public new float Height {
		get => InternalResNode->Height;
		set {
			InternalResNode->SetHeight((ushort) value);
			BackgroundNode.Height = value;
			DecorationNode.Height = value - 8.0f;
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