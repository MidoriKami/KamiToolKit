using System;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Classes;

internal class EventHandlerInfo {
    public AtkEventListener.Delegates.ReceiveEvent? OnReceiveEventDelegate;
    public Action? OnActionDelegate;
}
