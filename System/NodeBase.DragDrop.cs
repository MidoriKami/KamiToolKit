using System;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using KamiToolKit.Extensions;

namespace KamiToolKit.System;

public abstract partial class NodeBase {

	private bool clickDragEventsRegistered;

	private bool isDragging;
	private Vector2 clickStart = Vector2.Zero;

	public Action? OnClickDragComplete { get; set; }

	public void EnableClickDrag(bool setEventFlags = false) {
		if (clickDragEventsRegistered) return;
		
		AddEvent(AddonEventType.MouseOver, ClickDragMouseOver);
		AddEvent(AddonEventType.MouseDown, ClickDragStart);
		AddEvent(AddonEventType.MouseMove, ClickDragMove);
		AddEvent(AddonEventType.MouseUp, ClickDragEnd);
		AddEvent(AddonEventType.MouseOut, ClickDragMouseOut);
		
		clickDragEventsRegistered = true;

		if (setEventFlags) {
			SetEventFlags();
		}
	}

	public void DisableClickDrag(bool clearEventFlags = false) {
		if (!clickDragEventsRegistered) return;
		
		RemoveEvent(AddonEventType.MouseOver, ClickDragMouseOver);
		RemoveEvent(AddonEventType.MouseDown, ClickDragStart);
		RemoveEvent(AddonEventType.MouseMove, ClickDragMove);
		RemoveEvent(AddonEventType.MouseUp, ClickDragEnd);
		RemoveEvent(AddonEventType.MouseOut, ClickDragMouseOut);
		
		clickDragEventsRegistered = false;

		if (clearEventFlags) {
			ClearEventFlags();
		}
	}

	private void ClickDragMouseOver(AddonEventData eventData) {
		SetCursor(AddonCursorType.Hand);
		
		eventData.SetHandled();
	}
	
	private void ClickDragStart(AddonEventData eventData) {
		isDragging = true;
		clickStart = eventData.GetMousePosition();
		SetCursor(AddonCursorType.Grab);
		
		eventData.SetHandled();
	}
	
	private void ClickDragMove(AddonEventData eventData) {
		if (!isDragging) return;
		
		var newPosition = eventData.GetMousePosition();
		var delta = newPosition - clickStart;
		
		Position += delta;
		clickStart = newPosition;
		
		eventData.SetHandled();
	}

	private void ClickDragEnd(AddonEventData eventData) {
		isDragging = false;
		SetCursor(AddonCursorType.Hand);
		
		OnClickDragComplete?.Invoke();
		
		eventData.SetHandled();
	}

	private void ClickDragMouseOut(AddonEventData eventData) {
		isDragging = false;
		ResetCursor();

		OnClickDragComplete?.Invoke();
		
		eventData.SetHandled();
	}
}