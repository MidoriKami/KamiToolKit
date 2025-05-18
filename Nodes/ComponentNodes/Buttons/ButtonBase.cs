using System.Numerics;
using Dalamud.Game.Addon.Events;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using KamiToolKit.Classes;
using KamiToolKit.System;

namespace KamiToolKit.Nodes.ComponentNodes;

public abstract unsafe class ButtonBase : ComponentNode<AtkComponentButton, AtkUldComponentDataButton> {
	public readonly NineGridNode BackgroundNode;
	protected abstract NodeBase DecorationNode { get; }

	private bool buttonHeld;

	protected ButtonBase() {
		SetInternalComponentType(ComponentType.Button);
		Data->Nodes[1] = 2;
		
		BackgroundNode = new SimpleNineGridNode {
			TexturePath = "ui/uld/ButtonA_hr1.tex",
			IsVisible = true,
			TextureSize = new Vector2(100.0f, 28.0f),
			LeftOffset = 16.0f,
			RightOffset = 16.0f,
			PartsRenderType = 88,
			NodeId = 2,
		};

		BackgroundNode.AttachNode(this, NodePosition.AfterAllSiblings);

		CollisionNode.AddEvent(AddonEventType.MouseOver, OnMouseOver);
		CollisionNode.AddEvent(AddonEventType.MouseOut, OnMouseOut);
		CollisionNode.AddEvent(AddonEventType.MouseDown, OnMouseDown);
		CollisionNode.AddEvent(AddonEventType.MouseUp, OnMouseUp);
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			BackgroundNode.DetachNode();
			BackgroundNode.Dispose();
			
			CollisionNode.RemoveEvent(AddonEventType.MouseOver, OnMouseOver);
			CollisionNode.RemoveEvent(AddonEventType.MouseOut, OnMouseOut);
			CollisionNode.RemoveEvent(AddonEventType.MouseDown, OnMouseDown);
			CollisionNode.RemoveEvent(AddonEventType.MouseUp, OnMouseUp);

			NativeMemoryHelper.UiFree(Data);
			Data = null;

			base.Dispose(disposing);
		}
	}

	public override void EnableEvents(IAddonEventManager eventManager, AtkUnitBase* addon) {
		base.EnableEvents(eventManager, addon);
		
		CollisionNode.EnableEvents(eventManager, addon);
	}

	public override void DisableEvents(IAddonEventManager eventManager) {
		base.DisableEvents(eventManager);
		
		CollisionNode.DisableEvents(eventManager);
	}

	private void OnMouseDown() {
		if (!buttonHeld) {
			BackgroundNode.Position += new Vector2(0.0f, 1.0f);
			DecorationNode.Position += new Vector2(0.0f, 1.0f);
			buttonHeld = true;
		}
	}
	
	private void OnMouseUp() {
		if (buttonHeld) {
			BackgroundNode.Position -= new Vector2(0.0f, 1.0f);
			DecorationNode.Position -= new Vector2(0.0f, 1.0f);
			buttonHeld = false;
		}
	}

	private void OnMouseOver() {
		BackgroundNode.AddColor = new Vector3(16.0f, 16.0f, 16.0f).AsVector4().NormalizeToUnitRange().AsVector3();
		SetCursor(AddonCursorType.Clickable);
	}
	
	private void OnMouseOut() {
		if (buttonHeld) {
			BackgroundNode.Position -= new Vector2(0.0f, 1.0f);
			DecorationNode.Position -= new Vector2(0.0f, 1.0f);
			buttonHeld = false;
		}
		
		BackgroundNode.AddColor = Vector3.Zero;
		ResetCursor();
	}
	
	public new float Width {
		get => InternalResNode->Width;
		set {
			InternalResNode->SetWidth((ushort) value);
			BackgroundNode.Width = value;
			CollisionNode.Width = value;
		}
	}

	public new float Height {
		get => InternalResNode->Height;
		set {
			InternalResNode->SetHeight((ushort) value);
			BackgroundNode.Height = value;
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

	public bool BackgroundVisible {
		get => BackgroundNode.IsVisible;
		set => BackgroundNode.IsVisible = value;
	}

	public override void DrawConfig() {
		using var buttonNode = ImRaii.TreeNode("Button Node");
		if (!buttonNode) return;
		
		base.DrawConfig();

		using (var buttonProperties = ImRaii.TreeNode("Button Properties")) {
			
		}

		using (var table = ImRaii.Table("button_properties_table", 2)) {
			if (!table) return;
		
			ImGui.TableSetupColumn("##label", ImGuiTableColumnFlags.WidthStretch, 1.0f);
			ImGui.TableSetupColumn("##configuration", ImGuiTableColumnFlags.WidthStretch, 2.0f);

			ImGui.TableNextColumn();
			ImGui.Text("Background Visible");
		
			ImGui.TableNextColumn();
			var backgroundvisible = BackgroundVisible;
			ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
			if (ImGui.Checkbox("##BackgroundVisible", ref backgroundvisible)) {
				BackgroundVisible = backgroundvisible;
			}
		}
		
		using (var collisionNode = ImRaii.TreeNode("Collision Node")) {
			if (collisionNode) {
				CollisionNode.DrawConfig();
			}
		}
				
		using (var backgroundNode = ImRaii.TreeNode("Background Node")) {
			if (backgroundNode) {
				BackgroundNode.DrawConfig();
			}
		}
		
		using (var decorationNode = ImRaii.TreeNode("Decoration Node")) {
			if (decorationNode) {
				DecorationNode.DrawConfig();
			}
		}
	}
}