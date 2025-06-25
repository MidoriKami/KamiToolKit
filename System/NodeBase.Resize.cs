using System;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Extensions;
using KamiToolKit.Nodes;

namespace KamiToolKit.System;

public abstract unsafe partial class NodeBase {

	private ViewportEventListener? resizeEventListener;

	private SimpleComponentNode? resizeContainer;
	
	private NineGridNode? rightBorderNode;
	private NineGridNode? bottomBorderNode;
	private NineGridNode? leftBorderNode;
	private NineGridNode? topBorderNode;
	private ResizeButtonNode? bottomRightResizeNode;
	private ResizeButtonNode? bottomLeftResizeNode;
	
	private bool resizeEventsRegistered;

	private bool isResizeDragging;
	private Vector2 resizeClickStart = Vector2.Zero;

	public Action? OnResizeComplete { get; set; }

	public void EnableResize() {
		if (resizeEventsRegistered) return;

		resizeEventListener ??= new ViewportEventListener(OnResizeEvent);
		
		BuildResizeNodes();

		if (resizeContainer is not null) {
			resizeEventListener.AddEvent(AtkEventType.MouseMove, resizeContainer.InternalResNode);
			resizeEventListener.AddEvent(AtkEventType.MouseDown, resizeContainer.InternalResNode);
		}
		
		resizeEventsRegistered = true;
	}

	public void DisableResize() {
		if (!resizeEventsRegistered) return;

		resizeEventListener?.RemoveEvent(AtkEventType.MouseMove);
		resizeEventListener?.RemoveEvent(AtkEventType.MouseDown);
		
		resizeEventListener?.Dispose();
		resizeEventListener = null;
		
		rightBorderNode?.Dispose();
		rightBorderNode = null;
		
		bottomBorderNode?.Dispose();
		bottomBorderNode = null;
		
		leftBorderNode?.Dispose();
		leftBorderNode = null;
				
		topBorderNode?.Dispose();
		topBorderNode = null;
		
		bottomRightResizeNode?.Dispose();
		bottomRightResizeNode = null;
		
		bottomLeftResizeNode?.Dispose();
		bottomLeftResizeNode = null;
				
		resizeContainer?.Dispose();
		resizeContainer = null;
		
		resizeEventsRegistered = false;
	}

	private void BuildResizeNodes() {
		resizeContainer = new SimpleComponentNode {
			Position = -new Vector2(16.0f, 16.0f),
			Size = Size + new Vector2(32.0f, 32.0f),
			IsVisible = true,
		};
		
		bottomRightResizeNode = new ResizeButtonNode(ResizeDirection.BottomRight) {
			Position = resizeContainer.Size - new Vector2(28.0f, 28.0f) - new Vector2(16.0f, 16.0f),
			Size = new Vector2(28.0f, 28.0f),
			Origin = new Vector2(14.0f, 14.0f),
			IsVisible = true,
			EnableEventFlags = true,
		};
		bottomRightResizeNode.AttachNode(resizeContainer);
		
		resizeContainer.AttachNode(this);
	}

	private void OnMouseDown(AddonEventData data) {
		if (resizeEventListener is null) return;
		if (resizeContainer is null) return;
		
		isResizeDragging = true;
		resizeEventListener.AddEvent(AtkEventType.MouseUp, resizeContainer.InternalResNode);
		data.SetHandled();
	}

	private void OnResizeEvent(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
		if (resizeContainer is null) return;
		if (bottomRightResizeNode is null) return;
		if (resizeEventListener is null) return;

		ref var mouseData = ref atkEventData->MouseData;
		var mousePosition = new Vector2(mouseData.PosX,  mouseData.PosY);
		
		switch (eventType) {
			case AtkEventType.MouseMove when !isResizeDragging: {
				if (resizeContainer.CheckCollision(atkEventData)) {
					bottomRightResizeNode.IsHovered = bottomRightResizeNode.CheckCollision(atkEventData);
				}
				else {
					bottomRightResizeNode.IsHovered = false;
				}
			} break;

			case AtkEventType.MouseMove when isResizeDragging: {
				var newPosition = mousePosition;
				var delta = newPosition - resizeClickStart;
		
				Size += delta;
				resizeContainer.Size += delta;
				bottomRightResizeNode.Position += delta;

				resizeClickStart = newPosition;
				atkEvent->SetEventIsHandled(true);
			} break;

			case AtkEventType.MouseDown when !isResizeDragging && IsAnyButtonHovered(atkEventData): {
				isResizeDragging = true;
				resizeEventListener.AddEvent(AtkEventType.MouseUp, resizeContainer.InternalResNode);
				resizeClickStart = mousePosition;

				atkEvent->SetEventIsHandled(true);
			} break;

			case AtkEventType.MouseUp: {
				if (isResizeDragging) {
					OnResizeComplete?.Invoke();
				}

				isResizeDragging = false;
				resizeEventListener.RemoveEvent(AtkEventType.MouseUp);
			} break;
		}
		
		ResetCursor();

		if (bottomRightResizeNode.IsHovered) SetCursor(AddonCursorType.ResizeNWSR);
	}

	private bool IsAnyButtonHovered(AtkEventData* atkEventData) {
		if (bottomRightResizeNode?.CheckCollision(atkEventData) ?? false) return true;

		return false;
	}
}