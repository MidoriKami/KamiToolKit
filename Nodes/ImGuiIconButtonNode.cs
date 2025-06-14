using System;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using Dalamud.Interface.Textures.TextureWraps;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.TimelineBuilding;

namespace KamiToolKit.Nodes;

public class ImGuiIconButtonNode : ComponentNode<AtkComponentButton, AtkUldComponentDataButton> {
	protected readonly ImGuiImageNode ImageNode;

	public ImGuiIconButtonNode() {
		SetInternalComponentType(ComponentType.Button);
		
		ImageNode = new ImGuiImageNode {
			IsVisible = true,
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

	public override float Width {
		get => base.Width;
		set {
			ImageNode.Width = value;
			base.Width = value;
		}
	}

	public override float Height {
		get => base.Height;
		set {
			ImageNode.Height = value;
			base.Height = value;
		}
	}

	public void LoadTexture(IDalamudTextureWrap texture)
		=> ImageNode.LoadTexture(texture);
	
	public void LoadTextureFromFile(string path)
		=> ImageNode.LoadTextureFromFile(path);

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