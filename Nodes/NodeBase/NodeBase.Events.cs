using System;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Nodes;

public abstract unsafe partial class NodeBase {
	public Action? MouseOver { get; set; }
	private IAddonEventHandle? MouseOverHandle { get; set; } 
	
	public Action? MouseOut { get; set; }
	private IAddonEventHandle? MouseOutHandle { get; set; } 

	public Action? MouseClick { get; set; }
	private IAddonEventHandle? MouseClickHandle { get; set; } 

	public SeString? Tooltip { get; set; }
	
	public Action? MouseDown { get; set; }
	private IAddonEventHandle? MouseDownHandle { get; set; }
	
	public Action? MouseUp { get; set; }
	private IAddonEventHandle? MouseUpHandle { get; set; }
	
	public Action? InputReceived { get; set; }
	private IAddonEventHandle? InputReceivedHandle { get; set; }
	
	private IAddonEventManager? EventManager { get; set; }

	public virtual void EnableEvents(IAddonEventManager eventManager, AtkUnitBase* addon) {
		if (MouseOver is not null || MouseOut is not null || MouseClick is not null || Tooltip is not null ||
		    MouseDown is not null || MouseUp is not null || InputReceived is not null || InputReceivedHandle is not null) {
			AddFlags(NodeFlags.EmitsEvents | NodeFlags.HasCollision | NodeFlags.RespondToMouse);
		}

		if ((MouseOver is not null || Tooltip is not null) && MouseOverHandle is null) {
			MouseOverHandle = eventManager.AddEvent((nint) addon, (nint) InternalResNode, AddonEventType.MouseOver, HandleEvents);
		}

		if ((MouseOut is not null || Tooltip is not null) && MouseOutHandle is null) {
			MouseOutHandle = eventManager.AddEvent((nint) addon, (nint) InternalResNode, AddonEventType.MouseOut, HandleEvents);
		}

		if (MouseClick is not null && MouseClickHandle is null) {
			MouseOverHandle ??= eventManager.AddEvent((nint) addon, (nint) InternalResNode, AddonEventType.MouseOver, HandleEvents);
			MouseOutHandle ??= eventManager.AddEvent((nint) addon, (nint) InternalResNode, AddonEventType.MouseOut, HandleEvents);
			MouseClickHandle = eventManager.AddEvent((nint) addon, (nint) InternalResNode, AddonEventType.MouseClick, HandleEvents);
		}

		if (MouseDown is not null) {
			MouseDownHandle ??= eventManager.AddEvent((nint) addon, (nint) InternalResNode, AddonEventType.MouseDown, HandleEvents);
		}

		if (MouseUp is not null) {
			MouseUpHandle ??= eventManager.AddEvent((nint) addon, (nint) InternalResNode, AddonEventType.MouseUp, HandleEvents);
		}

		if (InputReceived is not null) {
			InputReceivedHandle ??= eventManager.AddEvent((nint) addon, (nint) InternalResNode, AddonEventType.InputReceived, HandleEvents);
		}

		EventManager = eventManager;
	}
	
	public virtual void DisableEvents(IAddonEventManager eventManager) {
		RemoveFlags(NodeFlags.EmitsEvents | NodeFlags.HasCollision | NodeFlags.RespondToMouse);

		if (MouseOverHandle is not null) {
			eventManager.RemoveEvent(MouseOverHandle);
			MouseOverHandle = null;
		}

		if (MouseOutHandle is not null) {
			eventManager.RemoveEvent(MouseOutHandle);
			MouseOutHandle = null;
		}

		if (MouseClickHandle is not null) {
			eventManager.RemoveEvent(MouseClickHandle);
			MouseClickHandle = null;
		}

		if (MouseDownHandle is not null) {
			eventManager.RemoveEvent(MouseDownHandle);
		}

		if (MouseUpHandle is not null) {
			eventManager.RemoveEvent(MouseUpHandle);
		}

		if (InputReceivedHandle is not null) {
			eventManager.RemoveEvent(InputReceivedHandle);
		}
		
		EventManager = eventManager;
	}
	
	public virtual void UpdateEvents(IAddonEventManager eventManager, AtkUnitBase* addon) {
		DisableEvents(eventManager);
		EnableEvents(eventManager, addon);
	}

	private void HandleEvents(AddonEventType atkEventType, IntPtr atkUnitBase, IntPtr atkResNode) {
		var parentAddon = (AtkUnitBase*) atkUnitBase;

		if (!IsVisible) return;

		switch (atkEventType) {
			case AddonEventType.MouseOver:
				if (Tooltip is not null) {
					AtkStage.Instance()->TooltipManager.ShowTooltip(parentAddon->Id, (AtkResNode*) atkResNode, Tooltip.Encode());
				}

				if (MouseClick is not null) {
					EventManager?.SetCursor(AddonCursorType.Clickable);
				}
				
				MouseOver?.Invoke();
				break;

			case AddonEventType.MouseOut:
				if (Tooltip is not null) {
					AtkStage.Instance()->TooltipManager.HideTooltip(parentAddon->Id);
				}
				
				if (MouseClick is not null) {
					EventManager?.ResetCursor();
				}
				
				MouseOut?.Invoke();
				break;
			
			case AddonEventType.MouseClick:
				MouseClick?.Invoke();
				break;
			
			case AddonEventType.MouseDown:
				MouseDown?.Invoke();
				break;
			
			case AddonEventType.MouseUp:
				MouseUp?.Invoke();
				break;
			
			case AddonEventType.InputReceived:
				InputReceived?.Invoke();
				break;
		}
	}

	protected void SetCursor(AddonCursorType cursor) {
		EventManager?.SetCursor(cursor);
	}

	protected void ResetCursor() {
		EventManager?.ResetCursor();
	}
}