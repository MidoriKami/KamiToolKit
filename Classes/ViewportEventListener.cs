﻿using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;

namespace KamiToolKit.Classes;

public unsafe class ViewportEventListener(AtkEventListener.Delegates.ReceiveEvent eventHandler) : CustomEventListener(eventHandler) {
    public void AddEvent(AtkEventType eventType, AtkResNode* node) {
        DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
            Log.Verbose($"正在注册 Viewport 事件：{eventType}");
            AtkStage.Instance()->ViewportEventManager.RegisterEvent(eventType, 0, node, (AtkEventTarget*)node, EventListener, false);
        });
    }

    public void RemoveEvent(AtkEventType eventType) {
        DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
            Log.Verbose($"正在注销 Viewport 事件：{eventType}");
            AtkStage.Instance()->ViewportEventManager.UnregisterEvent(eventType, 0, EventListener, false);
        });
    }

    // Todo: maybe use Unregister All event type to do this better?
    public override void Dispose() {
        var eventList = new List<Pointer<AtkEvent>>();

        var currentEvent = AtkStage.Instance()->ViewportEventManager.Event;
        while (currentEvent is not null) {
            eventList.Add(currentEvent);
            currentEvent = currentEvent->NextEvent;
        }

        foreach (var atkEvent in eventList) {
            RemoveEvent(atkEvent.Value->State.EventType);
        }

        base.Dispose();
    }
}
