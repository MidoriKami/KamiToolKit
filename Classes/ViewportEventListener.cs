using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Internal.Classes;

namespace KamiToolKit.Classes;

/// <summary>
/// Event listener that wires events to the global viewport.
/// <em>Warning, these events may be triggered every frame multiple times per frame, and are not intended to be long-lived.</em>
/// </summary>
public unsafe class ViewportEventListener(AtkEventListener.Delegates.ReceiveEvent eventHandler) : CustomEventListener(eventHandler) {

    /// <summary>
    /// Registers a viewport event for the specified node.
    /// </summary>
    /// <remarks>
    /// This can only be called from the games main thread.
    /// </remarks>
    /// <param name="eventType">Event Type to listen for.</param>
    /// <param name="node">Node to pass when the callback is triggered.</param>
    public void AddEvent(AtkEventType eventType, AtkResNode* node) {
        ThreadSafety.AssertMainThread();

        Services.Log.Verbose($"Registering ViewportEvent: {eventType}");
        AtkStage.Instance()->ViewportEventManager.RegisterEvent(eventType, 0, node, &node->AtkEventTarget, this, false);
    }

    /// <summary>
    /// Removes a viewport event by type.
    /// </summary>
    /// <remarks>
    /// This can only be called from the games main thread.
    /// </remarks>
    /// <param name="eventType">Event Type to no longer listen for.</param>
    public void RemoveEvent(AtkEventType eventType) {
        ThreadSafety.AssertMainThread();

        Services.Log.Verbose($"Unregistering ViewportEvent: {eventType}");
        AtkStage.Instance()->ViewportEventManager.UnregisterEvent(eventType, 0, this, false);
    }

    /// <summary>
    /// Unregisters all events and disposes this instance.
    /// </summary>
    /// <remarks>
    /// This can only be called from the games main thread.
    /// </remarks>
    public override void Dispose() {
        ThreadSafety.AssertMainThread();

        Services.Log.Verbose("Disposing ViewportEventListener");

        RemoveEvent(AtkEventType.UnregisterAll);
        base.Dispose();
    }
}
