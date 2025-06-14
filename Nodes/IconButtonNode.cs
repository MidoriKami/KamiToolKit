using System;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.TimelineBuilding;

namespace KamiToolKit.Nodes;

/// <summary>
/// Uses a GameIconId to display that icon as the decorator for the button.
/// </summary>
public class IconButtonNode : ComponentNode<AtkComponentButton, AtkUldComponentDataButton> {
	protected readonly IconImageNode ImageNode;
	protected readonly NineGridNode BackgroundNode;

	public IconButtonNode() {
		SetInternalComponentType(ComponentType.Button);

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
		
		AddEvent(AddonEventType.MouseClick, ClickHandler);
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			ImageNode.DetachNode();
			ImageNode.Dispose();
			
			base.Dispose(disposing);
		}
	}

	public Action? OnClick { get; set; }
	
	private void ClickHandler() {
		OnClick?.Invoke();
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

	private void LoadTimelines() {
		AddTimeline(new TimelineBuilder()
				.BeginFrameSet(1, 30)
				.AddEmptyFrame(1)
				.EndFrameSet()
				.BeginFrameSet(31, 60)
				.AddEmptyFrame(31)
				.EndFrameSet()
				.Build()
			);
		
		ImageNode.AddTimeline(new TimelineBuilder()
				.BeginFrameSet(1, 9)
				.AddFrame(1, position: new Vector2(0,0))
				.AddFrame(1, alpha: 255)
				.AddFrame(1, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
				.EndFrameSet()
				.BeginFrameSet(10, 19)
				.AddFrame(10, position: new Vector2(0,0))
				.AddFrame(12, position: new Vector2(0,0))
				.AddFrame(10, alpha: 255)
				.AddFrame(12, alpha: 255)
				.AddFrame(10, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
				.AddFrame(12, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
				.EndFrameSet()
				.BeginFrameSet(20, 29)
				.AddFrame(20, position: new Vector2(0,1))
				.AddFrame(20, alpha: 255)
				.AddFrame(20, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
				.EndFrameSet()
				.BeginFrameSet(30, 39)
				.AddFrame(30, position: new Vector2(0,0))
				.AddFrame(30, alpha: 178)
				.AddFrame(30, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(50, 50, 50))
				.EndFrameSet()
				.BeginFrameSet(40, 49)
				.AddFrame(40, position: new Vector2(0,0))
				.AddFrame(40, alpha: 255)
				.AddFrame(40, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
				.EndFrameSet()
				.BeginFrameSet(50, 59)
				.AddFrame(50, position: new Vector2(0,0))
				.AddFrame(52, position: new Vector2(0,0))
				.AddFrame(50, alpha: 255)
				.AddFrame(52, alpha: 255)
				.AddFrame(50, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
				.AddFrame(52, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
				.EndFrameSet()
				.Build()
			);
	}
}