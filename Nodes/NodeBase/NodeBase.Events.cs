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

	public void EnableEvents(IAddonEventManager eventManager, AtkUnitBase* addon) {
		AddFlags(NodeFlags.EmitsEvents | NodeFlags.HasCollision | NodeFlags.RespondToMouse);

		if ((MouseOver is not null || Tooltip is not null) && MouseOverHandle is null) {
			MouseOverHandle = eventManager.AddEvent((nint) addon, (nint) InternalResNode, AddonEventType.MouseOver, HandleEvents);
		}

		if ((MouseOut is not null || Tooltip is not null) && MouseOutHandle is null) {
			MouseOutHandle = eventManager.AddEvent((nint) addon, (nint) InternalResNode, AddonEventType.MouseOut, HandleEvents);
		}

		if (MouseClick is not null && MouseClickHandle is null) {
			MouseClickHandle = eventManager.AddEvent((nint) addon, (nint) InternalResNode, AddonEventType.MouseClick, HandleEvents);
		}
	}
	
	public void DisableEvents(IAddonEventManager eventManager) {
		RemoveFlags(NodeFlags.EmitsEvents | NodeFlags.HasCollision | NodeFlags.RespondToMouse);

		if (MouseOver is null && Tooltip is null && MouseOverHandle is not null) {
			eventManager.RemoveEvent(MouseOverHandle);
			MouseOverHandle = null;
		}

		if (MouseOut is null && Tooltip is null && MouseOutHandle is not null) {
			eventManager.RemoveEvent(MouseOutHandle);
			MouseOutHandle = null;
		}

		if (MouseClick is null && MouseClickHandle is not null) {
			eventManager.RemoveEvent(MouseClickHandle);
			MouseClickHandle = null;
		}
	}
	
	public void UpdateEvents(IAddonEventManager eventManager, AtkUnitBase* addon) {
		DisableEvents(eventManager);
		EnableEvents(eventManager, addon);
	}

	private void HandleEvents(AddonEventType atkEventType, IntPtr atkUnitBase, IntPtr atkResNode) {
		var parentAddon = (AtkUnitBase*) atkUnitBase;

		switch (atkEventType) {
			case AddonEventType.MouseOver:
				if (Tooltip is not null) {
					AtkStage.Instance()->TooltipManager.ShowTooltip(parentAddon->Id, (AtkResNode*) atkResNode, Tooltip.Encode());
				}

				MouseOver?.Invoke();
				break;

			case AddonEventType.MouseOut:
				if (Tooltip is not null) {
					AtkStage.Instance()->TooltipManager.HideTooltip(parentAddon->Id);
				}
				
				MouseOut?.Invoke();
				break;
			
			case AddonEventType.MouseClick:
				MouseClick?.Invoke();
				break;
		}
	}
}