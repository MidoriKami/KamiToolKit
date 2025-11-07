using System;
using System.Collections.Generic;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using Newtonsoft.Json;

namespace KamiToolKit.System;

internal class EventHandlerInfo {
    public AtkEventListener.Delegates.ReceiveEvent? OnReceiveEventDelegate;
    public Action? OnActionDelegate;
}

public abstract unsafe partial class NodeBase {

    private CustomEventListener? nodeEventListener;
    private Dictionary<AtkEventType, EventHandlerInfo> eventHandlers = [];

    public virtual SeString? Tooltip {
        get;
        set {
            if (value is not null && !value.TextValue.IsNullOrEmpty()) {
                field = value;

                if (!TooltipRegistered) {
                    AddEvent(AtkEventType.MouseOver, ShowTooltip);
                    AddEvent(AtkEventType.MouseOut, HideTooltip);
                    OnVisibilityToggled += ToggleCollisionFlag;

                    if (this is not ComponentNode) {
                        AddFlags(NodeFlags.HasCollision);
                    }

                    TooltipRegistered = true;
                }
            }
            else if (value is null) {
                if (TooltipRegistered) {
                    RemoveEvent(AtkEventType.MouseOver, ShowTooltip);
                    RemoveEvent(AtkEventType.MouseOut, HideTooltip);
                    OnVisibilityToggled -= ToggleCollisionFlag;

                    TooltipRegistered = false;
                }
            }
        }
    }

    [JsonProperty] public string TooltipString {
        get => Tooltip?.ToString() ?? string.Empty;
        set => Tooltip = value;
    }

    protected bool TooltipRegistered { get; set; }

    public void AddEvent(AtkEventType eventType, Action callback) {
        nodeEventListener ??= new CustomEventListener(HandleEvents);

        switch (eventType) {
            case AtkEventType.MouseOver:
            case AtkEventType.MouseOut:
                AddFlags(NodeFlags.RespondToMouse);
                break;
            
            case AtkEventType.MouseDown:
            case AtkEventType.MouseUp:
            case AtkEventType.MouseMove:
                AddFlags(NodeFlags.HasCollision, NodeFlags.RespondToMouse);
                break;
                
            case AtkEventType.MouseClick:
                AddFlags(NodeFlags.HasCollision);
                break;
        }

        AddFlags(NodeFlags.EmitsEvents);

        if (eventHandlers.TryAdd(eventType, new EventHandlerInfo { OnActionDelegate = callback })) {
            Log.Verbose($"[{eventType}] Registered for {GetType()} [{(nint)InternalResNode:X}]");
            InternalResNode->AtkEventManager.RegisterEvent(eventType, 0, this, this, nodeEventListener, false);
        }
        else {
            eventHandlers[eventType].OnActionDelegate += callback;
        }
    }
    
    public void AddEvent(AtkEventType eventType, AtkEventListener.Delegates.ReceiveEvent callback) {
        nodeEventListener ??= new CustomEventListener(HandleEvents);

        if (eventHandlers.TryAdd(eventType, new EventHandlerInfo { OnReceiveEventDelegate = callback })) {
            Log.Verbose($"[{eventType}] Registered for {GetType()} [{(nint)InternalResNode:X}]");
            InternalResNode->AtkEventManager.RegisterEvent(eventType, 0, this, this, nodeEventListener, false);
        }
        else {
            eventHandlers[eventType].OnReceiveEventDelegate += callback;
        }
    }
    
    public void RemoveEvent(AtkEventType eventType) {
        if (nodeEventListener is null) return;

        if (eventHandlers.Remove(eventType)) {
            Log.Verbose($"[{eventType}] Unregistered from {GetType()} [{(nint)InternalResNode:X}]");
            InternalResNode->AtkEventManager.UnregisterEvent(eventType, 0, nodeEventListener, false);
        }

        // If we have removed the last event, free the event listener
        if (eventHandlers.Keys.Count is 0) {
            nodeEventListener.Dispose();
            nodeEventListener = null;
        }
    }

    public void RemoveEvent(AtkEventType eventType, Action callback) {
        if (nodeEventListener is null) return;

        if (eventHandlers.TryGetValue(eventType, out var handler)) {
            handler.OnActionDelegate -= callback;
            
            if (handler.OnReceiveEventDelegate is null && handler.OnActionDelegate is null) {
                RemoveEvent(eventType);
            }
        }
    }

    public void RemoveEvent(AtkEventType eventType, AtkEventListener.Delegates.ReceiveEvent callback) {
        if (nodeEventListener is null) return;

        if (eventHandlers.TryGetValue(eventType, out var handler)) {
            handler.OnReceiveEventDelegate -= callback;
            
            if (handler.OnReceiveEventDelegate is null && handler.OnActionDelegate is null) {
                RemoveEvent(eventType);
            }
        }
    }

    private void DisposeEvents() {
        if (nodeEventListener is not null) {
            InternalResNode->AtkEventManager.UnregisterEvent(AtkEventType.UnregisterAll, 0, nodeEventListener, false);
        }

        eventHandlers.Clear();

        nodeEventListener?.Dispose();
        nodeEventListener = null;
    }

    private void HandleEvents(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        if (!IsVisible) return;

        if (eventHandlers.TryGetValue(eventType, out var handler)) {
            handler.OnActionDelegate?.Invoke();
            handler.OnReceiveEventDelegate?.Invoke(thisPtr, eventType, eventParam, atkEvent, atkEventData);
        }
    }

    private void ToggleCollisionFlag(bool isVisible) {
        if (this is ComponentNode) return;

        if (isVisible) {
            AddFlags(NodeFlags.HasCollision);
        }
        else {
            RemoveFlags(NodeFlags.HasCollision);
        }
    }

    protected static void SetCursor(AddonCursorType cursor)
        => DalamudInterface.Instance.AddonEventManager.SetCursor(cursor);

    protected static void ResetCursor()
        => DalamudInterface.Instance.AddonEventManager.ResetCursor();

    public void ShowTooltip() {
        if (Tooltip is not null && TooltipRegistered) {
            var addon = GetAddonForNode(InternalResNode);
            AtkStage.Instance()->TooltipManager.ShowTooltip(addon->Id, InternalResNode, Tooltip.Encode());
        }
    }

    public void HideTooltip() {
        var addon = GetAddonForNode(InternalResNode);
        if (addon is null) return;

        AtkStage.Instance()->TooltipManager.HideTooltip(addon->Id);
    }
}
