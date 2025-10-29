using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Classes;

public unsafe class ViewportEventListener(AtkEventListener.Delegates.ReceiveEvent eventHandler) : CustomEventListener(eventHandler) {
    public void AddEvent(AtkEventType eventType, AtkResNode* node) {
        DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
            Log.Verbose($"Registering ViewportEvent: {eventType}");
            AtkStage.Instance()->ViewportEventManager.RegisterEvent(eventType, 0, node, &node->AtkEventTarget, EventListener, false);
        });
    }

    public void RemoveEvent(AtkEventType eventType) {
        DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
            Log.Verbose($"Unregistering ViewportEvent: {eventType}");
            AtkStage.Instance()->ViewportEventManager.UnregisterEvent(eventType, 0, EventListener, false);
        });
    }

    public override void Dispose() {
        Log.Verbose("Disposing ViewportEventListener");

        RemoveEvent(AtkEventType.UnregisterAll);
        base.Dispose();
    }
}
