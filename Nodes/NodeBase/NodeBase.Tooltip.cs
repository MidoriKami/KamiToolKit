using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Nodes;

public abstract unsafe partial class NodeBase {
    private readonly List<IAddonEventHandle?> tooltipHandles = [];
    private IAddonEventManager? tooltipAddonEventManager;
    
    public SeString? Tooltip { get; set; }
    
    public void EnableTooltip(IAddonEventManager eventManager, void* addon) {
        var atkUnitBase = (AtkUnitBase*) addon;
    
        if (tooltipHandles.Count == 0) {
            AddTooltipEvents(eventManager, atkUnitBase);
        }
    }
    
    public void DisableTooltip() {
        if (tooltipHandles.Count != 0) {
            RemoveTooltipEvents();
        }
    }
    
    private void AddTooltipEvents(IAddonEventManager eventManager, AtkUnitBase* addon) {
        tooltipAddonEventManager ??= eventManager;

        if (addon->IsReady && addon->UldManager.LoadedState is AtkLoadState.Loaded) {
            addon->UpdateCollisionNodeList(false);
        }

        tooltipHandles.AddRange([
            eventManager.AddEvent((nint) addon, (nint) InternalResNode, AddonEventType.MouseOver, HandleTooltipEvent),
            eventManager.AddEvent((nint) addon, (nint) InternalResNode, AddonEventType.MouseOut, HandleTooltipEvent),
        ]);
        
        AddFlags(NodeFlags.EmitsEvents | NodeFlags.HasCollision | NodeFlags.RespondToMouse);
    }
    
    private void RemoveTooltipEvents() {
        foreach (var tooltipHandle in tooltipHandles.OfType<IAddonEventHandle>()) {
            tooltipAddonEventManager!.RemoveEvent(tooltipHandle);
        }
        
        tooltipHandles.Clear();
    }

    private void HandleTooltipEvent(AddonEventType atkEventType, IntPtr atkUnitBase, IntPtr atkResNode) {
        if (Tooltip is null) return;

        var parentAddon = (AtkUnitBase*) atkUnitBase;

        switch (atkEventType) {
            case AddonEventType.MouseOver:
                AtkStage.Instance()->TooltipManager.ShowTooltip(parentAddon->Id, (AtkResNode*) atkResNode, Tooltip.Encode());
                break;

            case AddonEventType.MouseOut:
                AtkStage.Instance()->TooltipManager.HideTooltip(parentAddon->Id);
                break;
        }
    }
}