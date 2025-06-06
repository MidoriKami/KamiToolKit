﻿using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Classes.TimelineBuilding;

namespace KamiToolKit.Nodes.Window;

public unsafe class WindowNode : ComponentNode<AtkComponentWindow, AtkUldComponentDataWindow> {

	private WindowHeaderNode headerNode;
	private CollisionNode headerCollisionNode;
	private WindowBackgroundNode backgroundNode;
	private WindowBackgroundNode borderNode;
	private ImageNode backgroundImageNode;

	public WindowNode() {
		SetInternalComponentType(ComponentType.Window);

		CollisionNode.NodeId = 13;
		Component->ShowFlags = 1;

		headerCollisionNode = new CollisionNode {
			Uses = 2,
			NodeId = 12,
			Height = 28.0f,
			Position = new Vector2(8.0f, 8.0f),
			NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.AnchorRight | NodeFlags.Visible |NodeFlags.Enabled |NodeFlags.HasCollision | NodeFlags.RespondToMouse | NodeFlags.EmitsEvents,
		};
		
		headerCollisionNode.AttachNode(this, NodePosition.AfterAllSiblings);

		backgroundNode = new WindowBackgroundNode(false) {
			NodeId = 11,
			Position = Vector2.Zero,
			Offsets = new Vector4(64.0f, 32.0f, 32.0f, 32.0f),
			NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.Fill | NodeFlags.EmitsEvents,
			PartsRenderType = 19,
		};
		
		backgroundNode.AttachNode(this, NodePosition.AfterAllSiblings);

		borderNode = new WindowBackgroundNode(true) {
			NodeId = 10,
			Position = Vector2.Zero,
			Offsets = new Vector4(64.0f, 32.0f, 32.0f, 32.0f),
			NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Enabled | NodeFlags.Fill | NodeFlags.EmitsEvents,
			PartsRenderType = 7,
		};
		
		borderNode.AttachNode(this, NodePosition.AfterAllSiblings);

		backgroundImageNode = new SimpleImageNode {
			NodeId = 9,
			WrapMode = 2,
			ImageNodeFlags = 0,
			NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.AnchorBottom | NodeFlags.AnchorRight | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
			TexturePath = "ui/uld/WindowA_Gradation.tex",
			TextureCoordinates = new Vector2(6.0f, 2.0f),
			TextureSize = new Vector2(24.0f, 24.0f),
		};
		
		backgroundImageNode.AttachNode(this, NodePosition.AfterAllSiblings);

		headerNode = new WindowHeaderNode {
			Size = new Vector2(477.0f, 38.0f),
			NodeId = 2,
			IsVisible = true,
		};
		headerNode.AttachNode(this, NodePosition.AfterAllSiblings);
		
		Data->ShowCloseButton = 1;
		Data->ShowConfigButton = 0;
		Data->ShowHelpButton = 0;
		Data->ShowHeader = 1;
		Data->Nodes[0] = headerNode.TitleNode.NodeId;
		Data->Nodes[1] = headerNode.SubtitleNode.NodeId;
		Data->Nodes[2] = headerNode.CloseButtonNode.NodeId;
		Data->Nodes[3] = headerNode.ConfigurationButtonNode.NodeId;
		Data->Nodes[4] = headerNode.InformationButtonNode.NodeId;
		Data->Nodes[5] = 0;
		Data->Nodes[6] = headerNode.NodeId;
		Data->Nodes[7] = 0;
		
		NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents;

		LoadTimelines();
		
		InitializeComponentEvents();
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			headerNode.Dispose();
			headerCollisionNode.Dispose();
			backgroundNode.Dispose();
			borderNode.Dispose();
			backgroundImageNode.Dispose();
			
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
		get => headerNode.TitleNode.String;
		set {
			headerNode.TitleNode.String = value;
			headerNode.TitleNode.IsVisible = true;
		}
	}

	public string Subtitle {
		get => headerNode.SubtitleNode.String;
		set {
			headerNode.SubtitleNode.String = value;
			headerNode.SubtitleNode.IsVisible = true;
			headerNode.SubtitleNode.X = headerNode.TitleNode.X + headerNode.TitleNode.Width + 2.0f;
		}
	}

	public bool ShowCloseButton {
		get => headerNode.CloseButtonNode.IsVisible;
		set => headerNode.CloseButtonNode.IsVisible = value;
	}

	public bool ShowConfigButton {
		get => headerNode.ConfigurationButtonNode.IsVisible;
		set => headerNode.ConfigurationButtonNode.IsVisible = value;
	}

	public bool ShowHelpButton {
		get => headerNode.InformationButtonNode.IsVisible;
		set => headerNode.InformationButtonNode.IsVisible = value;
	}

	public bool ShowHeader {
		get => headerNode.InformationButtonNode.IsVisible;
		set => headerNode.InformationButtonNode.IsVisible = value;
	}

	public override float Width {
		get => base.Width;
		set {
			base.Width = value;
			headerNode.Width = value;
			headerCollisionNode.Width = value - 14.0f;
			backgroundNode.Width = value;
			borderNode.Width = value;
			backgroundImageNode.Width = value - 8.0f;
			backgroundImageNode.X = 4.0f;
		}
	}

	public override float Height {
		get => base.Height;
		set {
			base.Height = value;
			backgroundNode.Height = value;
			borderNode.Height = value;
			backgroundImageNode.Height = value - 16.0f;
			backgroundImageNode.Y = 4.0f;
		}
	}

	public bool Focused {
		get => borderNode.IsVisible;
		set => borderNode.IsVisible = value;
	}
	
	public float HeaderHeight => headerNode.Height;
	
	public Vector2 ContentSize => new(backgroundImageNode.Width, backgroundImageNode.Height - HeaderHeight);
	
	public Vector2 ContentStartPosition => new(backgroundImageNode.X, backgroundImageNode.Y + HeaderHeight);
	
	private void LoadTimelines() {
		AddTimeline(new TimelineBuilder()
			.BeginFrameSet(1, 29)
			.AddLabelPair(1, 9, 17)
			.AddLabelPair(10, 19, 18)
			.AddLabelPair(20, 29, 7)
			.EndFrameSet()
			.Build());

		backgroundNode.AddTimeline(new TimelineBuilder()
			.AddFrameSetWithFrame(1, 9, 1, multiplyColor: new Vector3(100.0f))
			.AddFrameSetWithFrame(10, 19, 10, multiplyColor: new Vector3(100.0f))
			.AddFrameSetWithFrame(20, 29, 20, multiplyColor: new Vector3(50.0f))
			.Build());
		
		borderNode.AddTimeline(new TimelineBuilder()
			.BeginFrameSet(10, 19)
			.AddFrame(10, alpha: 0)
			.AddFrame(12, alpha: 255)
			.EndFrameSet()
			.Build());
	}
}
