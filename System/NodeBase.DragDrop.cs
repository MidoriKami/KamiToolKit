using System;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Extensions;

namespace KamiToolKit.System;

public abstract unsafe partial class NodeBase {

	private CustomEventListener? customEventListener;

	private bool clickDragEventsRegistered;

	private bool isDragging;
	private Vector2 clickStart = Vector2.Zero;

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
		
		customEventListener ??= new CustomEventListener(OnViewportEvent);
		
		clickDragEventsRegistered = true;

		if (setEventFlags) {
			SetEventFlags();
		}
	}

	// Note: Event flags must be set to allow drag drop
	public void DisableClickDrag(bool clearEventFlags = false) {
		if (!clickDragEventsRegistered || customEventListener is null) return;
		
		RemoveEvent(AddonEventType.MouseOver, ClickDragMouseOver);
		RemoveEvent(AddonEventType.MouseDown, ClickDragStart);
		RemoveEvent(AddonEventType.MouseOut, ClickDragMouseOut);
		
		customEventListener?.Dispose();
		customEventListener = null;
		
		clickDragEventsRegistered = false;

		if (clearEventFlags) {
			ClearEventFlags();
		}
	}

	private void OnViewportEvent(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
		if (!isDragging) return;

		ref var mouseData = ref atkEventData->MouseData;
		
		switch (eventType) {
			case AtkEventType.MouseMove: {
				var mousePosition = new Vector2(mouseData.PosX,  mouseData.PosY);
		
				var newPosition = mousePosition;
				var delta = newPosition - clickStart;
		
				Position += delta;
				clickStart = newPosition;
			}
				break;

			case AtkEventType.MouseUp: {
				if (isDragging) {
					OnClickDragComplete?.Invoke();
				}
		
				isDragging = false;
				SetCursor(AddonCursorType.Hand);
				
				RemoveViewportEvent(AtkEventType.MouseMove);
				RemoveViewportEvent(AtkEventType.MouseUp);
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
		isDragging = true;
		clickStart = eventData.GetMousePosition();
		SetCursor(AddonCursorType.Grab);

		AddViewportEvent(AtkEventType.MouseMove);
		AddViewportEvent(AtkEventType.MouseUp);

		eventData.SetHandled();
	}
	
	private void ClickDragMouseOut(AddonEventData eventData) {
		if (isDragging) return;
		ResetCursor();
		
		eventData.SetHandled();
	}

	private void AddViewportEvent(AtkEventType eventType) {
		if (customEventListener is null) return;

		Log.Verbose($"Registering ViewportEvent: {eventType}");

		Experimental.Instance.RegisterViewportEvent?.Invoke(
			Experimental.Instance.ViewportEventManager,
			eventType,
			0,
			InternalResNode,
			(AtkEventTarget*) InternalResNode, 
			customEventListener.EventListener, 
			false);
	}

	private void RemoveViewportEvent(AtkEventType eventType) {
		if (customEventListener is null) return;
		
		Log.Verbose($"Unregistering ViewportEvent: {eventType}");

		Experimental.Instance.UnregisterViewportEvent?.Invoke(
			Experimental.Instance.ViewportEventManager,
			eventType,
			0,
			customEventListener.EventListener, 
			false);
	}
}