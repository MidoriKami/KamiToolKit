using System;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Extensions;
using KamiToolKit.Nodes;

namespace KamiToolKit.System;

public abstract unsafe partial class NodeBase {

	private SimpleComponentNode? clickDragContainer;
	
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
	public void EnableClickDrag() {
		if (clickDragEventsRegistered) return;
		if (EventAddonPointer is null) {
			Log.Warning("Attempted to enable node events when not attached to native tree. Aborting.");
			return;
		}

		clickDragContainer = new SimpleComponentNode {
			Size = Size + new Vector2(32.0f, 32.0f), 
			Position = new Vector2(-16.0f, -16.0f),
			IsVisible = true,
			EventFlagsSet = true,
		};
		
		clickDragContainer.AttachNode(this);
		
		clickDragContainer?.AddEvent(AddonEventType.MouseOver, ClickDragMouseOver);
		clickDragContainer?.AddEvent(AddonEventType.MouseDown, ClickDragStart);
		clickDragContainer?.AddEvent(AddonEventType.MouseOut, ClickDragMouseOut);
		
		clickDragEventListener ??= new ViewportEventListener(OnViewportEvent);
		
		clickDragEventsRegistered = true;
	}

	// Note: Event flags must be set to allow drag drop
	public void DisableClickDrag() {
		if (!clickDragEventsRegistered) return;
		
		clickDragContainer?.RemoveEvent(AddonEventType.MouseOver, ClickDragMouseOver);
		clickDragContainer?.RemoveEvent(AddonEventType.MouseDown, ClickDragStart);
		clickDragContainer?.RemoveEvent(AddonEventType.MouseOut, ClickDragMouseOut);
		
		clickDragContainer?.Dispose();
		clickDragContainer = null;
		
		clickDragEventListener?.Dispose();
		clickDragEventListener = null;
		
		clickDragEventsRegistered = false;
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
				
				clickDragEventListener!.RemoveEvent(AtkEventType.MouseMove);
				clickDragEventListener!.RemoveEvent(AtkEventType.MouseUp);
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

		clickDragEventListener?.AddEvent(AtkEventType.MouseMove, InternalResNode);
		clickDragEventListener?.AddEvent(AtkEventType.MouseUp, InternalResNode);

		eventData.SetHandled();
	}
	
	private void ClickDragMouseOut(AddonEventData eventData) {
		if (isClickDragDragging) return;

		ResetCursor();
		eventData.SetHandled();
	}
}