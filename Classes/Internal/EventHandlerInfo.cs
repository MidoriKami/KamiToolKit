using System;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Classes.Internal;

internal class EventHandlerInfo {
    public AtkEventListener.Delegates.ReceiveEvent? OnReceiveEventDelegate;
    public Action? OnActionDelegate;
}
