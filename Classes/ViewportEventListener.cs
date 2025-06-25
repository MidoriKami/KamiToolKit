using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Classes;

public unsafe class ViewportEventListener(AtkEventListener.Delegates.ReceiveEvent eventHandler) : CustomEventListener(eventHandler) {

	public void AddEvent(AtkEventType eventType, uint param, AtkResNode* node) {
		Log.Verbose($"Registering ViewportEvent: {eventType}");
		Experimental.Instance.ViewportEventManager->RegisterEvent(eventType, param, node, (AtkEventTarget*) node, EventListener, false);
	}

	public void RemoveEvent(AtkEventType eventType, uint param) {
		Log.Verbose($"Unregistering ViewportEvent: {eventType}");
		Experimental.Instance.ViewportEventManager->UnregisterEvent(eventType, param, EventListener, false);
	}
}