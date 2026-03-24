using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Dalamud;

namespace KamiToolKit.Classes;

public unsafe class ViewportEventListener(AtkEventListener.Delegates.ReceiveEvent eventHandler) : CustomEventListener(eventHandler) {
    public void AddEvent(AtkEventType eventType, AtkResNode* node) {
        Services.Framework.RunOnFrameworkThread(() => {
            Services.Log.Verbose($"Registering ViewportEvent: {eventType}");
            AtkStage.Instance()->ViewportEventManager.RegisterEvent(eventType, 0, node, &node->AtkEventTarget, this, false);
        });
    }

    public void RemoveEvent(AtkEventType eventType) {
        Services.Framework.RunOnFrameworkThread(() => {
            Services.Log.Verbose($"Unregistering ViewportEvent: {eventType}");
            AtkStage.Instance()->ViewportEventManager.UnregisterEvent(eventType, 0, this, false);
        });
    }

    public override void Dispose() {
        Services.Log.Verbose("Disposing ViewportEventListener");

        RemoveEvent(AtkEventType.UnregisterAll);
        base.Dispose();
    }
}
