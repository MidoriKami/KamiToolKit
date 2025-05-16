using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Newtonsoft.Json;

namespace KamiToolKit.System;

public abstract unsafe partial class NodeBase {

	private readonly Dictionary<AddonEventType, EventHandler> eventHandlers = [];

	public bool IsEventRegistered(AddonEventType eventType) 
		=> eventHandlers.ContainsKey(eventType);
	
	public void AddEvent(AddonEventType eventType, Action action) {
		// Check if this eventType is already registered
		if (eventHandlers.TryGetValue(eventType, out var handler)) {
			handler.EventAction += action;
		}
		else {
			eventHandlers.Add(eventType, handler = new EventHandler {
				EventAction = action,
			});
			
			if (EventsActive) {
				handler.EventHandle ??= EventManager.AddEvent((nint) EventAddonPointer, (nint) InternalResNode, eventType, HandleEvents);
			}
			
			// If we have added a click event, we need to also make the cursor change when hovering this node
			if (eventType is AddonEventType.MouseClick && !CursorEventsSet) {
				AddEvent(AddonEventType.MouseOver, SetCursorMouseover);
				AddEvent(AddonEventType.MouseOut, ResetCursorMouseover);
				CursorEventsSet = true;
			}
		}
	}

	public void RemoveEvent(AddonEventType eventType, Action action) {
		if (eventHandlers.TryGetValue(eventType, out var handler)) {
			if (handler.EventAction is not null) {
				handler.EventAction -= action;
			}
			
			// If we removed the last remaining action for this Event Type
			if (handler is { EventAction: null, EventHandle: not null } ) {

				// Remove this event handler entirely
				eventHandlers.Remove(eventType);
				
				// And if events are currently active, unregister them
				if (EventsActive) {
					EventManager.RemoveEvent(handler.EventHandle);
					handler.EventHandle = null;
				}
				
				// If we removed the last MouseClick event, we should also remove the cursor modification events
				if (eventType is AddonEventType.MouseClick && CursorEventsSet) {
					RemoveEvent(AddonEventType.MouseOver, SetCursorMouseover);
					RemoveEvent(AddonEventType.MouseOut, ResetCursorMouseover);
					CursorEventsSet = false;
				}
			}
		}
	}

	private void HandleEvents(AddonEventType atkEventType, IntPtr atkUnitBase, IntPtr atkResNode) {
		if (!IsVisible) return;

		if (eventHandlers.TryGetValue(atkEventType, out var handler)) {
			handler.EventAction?.Invoke();
		}
	}

	private SeString? internalTooltip;

	[JsonIgnore] public SeString? Tooltip {
		get => internalTooltip;
		set {
			if (value is not null) {
				internalTooltip = value;

				if (!TooltipRegistered) {
					AddEvent(AddonEventType.MouseOver, ShowTooltip);
					AddEvent(AddonEventType.MouseOut, HideTooltip);
					TooltipRegistered = true;
				}
			}
			else if (value is null) {
				if (TooltipRegistered) {
					RemoveEvent(AddonEventType.MouseOver, ShowTooltip);
					RemoveEvent(AddonEventType.MouseOut, HideTooltip);
					TooltipRegistered = false;
				}
			}
		}
	}

	public string TooltipString {
		get => Tooltip?.ToString() ?? string.Empty;
		set => Tooltip = value;
	}
	
	private IAddonEventManager? EventManager { get; set; }
	private AtkUnitBase* EventAddonPointer { get; set; }
	
	[MemberNotNullWhen(true, nameof(EventManager))]
	private bool EventsActive { get; set; }
	private bool TooltipRegistered { get; set; }
	private bool CursorEventsSet { get; set; }

	[JsonIgnore] public bool EventFlagsSet {
		get => NodeFlags.HasFlag(NodeFlags.EmitsEvents) &&
		       NodeFlags.HasFlag(NodeFlags.HasCollision) &&
		       NodeFlags.HasFlag(NodeFlags.RespondToMouse);
		set {
			if (value) {
				SetEventFlags();
			}
			else {
				ClearEventFlags();
			}
		}
	}

	public virtual void EnableEvents(IAddonEventManager eventManager, AtkUnitBase* addon) {
		EventManager ??= eventManager;
		EventAddonPointer = addon;
		EventsActive = true;

		foreach (var (eventType, handler) in eventHandlers) {
			handler.EventHandle = EventManager.AddEvent((nint) addon, (nint) InternalResNode, eventType, HandleEvents);
		}
	}
	
	public virtual void DisableEvents(IAddonEventManager eventManager) {
		EventAddonPointer = null;
		EventsActive = false;
		
		foreach (var (_, handler) in eventHandlers) {
			if (handler.EventHandle is not null) {
				
				eventManager.RemoveEvent(handler.EventHandle);
				handler.EventHandle = null;
			}
		}
	}

	/// <summary>
	/// Sets EmitsEvents, HasCollision, and RespondToMouse flags for the EventTarget node to allow it to be interactable.
	/// </summary>
	public void SetEventFlags() {
		AddFlags(NodeFlags.EmitsEvents | NodeFlags.HasCollision | NodeFlags.RespondToMouse);
	}

	/// <summary>
	/// Clears EmitsEvents, HasCollision, and RespondToMouse flags, for the EventTarget node to disable it stealing inputs
	/// </summary>
	public void ClearEventFlags() {
		RemoveFlags(NodeFlags.EmitsEvents | NodeFlags.HasCollision | NodeFlags.RespondToMouse);
	}

	protected void SetCursor(AddonCursorType cursor) {
		EventManager?.SetCursor(cursor);
	}

	protected void ResetCursor() {
		EventManager?.ResetCursor();
	}

	private void ShowTooltip() {
		if (Tooltip is not null) {
			AtkStage.Instance()->TooltipManager.ShowTooltip(EventAddonPointer->Id, InternalResNode, Tooltip.Encode());
		}
	}

	private void HideTooltip() {
		if (Tooltip is not null) {
			AtkStage.Instance()->TooltipManager.HideTooltip(EventAddonPointer->Id);
		}
	}

	private void SetCursorMouseover()
		=> SetCursor(AddonCursorType.Clickable);

	private void ResetCursorMouseover()
		=> ResetCursor();
}

public class EventHandler {
	public required Action? EventAction { get; set; }
	public IAddonEventHandle? EventHandle { get; set; }
}