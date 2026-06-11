using System;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Internal.Classes;

internal class EventHandlerInfo {
    public AtkEventListener.Delegates.ReceiveEvent? OnReceiveEventDelegate;
    public Action? OnActionDelegate;
}
