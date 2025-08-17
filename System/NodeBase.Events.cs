using System;
using System.Collections.Generic;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Addon.Events.EventDataTypes;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using Newtonsoft.Json;

namespace KamiToolKit.System;

public class EventHandler {
    public required Action<AddonEventData>? EventAction { get; set; }
    public IAddonEventHandle? EventHandle { get; set; }
}

public abstract unsafe partial class NodeBase {

    private readonly Dictionary<AddonEventType, EventHandler> eventHandlers = [];

    public SeString? Tooltip {
        get;
        set {
            if (value is not null && !value.TextValue.IsNullOrEmpty()) {
                field = value;

                if (!TooltipRegistered) {
                    if (IsDragDropComponent()) {
                        AddEvent(AddonEventType.DragDropRollOver, ShowTooltip);
                        AddEvent(AddonEventType.DragDropRollOut, HideTooltip);
                    }
                    else {
                        AddEvent(AddonEventType.MouseOver, ShowTooltip);
                        AddEvent(AddonEventType.MouseOut, HideTooltip);
                    }

                    TooltipRegistered = true;
                }
            }
            else if (value is null) {
                if (TooltipRegistered) {
                    if (IsDragDropComponent()) {
                        RemoveEvent(AddonEventType.DragDropRollOver, ShowTooltip);
                        RemoveEvent(AddonEventType.DragDropRollOut, HideTooltip);
                    }
                    else {
                        RemoveEvent(AddonEventType.MouseOver, ShowTooltip);
                        RemoveEvent(AddonEventType.MouseOut, HideTooltip);
                    }

                    TooltipRegistered = false;
                }
            }
        }
    }

    [JsonProperty] public string TooltipString {
        get => Tooltip?.ToString() ?? string.Empty;
        set => Tooltip = value;
    }

    private AtkUnitBase* EventAddonPointer { get; set; }

    private bool EventsActive { get; set; }
    private bool TooltipRegistered { get; set; }
    private bool CursorEventsSet { get; set; }

    public bool EnableEventFlags {
        get => EventFlagsSet;
        set => EventFlagsSet = value;
    }

    public bool EventFlagsSet {
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

    public bool IsEventRegistered(AddonEventType eventType)
        => eventHandlers.ContainsKey(eventType);

    public void AddEvent(AddonEventType eventType, Action<AddonEventData> action, bool enableEventFlags = false, bool addClickHelpers = true) {
        // Check if this eventType is already registered
        if (eventHandlers.TryGetValue(eventType, out var handler)) {
            handler.EventAction += action;
        }
        else {
            eventHandlers.Add(eventType, handler = new EventHandler {
                EventAction = action,
            });

            if (EventsActive) {
                handler.EventHandle ??= DalamudInterface.Instance.AddonEventManager.AddEvent((nint)EventAddonPointer, (nint)InternalResNode, eventType, HandleEvents);
            }

            // If we have added a click event, we need to also make the cursor change when hovering this node
            if (eventType is AddonEventType.MouseClick && !CursorEventsSet && addClickHelpers) {
                AddEvent(AddonEventType.MouseOver, SetCursorMouseover);
                AddEvent(AddonEventType.MouseOut, ResetCursorMouseover);
                CursorEventsSet = true;
            }
        }

        if (!EventFlagsSet && enableEventFlags) {
            EventFlagsSet = true;
        }
    }

    public void RemoveEvent(AddonEventType eventType, Action<AddonEventData> action, bool clearEventFlags = false) {
        if (eventHandlers.TryGetValue(eventType, out var handler)) {
            if (handler.EventAction is not null) {
                handler.EventAction -= action;
            }

            // If we removed the last remaining action for this Event Type
            if (handler is { EventAction: null, EventHandle: not null }) {

                // Remove this event handler entirely
                eventHandlers.Remove(eventType);

                // And if events are currently active, unregister them
                if (EventsActive) {
                    DalamudInterface.Instance.AddonEventManager.RemoveEvent(handler.EventHandle);
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

        if (EventFlagsSet && clearEventFlags) {
            EventFlagsSet = false;
        }
    }

    private void HandleEvents(AddonEventType atkEventType, AddonEventData eventData) {
        if (!IsVisible) return;

        if (eventHandlers.TryGetValue(atkEventType, out var handler)) {
            handler.EventAction?.Invoke(eventData);
        }
    }

    internal void EnableEvents(AtkUnitBase* addon) {
        if (addon is null) return;
        if (EventsActive) return;

        EventAddonPointer = addon;
        EventsActive = true;

        foreach (var (eventType, handler) in eventHandlers) {
            handler.EventHandle = DalamudInterface.Instance.AddonEventManager.AddEvent((nint)addon, (nint)InternalResNode, eventType, HandleEvents);
        }

        VisitChildren(node => node?.EnableEvents(addon));
    }

    internal void DisableEvents() {
        if (!EventsActive) return;
        EventsActive = false;

        foreach (var (_, handler) in eventHandlers) {
            if (handler.EventHandle is not null) {

                DalamudInterface.Instance.AddonEventManager.RemoveEvent(handler.EventHandle);
                handler.EventHandle = null;
            }
        }

        VisitChildren(node => node?.DisableEvents());
    }

    /// <summary>
    ///     Sets EmitsEvents, HasCollision, and RespondToMouse flags for the EventTarget node to allow it to be interactable.
    /// </summary>
    public void SetEventFlags()
        => AddFlags(NodeFlags.EmitsEvents | NodeFlags.HasCollision | NodeFlags.RespondToMouse);

    /// <summary>
    ///     Clears EmitsEvents, HasCollision, and RespondToMouse flags, for the EventTarget node to disable it stealing inputs
    /// </summary>
    public void ClearEventFlags()
        => RemoveFlags(NodeFlags.EmitsEvents | NodeFlags.HasCollision | NodeFlags.RespondToMouse);

    protected void SetCursor(AddonCursorType cursor)
        => DalamudInterface.Instance.AddonEventManager.SetCursor(cursor);

    protected void ResetCursor()
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

    private void ShowTooltip(AddonEventData data)
        => ShowTooltip();

    private void HideTooltip(AddonEventData data)
        => HideTooltip();

    private void SetCursorMouseover(AddonEventData data)
        => SetCursor(AddonCursorType.Clickable);

    private void ResetCursorMouseover(AddonEventData data)
        => ResetCursor();
}
