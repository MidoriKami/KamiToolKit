using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Addon.Events;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Nodes;

public abstract unsafe partial class NodeBase {
    private readonly List<IAddonEventHandle?> onClickHandles = [];
    private IAddonEventManager? onClickAddonEventManager;
    
    public Action? OnClick { get; set; }
    
    public void EnableOnClick(IAddonEventManager eventManager, void* addon) {
        var atkUnitBase = (AtkUnitBase*) addon;
    
        if (onClickHandles.Count == 0) {
            AddOnClickEvents(eventManager, atkUnitBase);
        }
    }
    
    public void DisableOnClick() {
        if (onClickHandles.Count != 0) {
            RemoveOnClickEvents();
        }
    }
    
    private void AddOnClickEvents(IAddonEventManager eventManager, AtkUnitBase* addon) {
        onClickAddonEventManager ??= eventManager;
        
        addon->UpdateCollisionNodeList(false);

        onClickHandles.AddRange([
            eventManager.AddEvent((nint) addon, (nint) InternalResNode, AddonEventType.MouseOver, HandleOnClickEvent),
            eventManager.AddEvent((nint) addon, (nint) InternalResNode, AddonEventType.MouseOut, HandleOnClickEvent),
            eventManager.AddEvent((nint) addon, (nint) InternalResNode, AddonEventType.MouseClick, HandleOnClickEvent),
        ]);
        
        AddFlags(NodeFlags.EmitsEvents | NodeFlags.HasCollision | NodeFlags.RespondToMouse);
    }
    
    private void RemoveOnClickEvents() {
        foreach (var tooltipHandle in onClickHandles.OfType<IAddonEventHandle>()) {
            onClickAddonEventManager!.RemoveEvent(tooltipHandle);
        }
        
        onClickHandles.Clear();
    }

    private void HandleOnClickEvent(AddonEventType atkEventType, IntPtr atkUnitBase, IntPtr atkResNode) {
        if (OnClick is null) return;

        switch (atkEventType) {
            case AddonEventType.MouseOver:
                onClickAddonEventManager!.SetCursor(AddonCursorType.Clickable);
                break;

            case AddonEventType.MouseOut:
                onClickAddonEventManager!.ResetCursor();
                break;

            case AddonEventType.MouseClick:
                OnClick.Invoke();
                break;
        }
    }
}