using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes.ComponentNodes.Window;

internal class WindowHeaderNode : ResNode {

	internal TextureButtonNode CloseButtonNode;
	internal TextureButtonNode ConfigurationButtonNode;
	internal TextureButtonNode InformationButtonNode;
	internal TextNode SubtitleNode;
	internal TextNode TitleNode;
	
	public WindowHeaderNode() {
		CloseButtonNode = new TextureButtonNode {
			NodeId = 7,
			Size = new Vector2(28.0f, 28.0f),
			Position = new Vector2(449.0f, 6.0f),
			NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorRight | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
			TexturePath = "ui/uld/WindowA_Button_hr1.tex",
			TextureCoordinates = new Vector2(0.0f, 0.0f),
			TextureSize = new Vector2(28.0f, 28.0f),
		};
		
		CloseButtonNode.AttachNode(this, NodePosition.AsLastChild);

		ConfigurationButtonNode = new TextureButtonNode {
			NodeId = 6,
			Size = new Vector2(16.0f, 16.0f),
			Position = new Vector2(435.0f, 8.0f),
			NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorRight | NodeFlags.Enabled | NodeFlags.EmitsEvents,
			TexturePath = "ui/uld/WindowA_Button_hr1.tex",
			TextureCoordinates = new Vector2(44.0f, 0.0f),
			TextureSize = new Vector2(16.0f, 16.0f),
			IsVisible = true,
		};
		
		ConfigurationButtonNode.AttachNode(this, NodePosition.AsLastChild);
		
		InformationButtonNode = new TextureButtonNode {
			NodeId = 5,
			Size = new Vector2(16.0f, 16.0f),
			Position = new Vector2(421.0f, 8.0f),
			NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorRight | NodeFlags.Enabled | NodeFlags.EmitsEvents,
			TexturePath = "ui/uld/WindowA_Button_hr1.tex",
			TextureCoordinates = new Vector2(28.0f, 0.0f),
			TextureSize = new Vector2(16.0f, 16.0f),
			IsVisible = true,
		};
		
		InformationButtonNode.AttachNode(this, NodePosition.AsLastChild);

		SubtitleNode = new TextNode {
			NodeId = 4,
			LineSpacing = 12,
			AlignmentType = AlignmentType.Left,
			FontSize = 12,
			TextFlags = TextFlags.Emboss,
			FontType = FontType.Axis,
			NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
			TextColor = new Vector4(238.0f, 225.0f, 197.0f, 255.0f).NormalizeToUnitRange(),
			TextOutlineColor = new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
			BackgroundColor = Vector4.Zero,
			Size = new Vector2(46.0f, 20.0f),
			Position = new Vector2(83.0f, 17.0f),
		};
		
		SubtitleNode.AttachNode(this, NodePosition.AsLastChild);

		TitleNode = new TextNode {
			NodeId = 3,
			LineSpacing = 23,
			AlignmentType = AlignmentType.Left,
			FontSize = 23,
			TextFlags = TextFlags.Emboss,
			FontType = FontType.TrumpGothic,
			NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
			TextColor = new Vector4(204.0f, 204.0f, 204.0f, 255.0f).NormalizeToUnitRange(),
			TextOutlineColor = new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
			BackgroundColor = Vector4.Zero,
			Size = new Vector2(86.0f, 31.0f),
			Position = new Vector2(12.0f, 7.0f),
		};
		
		TitleNode.AttachNode(this, NodePosition.AsLastChild);
	}
	
	protected override void Dispose(bool disposing) {
		if (disposing) {
			CloseButtonNode.Dispose();
			ConfigurationButtonNode.Dispose();
			InformationButtonNode.Dispose();
			SubtitleNode.Dispose();
			TitleNode.Dispose();
			
			base.Dispose(disposing);
		}
	}
	
	public override float Width {
		get => base.Width;
		set {
			CloseButtonNode.X = value - 33.0f;
			ConfigurationButtonNode.X = value - 47.0f;
			InformationButtonNode.X = value - 61.0f;
			base.Width = value;
		}
	}

	internal override bool SuppressDispose {
		get => base.SuppressDispose;
		set {
			base.SuppressDispose = value;
			CloseButtonNode.SuppressDispose = value;
			ConfigurationButtonNode.SuppressDispose = value;
			InformationButtonNode.SuppressDispose = value;
			SubtitleNode.SuppressDispose = value;
			TitleNode.SuppressDispose = value;
		}
	}
}