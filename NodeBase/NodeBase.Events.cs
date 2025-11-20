using System;
using System.Collections.Generic;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;
using Newtonsoft.Json;

namespace KamiToolKit;

internal class EventHandlerInfo {
    public AtkEventListener.Delegates.ReceiveEvent? OnReceiveEventDelegate;
    public Action? OnActionDelegate;
}

public abstract unsafe partial class NodeBase {

    private CustomEventListener? nodeEventListener;
    private Dictionary<AtkEventType, EventHandlerInfo> eventHandlers = [];

    public virtual ReadOnlySeString? Tooltip {
        get;
        set {
            field = value;
            switch (value) {
                case { IsEmpty: false } when !TooltipRegistered: 
                    AddEvent(AtkEventType.MouseOver, ShowTooltip);
                    AddEvent(AtkEventType.MouseOut, HideTooltip);
                    OnVisibilityToggled += ToggleCollisionFlag;

                    if (this is not ComponentNode && IsVisible) {
                        AddFlags(NodeFlags.HasCollision);
                    }

                    TooltipRegistered = true;
                    break;

                case null when TooltipRegistered: {
                    RemoveEvent(AtkEventType.MouseOver, ShowTooltip);
                    RemoveEvent(AtkEventType.MouseOut, HideTooltip);
                    OnVisibilityToggled -= ToggleCollisionFlag;

                    TooltipRegistered = false;
                    break;
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

        SetNodeEventFlags(eventType);

        if (eventHandlers.TryAdd(eventType, new EventHandlerInfo { OnActionDelegate = callback })) {
            Log.Verbose($"[{eventType}] Registered for {GetType()} [{(nint)ResNode:X}]");
            ResNode->AtkEventManager.RegisterEvent(eventType, 0, this, this, nodeEventListener, false);
        }
        else {
            eventHandlers[eventType].OnActionDelegate += callback;
        }
    }

    public void AddEvent(AtkEventType eventType, AtkEventListener.Delegates.ReceiveEvent callback) {
        nodeEventListener ??= new CustomEventListener(HandleEvents);

        SetNodeEventFlags(eventType);

        if (eventHandlers.TryAdd(eventType, new EventHandlerInfo { OnReceiveEventDelegate = callback })) {
            Log.Verbose($"[{eventType}] Registered for {GetType()} [{(nint)ResNode:X}]");
            ResNode->AtkEventManager.RegisterEvent(eventType, 0, this, this, nodeEventListener, false);
        }
        else {
            eventHandlers[eventType].OnReceiveEventDelegate += callback;
        }
    }
    
    public void RemoveEvent(AtkEventType eventType) {
        if (nodeEventListener is null) return;

        if (eventHandlers.Remove(eventType)) {
            Log.Verbose($"[{eventType}] Unregistered from {GetType()} [{(nint)ResNode:X}]");
            ResNode->AtkEventManager.UnregisterEvent(eventType, 0, nodeEventListener, false);
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
            ResNode->AtkEventManager.UnregisterEvent(AtkEventType.UnregisterAll, 0, nodeEventListener, false);
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
    
    private void SetNodeEventFlags(AtkEventType eventType) {
        switch (eventType) {
            // Hover events need to propagate down to trigger various timelines
            case AtkEventType.MouseOver:
            case AtkEventType.MouseOut:
                AddFlags(NodeFlags.EmitsEvents, NodeFlags.RespondToMouse);
                break;

            // Any kind of direct interaction should be a blocking event
            // set HasCollision to prevent events from propagating
            case AtkEventType.MouseDown:
            case AtkEventType.MouseUp:
            case AtkEventType.MouseMove:
            case AtkEventType.MouseWheel:
            case AtkEventType.MouseClick:
                AddFlags(NodeFlags.EmitsEvents, NodeFlags.RespondToMouse, NodeFlags.HasCollision);
                break;
            
            // ButtonClick is mostly used as an event that native calls back to, when interacting with buttons
            // We do not want to re-emit, or block events in this case
            case AtkEventType.ButtonClick:
                break;
        }
    }

    public void ShowTooltip() {
        if (Tooltip is not null && TooltipRegistered && ParentAddon is not null) {
            using var stringBuilder = new RentedSeStringBuilder();
            AtkStage.Instance()->TooltipManager.ShowTooltip(ParentAddon->Id, ResNode, stringBuilder.Builder.Append(Tooltip).GetViewAsSpan());
        }
    }

    public void HideTooltip() {
        if (ParentAddon is null) return;

        AtkStage.Instance()->TooltipManager.HideTooltip(ParentAddon->Id);
    }
}
