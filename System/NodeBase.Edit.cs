using System;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace KamiToolKit.System;

public abstract unsafe partial class NodeBase {

	private ViewportEventListener? editEventListener;

	private NodeEditOverlayNode? overlayNode;

	private bool isMoving;
	private bool isResizing;
	private Vector2 clickStartPosition = Vector2.Zero;
	private NodeEditMode currentEditMode = 0;
	
	public Action? OnResizeComplete { get; set; }
	public Action? OnMoveComplete { get; set; }
	public Action? OnEditComplete { get; set; }

	public void EnableEditMode(NodeEditMode mode) {
		if (EventAddonPointer is null) {
			Log.Warning("Attempted to enable edit mode to dangling node. Aborting.");
			return;
		}

		currentEditMode |= mode;
		
		if (overlayNode is null) {
			overlayNode = new NodeEditOverlayNode {
				Position = new Vector2(-16.0f, -16.0f),
				Size = Size + new Vector2(32.0f, 32.0f),
				IsVisible = true,
			};
			overlayNode.AttachNode(this);
		}
		
		overlayNode.ShowParts = mode.HasFlag(NodeEditMode.Resize);

		if (editEventListener is null) {
			editEventListener = new ViewportEventListener(OnEditEvent);
			editEventListener.AddEvent(AtkEventType.MouseMove, overlayNode.InternalResNode);
			editEventListener.AddEvent(AtkEventType.MouseDown, overlayNode.InternalResNode);
		}
		
	}

	public void DisableEditMode(NodeEditMode mode) {

		currentEditMode &= ~mode;

		if (currentEditMode.HasFlag(NodeEditMode.Resize) || currentEditMode.HasFlag(NodeEditMode.Move)) return;

		if (editEventListener is not null) {
			editEventListener.Dispose();
			editEventListener = null;
		}

		if (overlayNode is not null) {
			overlayNode.Dispose();
			overlayNode = null;
		}
		
	}

	private void OnEditEvent(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
		if (overlayNode is null) return;
		if (editEventListener is null) return;
		
		ref var mouseData = ref atkEventData->MouseData;
		var mousePosition = new Vector2(mouseData.PosX,  mouseData.PosY);
		var mouseDelta = mousePosition - clickStartPosition;
		
		switch (eventType) {
			// Move Logic
			case AtkEventType.MouseMove when isMoving: {
				Position += mouseDelta;
				clickStartPosition = mousePosition;

				atkEvent->SetEventIsHandled(true);
			} break;

			// Update hover state when not resizing, as we latch that for the behavior
			case AtkEventType.MouseMove when !isResizing: {
				overlayNode.UpdateHover(atkEventData);
			} break;
			
			// Resize Logic
			case AtkEventType.MouseMove when isResizing: {
				Position += overlayNode.GetPositionDelta(mouseDelta);
				Size += overlayNode.GetSizeDelta(mouseDelta);
				
				overlayNode.Size = Size + new Vector2(32.0f, 32.0f);

				clickStartPosition = mousePosition;

				atkEvent->SetEventIsHandled(true);
			} break;

			// Begin Resize Event
			case AtkEventType.MouseDown when !isResizing && overlayNode.AnyHovered() && currentEditMode.HasFlag(NodeEditMode.Resize): {
				editEventListener.AddEvent(AtkEventType.MouseUp, overlayNode.InternalResNode);
				
				isResizing = true;
				clickStartPosition = mousePosition;

				atkEvent->SetEventIsHandled(true);
			} break;

			// End Resize Event
			case AtkEventType.MouseUp when isResizing: {
				OnResizeComplete?.Invoke();
				OnEditComplete?.Invoke();
				
				isResizing = false;
				editEventListener.RemoveEvent(AtkEventType.MouseUp);
			} break;
			
			// Begin Move Event
			case AtkEventType.MouseDown when !overlayNode.AnyHovered() && overlayNode.CheckCollision(atkEventData) && !isMoving && currentEditMode.HasFlag(NodeEditMode.Move): {
				editEventListener.AddEvent(AtkEventType.MouseUp, overlayNode.InternalResNode);
				
				isMoving = true;
				clickStartPosition = mousePosition;
				
				atkEvent->SetEventIsHandled(true);
			} break;
			
			// End Move Event
			case AtkEventType.MouseUp when isMoving: {
				OnMoveComplete?.Invoke();
				OnEditComplete?.Invoke();

				isMoving = false;
				editEventListener.RemoveEvent(AtkEventType.MouseUp);
			} break;
		}

		ResetCursor();
		if (currentEditMode.HasFlag(NodeEditMode.Move)) {
			if (isMoving) {
				SetCursor(AddonCursorType.Grab);
			}
			else if (overlayNode.CheckCollision(atkEventData)) {
				SetCursor(AddonCursorType.Hand);
			}
		}
		
		if (overlayNode.AnyHovered() && currentEditMode.HasFlag(NodeEditMode.Resize)) {
			overlayNode.SetCursor();
		}
	}
}