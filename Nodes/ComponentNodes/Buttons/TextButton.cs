using System.Numerics;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes.ComponentNodes.Abstract;
using KamiToolKit.System;
using Newtonsoft.Json;

namespace KamiToolKit.Nodes.ComponentNodes;

[JsonObject(MemberSerialization.OptIn)]
public unsafe class TextButton : ButtonBase {

	protected override NodeBase DecorationNode => LabelNode;
	public readonly TextNode LabelNode;

	public TextButton() {
		Data->Nodes[0] = 3;

		LabelNode = new TextNode {
			IsVisible = true,
			AlignmentType = AlignmentType.Center,
			Position = new Vector2(16.0f, 3.0f),
			NodeId = 3,
		};
		
		LabelNode.AttachNode(this, NodePosition.AfterAllSiblings);
		
		InitializeComponentEvents();
	}

	public SeString Label {
		get => LabelNode.Text;
		set => LabelNode.Text = value;
	}

	[JsonProperty] public string String {
		get => LabelNode.Text.ToString();
		set => LabelNode.Text = value;
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			LabelNode.DetachNode();
			LabelNode.Dispose();
			
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