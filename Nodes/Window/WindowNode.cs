using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.TimelineBuilding;
using KamiToolKit.Nodes.Image;

namespace KamiToolKit.Nodes.Window;

public unsafe class WindowNode : ComponentNode<AtkComponentWindow, AtkUldComponentDataWindow> {

	internal WindowHeaderNode HeaderNode;
	protected CollisionNode HeaderCollisionNode;
	protected WindowBackgroundNode BackgroundNode;
	protected WindowBackgroundNode BorderNode;
	protected ImageNode BackgroundImageNode;

	public WindowNode() {
		SetInternalComponentType(ComponentType.Window);

		CollisionNode.NodeId = 13;
		Component->ShowFlags = 1;

		HeaderCollisionNode = new CollisionNode {
			Uses = 2,
			NodeId = 12,
			Height = 28.0f,
			Position = new Vector2(8.0f, 8.0f),
			NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.AnchorRight | NodeFlags.Visible |NodeFlags.Enabled |NodeFlags.HasCollision | NodeFlags.RespondToMouse | NodeFlags.EmitsEvents,
		};
		
		HeaderCollisionNode.AttachNode(this);

		BackgroundNode = new WindowBackgroundNode(false) {
			NodeId = 11,
			Position = Vector2.Zero,
			Offsets = new Vector4(64.0f, 32.0f, 32.0f, 32.0f),
			NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.Fill | NodeFlags.EmitsEvents,
			PartsRenderType = 19,
		};
		
		BackgroundNode.AttachNode(this);

		BorderNode = new WindowBackgroundNode(true) {
			NodeId = 10,
			Position = Vector2.Zero,
			Offsets = new Vector4(64.0f, 32.0f, 32.0f, 32.0f),
			NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Enabled | NodeFlags.Fill | NodeFlags.EmitsEvents,
			PartsRenderType = 7,
		};
		
		BorderNode.AttachNode(this);

		BackgroundImageNode = new SimpleImageNode {
			NodeId = 9,
			WrapMode = 2,
			ImageNodeFlags = 0,
			NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.AnchorBottom | NodeFlags.AnchorRight | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
			TexturePath = "ui/uld/WindowA_Gradation.tex",
			TextureCoordinates = new Vector2(6.0f, 2.0f),
			TextureSize = new Vector2(24.0f, 24.0f),
		};
		
		BackgroundImageNode.AttachNode(this);

		HeaderNode = new WindowHeaderNode {
			Size = new Vector2(477.0f, 38.0f),
			NodeId = 2,
			IsVisible = true,
		};
		HeaderNode.AttachNode(this);
		
		Data->ShowCloseButton = 1;
		Data->ShowConfigButton = 0;
		Data->ShowHelpButton = 0;
		Data->ShowHeader = 1;
		Data->Nodes[0] = HeaderNode.TitleNode.NodeId;
		Data->Nodes[1] = HeaderNode.SubtitleNode.NodeId;
		Data->Nodes[2] = HeaderNode.CloseButtonNode.NodeId;
		Data->Nodes[3] = HeaderNode.ConfigurationButtonNode.NodeId;
		Data->Nodes[4] = HeaderNode.InformationButtonNode.NodeId;
		Data->Nodes[5] = 0;
		Data->Nodes[6] = HeaderNode.NodeId;
		Data->Nodes[7] = 0;
		
		NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents;

		LoadTimelines();
		
		InitializeComponentEvents();
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			HeaderNode.Dispose();
			HeaderCollisionNode.Dispose();
			BackgroundNode.Dispose();
			BorderNode.Dispose();
			BackgroundImageNode.Dispose();
			
			base.Dispose(disposing);
		}
	}

	public AtkUnitBase* OwnerAddon {
		get => Component->OwnerUnitBase;
		set => Component->OwnerUnitBase = value;
	}
	
	public void SetTitle(string title, string? subtitle = null)
		=> Component->SetTitle(title, subtitle ?? string.Empty);

	public string Title {
		get => HeaderNode.TitleNode.String;
		set {
			HeaderNode.TitleNode.String = value;
			HeaderNode.TitleNode.IsVisible = true;
		}
	}

	public string Subtitle {
		get => HeaderNode.SubtitleNode.String;
		set {
			HeaderNode.SubtitleNode.String = value;
			HeaderNode.SubtitleNode.IsVisible = true;
			HeaderNode.SubtitleNode.X = HeaderNode.TitleNode.X + HeaderNode.TitleNode.Width + 2.0f;
		}
	}

	public bool ShowCloseButton {
		get => HeaderNode.CloseButtonNode.IsVisible;
		set => HeaderNode.CloseButtonNode.IsVisible = value;
	}

	public bool ShowConfigButton {
		get => HeaderNode.ConfigurationButtonNode.IsVisible;
		set => HeaderNode.ConfigurationButtonNode.IsVisible = value;
	}

	public bool ShowHelpButton {
		get => HeaderNode.InformationButtonNode.IsVisible;
		set => HeaderNode.InformationButtonNode.IsVisible = value;
	}

	public bool ShowHeader {
		get => HeaderNode.InformationButtonNode.IsVisible;
		set => HeaderNode.InformationButtonNode.IsVisible = value;
	}

	public override float Width {
		get => base.Width;
		set {
			base.Width = value;
			HeaderNode.Width = value;
			HeaderCollisionNode.Width = value - 14.0f;
			BackgroundNode.Width = value;
			BorderNode.Width = value;
			BackgroundImageNode.Width = value - 8.0f;
			BackgroundImageNode.X = 4.0f;
		}
	}

	public override float Height {
		get => base.Height;
		set {
			base.Height = value;
			BackgroundNode.Height = value;
			BorderNode.Height = value;
			BackgroundImageNode.Height = value - 16.0f;
			BackgroundImageNode.Y = 4.0f;
		}
	}

	public bool Focused {
		get => BorderNode.IsVisible;
		set => BorderNode.IsVisible = value;
	}
	
	public float HeaderHeight => HeaderNode.Height;
	
	public Vector2 ContentSize => new(BackgroundImageNode.Width, BackgroundImageNode.Height - HeaderHeight);
	
	public Vector2 ContentStartPosition => new(BackgroundImageNode.X, BackgroundImageNode.Y + HeaderHeight);
	
	private void LoadTimelines() {
		AddTimeline(new TimelineBuilder()
			.BeginFrameSet(1, 29)
			.AddLabelPair(1, 9, 17)
			.AddLabelPair(10, 19, 18)
			.AddLabelPair(20, 29, 7)
			.EndFrameSet()
			.Build());

		BackgroundNode.AddTimeline(new TimelineBuilder()
			.AddFrameSetWithFrame(1, 9, 1, multiplyColor: new Vector3(100.0f))
			.AddFrameSetWithFrame(10, 19, 10, multiplyColor: new Vector3(100.0f))
			.AddFrameSetWithFrame(20, 29, 20, multiplyColor: new Vector3(50.0f))
			.Build());
		
		BorderNode.AddTimeline(new TimelineBuilder()
			.BeginFrameSet(10, 19)
			.AddFrame(10, alpha: 0)
			.AddFrame(12, alpha: 255)
			.EndFrameSet()
			.Build());
	}
}
