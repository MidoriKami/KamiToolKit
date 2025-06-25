using System;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Extensions;

namespace KamiToolKit.System;

public abstract unsafe partial class NodeBase {

	private ViewportEventListener? clickDragEventListener;

	private bool clickDragEventsRegistered;

	private bool isClickDragDragging;
	private Vector2 clickDragClickStart = Vector2.Zero;

	public Action? OnClickDragComplete { get; set; }

	// Note: Event flags must be set to allow drag drop
	public bool EnableDragDrop {
		get;
		set {
			field = value;
			if (value) {
				EnableClickDrag();
			}
			else {
				DisableClickDrag();
			}
		}
	}

	// Note: Event flags must be set to allow drag drop
	public void EnableClickDrag(bool setEventFlags = false) {
		if (clickDragEventsRegistered) return;
		
		AddEvent(AddonEventType.MouseOver, ClickDragMouseOver);
		AddEvent(AddonEventType.MouseDown, ClickDragStart);
		AddEvent(AddonEventType.MouseOut, ClickDragMouseOut);
		
		clickDragEventListener ??= new ViewportEventListener(OnViewportEvent);
		
		clickDragEventsRegistered = true;

		if (setEventFlags) {
			SetEventFlags();
		}
	}

	// Note: Event flags must be set to allow drag drop
	public void DisableClickDrag(bool clearEventFlags = false) {
		if (!clickDragEventsRegistered || clickDragEventListener is null) return;
		
		RemoveEvent(AddonEventType.MouseOver, ClickDragMouseOver);
		RemoveEvent(AddonEventType.MouseDown, ClickDragStart);
		RemoveEvent(AddonEventType.MouseOut, ClickDragMouseOut);
		
		clickDragEventListener?.Dispose();
		clickDragEventListener = null;
		
		clickDragEventsRegistered = false;

		if (clearEventFlags) {
			ClearEventFlags();
		}
	}

	private void OnViewportEvent(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
		if (!isClickDragDragging) return;

		ref var mouseData = ref atkEventData->MouseData;
		
		switch (eventType) {
			case AtkEventType.MouseMove: {
				var mousePosition = new Vector2(mouseData.PosX,  mouseData.PosY);
		
				var newPosition = mousePosition;
				var delta = newPosition - clickDragClickStart;
		
				Position += delta;
				clickDragClickStart = newPosition;
			}
				break;

			case AtkEventType.MouseUp: {
				if (isClickDragDragging) {
					OnClickDragComplete?.Invoke();
				}
		
				isClickDragDragging = false;
				SetCursor(AddonCursorType.Hand);
				
				clickDragEventListener!.RemoveEvent(AtkEventType.MouseMove, 1);
				clickDragEventListener!.RemoveEvent(AtkEventType.MouseUp, 1);
			}
				break;
		}
		
		atkEvent->SetEventIsHandled(true);
	}

	private void ClickDragMouseOver(AddonEventData eventData) {
		SetCursor(AddonCursorType.Hand);
		eventData.SetHandled();
	}
	
	private void ClickDragStart(AddonEventData eventData) {
		isClickDragDragging = true;
		clickDragClickStart = eventData.GetMousePosition();
		SetCursor(AddonCursorType.Grab);

		clickDragEventListener?.AddEvent(AtkEventType.MouseMove, 1, InternalResNode);
		clickDragEventListener?.AddEvent(AtkEventType.MouseUp, 1, InternalResNode);

		eventData.SetHandled();
	}
	
	private void ClickDragMouseOut(AddonEventData eventData) {
		if (isClickDragDragging) return;

		ResetCursor();
		eventData.SetHandled();
	}
}