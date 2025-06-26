using System;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace KamiToolKit.System;

public abstract unsafe partial class NodeBase {

	private ViewportEventListener? resizeEventListener;

	private SimpleComponentNode? resizeContainer;
	
	private VerticalResizeNineGridNode? rightBorderNode;
	private HorizontalResizeNineGridNode? bottomBorderNode;
	private VerticalResizeNineGridNode? leftBorderNode;
	private HorizontalResizeNineGridNode? topBorderNode;
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
		resizeContainer.AttachNode(this);
		
		bottomRightResizeNode = new ResizeButtonNode(ResizeDirection.BottomRight) {
			Position = resizeContainer.Size - new Vector2(28.0f, 28.0f) - new Vector2(16.0f, 16.0f),
			Size = new Vector2(28.0f, 28.0f),
			Origin = new Vector2(14.0f, 14.0f),
			IsVisible = true,
			EnableEventFlags = true,
		};
		bottomRightResizeNode.AttachNode(resizeContainer);

		bottomLeftResizeNode = new ResizeButtonNode(ResizeDirection.BottomLeft) {
			Position = new Vector2(16.0f, resizeContainer.Size.Y - 28.0f - 16.0f),
			Size = new Vector2(28.0f, 28.0f),
			Origin = new Vector2(14.0f, 14.0f),
			IsVisible = true,
		};
		bottomLeftResizeNode.AttachNode(resizeContainer);

		bottomBorderNode = new HorizontalResizeNineGridNode {
			Position = new Vector2(16.0f + 28.0f, Size.Y + 16.0f - 6.0f),
			Size = new Vector2(Size.X - 28.0f * 2.0f, 8.0f),
			IsVisible = true,
		};
		bottomBorderNode.AttachNode(resizeContainer);

		topBorderNode = new HorizontalResizeNineGridNode {
			Position = new Vector2(16.0f, 16.0f - 4.0f), 
			Size = new Vector2(Size.X, 8.0f), 
			IsVisible = true,
		};
		topBorderNode.AttachNode(resizeContainer);

		leftBorderNode = new VerticalResizeNineGridNode {
			Position = new Vector2(16.0f + 4.0f, 16.0f),
			Size = new Vector2 (8.0f, Size.Y - 28.0f),
			Rotation = 1 * MathF.PI / 2.0f,
			IsVisible = true,
		};
		leftBorderNode.AttachNode(resizeContainer);
		
		rightBorderNode = new VerticalResizeNineGridNode {
			Position = new Vector2(Size.X + 16.0f + 4.0f, 16.0f),
			Size = new Vector2 (8.0f, Size.Y - 28.0f),
			Rotation = 1 * MathF.PI / 2.0f,
			IsVisible = true,
		};
		rightBorderNode.AttachNode(resizeContainer);
	}

	private void OnResizeEvent(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
		if (resizeContainer is null) return;
		if (bottomRightResizeNode is null) return;
		if (resizeEventListener is null) return;
		if (bottomLeftResizeNode is null) return;
		if (bottomBorderNode is null) return;
		if (topBorderNode is null) return;
		if (leftBorderNode is null) return;
		if (rightBorderNode is null) return;

		ref var mouseData = ref atkEventData->MouseData;
		var mousePosition = new Vector2(mouseData.PosX,  mouseData.PosY);
		
		switch (eventType) {
			case AtkEventType.MouseMove when !isResizeDragging: {
				bottomRightResizeNode.IsHovered = bottomRightResizeNode.CheckCollision(atkEventData);
				bottomLeftResizeNode.IsHovered = bottomLeftResizeNode.CheckCollision(atkEventData);
				bottomBorderNode.IsHovered = bottomBorderNode.CheckCollision(atkEventData);
				topBorderNode.IsHovered = topBorderNode.CheckCollision(atkEventData);
				leftBorderNode.IsHovered = leftBorderNode.CheckCollision(atkEventData);
				rightBorderNode.IsHovered = rightBorderNode.CheckCollision(atkEventData);
			} break;

			case AtkEventType.MouseMove when isResizeDragging: {
				var newPosition = mousePosition;
				var delta = newPosition - resizeClickStart;
		
				if (bottomLeftResizeNode.IsHovered) {
					Position += delta with { Y = 0.0f };
					Size += new Vector2(-delta.X, delta.Y);
				}

				if (bottomRightResizeNode.IsHovered) {
					Size += delta;
				}

				if (bottomBorderNode.IsHovered) {
					Size += new Vector2(0.0f, delta.Y);
				}

				if (topBorderNode.IsHovered) {
					Position += delta with { X = 0.0f };
					Size -= new Vector2(0.0f, delta.Y);
				}

				if (leftBorderNode.IsHovered) {
					Position += delta with { Y = 0.0f };
					Size -= new Vector2(delta.X, 0.0f);
				}
				
				if (rightBorderNode.IsHovered) {
					Size += new Vector2(delta.X, 0.0f);
				}
				
				resizeContainer.Size = Size + new Vector2(32.0f, 32.0f);
				bottomLeftResizeNode.Position = new Vector2(16.0f, resizeContainer.Size.Y - 28.0f - 16.0f);
				bottomRightResizeNode.Position = resizeContainer.Size - new Vector2(28.0f, 28.0f) - new Vector2(16.0f, 16.0f);
				bottomBorderNode.Position = new Vector2(16.0f, Size.Y + 16.0f - 6.0f);
				bottomBorderNode.Width = Size.X;
				topBorderNode.Position = new Vector2(16.0f, 16.0f - 4.0f);
				topBorderNode.Width = Size.X;
				leftBorderNode.Height = Size.Y;
				rightBorderNode.Position = new Vector2(Size.X + 16.0f + 4.0f, 16.0f);
				rightBorderNode.Height = Size.Y;

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
		if (bottomLeftResizeNode.IsHovered) SetCursor(AddonCursorType.ResizeNESW);
		if (bottomBorderNode.IsHovered) SetCursor(AddonCursorType.ResizeNS);
		if (topBorderNode.IsHovered) SetCursor(AddonCursorType.ResizeNS);
		if (leftBorderNode.IsHovered) SetCursor(AddonCursorType.ResizeWE);
		if (rightBorderNode.IsHovered) SetCursor(AddonCursorType.ResizeWE);
	}

	private bool IsAnyButtonHovered(AtkEventData* atkEventData) {
		if (bottomRightResizeNode?.CheckCollision(atkEventData) ?? false) return true;
		if (bottomLeftResizeNode?.CheckCollision(atkEventData) ?? false) return true;
		if (bottomBorderNode?.CheckCollision(atkEventData) ?? false) return true;
		if (topBorderNode?.CheckCollision(atkEventData) ?? false) return true;
		if (leftBorderNode?.CheckCollision(atkEventData) ?? false) return true;
		if (rightBorderNode?.CheckCollision(atkEventData) ?? false) return true;

		return false;
	}
}