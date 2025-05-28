using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.NodeParts;

namespace KamiToolKit.Nodes.ComponentNodes.Window;

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
			TexturePath = "ui/uld/WindowA_Gradation_hr1.tex",
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

	internal override bool SuppressDispose {
		get => base.SuppressDispose;
		set {
			base.SuppressDispose = value;
			headerNode.SuppressDispose = value;
			headerCollisionNode.SuppressDispose = value;
			backgroundNode.SuppressDispose = value;
			borderNode.SuppressDispose = value;
			backgroundImageNode.SuppressDispose = value;
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
	
	private void LoadTimelines() {
		AddTimeline(new Timeline {
			FrameTime = 0.3f,
			ParentFrameTime = 0.033333335f,
			LabelFrameIdxDuration = 28,
			LabelEndFrameIdx = 29,
			Mask = (AtkTimelineMask) 0xFF,
			LabelSets = [
				new TimelineLabelSet {
					StartFrameId = 1,
					EndFrameId = 29,
					Labels = [
						new TimelineLabelFrame { FrameIndex = 1, LabelId = 17 },
						new TimelineLabelFrame { FrameIndex = 9, JumpBehavior = AtkTimelineJumpBehavior.PlayOnce },
						new TimelineLabelFrame { FrameIndex = 10, LabelId = 18 },
						new TimelineLabelFrame { FrameIndex = 19, JumpBehavior = AtkTimelineJumpBehavior.PlayOnce },
						new TimelineLabelFrame { FrameIndex = 20, LabelId = 7 },
						new TimelineLabelFrame { FrameIndex = 29, JumpBehavior = AtkTimelineJumpBehavior.PlayOnce },
					],
				},
			],
		});
		
		backgroundNode.AddTimeline(new Timeline {
			Mask = AtkTimelineMask.VendorSpecific2,
			Animations = [
				new TimelineAnimation{ StartFrameId = 1, EndFrameId = 9, KeyFrames = [
					new TimelineKeyFrame{ FrameIndex = 1, NodeTint = new NodeTint{ AddColor = Vector3.Zero, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f)}, }, 
				]},
				new TimelineAnimation{ StartFrameId = 10, EndFrameId = 19, KeyFrames = [
					new TimelineKeyFrame{ FrameIndex = 10, NodeTint = new NodeTint{ AddColor = Vector3.Zero, MultiplyColor = new Vector3(100.0f, 100.0f, 100.0f)}, }, 
				]},
				new TimelineAnimation{ StartFrameId = 20, EndFrameId = 29, KeyFrames = [
					new TimelineKeyFrame{ FrameIndex = 20, NodeTint = new NodeTint{ AddColor = Vector3.Zero, MultiplyColor = new Vector3(50.0f, 50.0f, 50.0f)}, }, 
				]},
			],
		});
		
		borderNode.AddTimeline(new Timeline {
			Mask = (AtkTimelineMask) 0xFF,
			Animations = [
				new TimelineAnimation{ StartFrameId = 10, EndFrameId = 19, KeyFrames = [
					new TimelineKeyFrame{ FrameIndex = 10, Alpha = 0, }, 
					new TimelineKeyFrame{ FrameIndex = 12, Alpha = 255, }, 
				]},
			],
		});
	}
}
