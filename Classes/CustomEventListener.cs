using System;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Dalamud;

namespace KamiToolKit.Classes;

public unsafe class CustomEventListener : IDisposable {

    private readonly AtkEventListener* eventListener;

    private AtkEventListener.Delegates.ReceiveEvent? receiveEventDelegate;
    private AtkEventListener.Delegates.ReceiveEvent? onCallbackTriggered;

    public CustomEventListener(AtkEventListener.Delegates.ReceiveEvent eventHandler) {
        receiveEventDelegate = eventHandler;

        onCallbackTriggered = HandleEventSafely;

        eventListener = NativeMemoryHelper.UiAlloc<AtkEventListener>();
        eventListener->VirtualTable = (AtkEventListener.AtkEventListenerVirtualTable*)NativeMemoryHelper.Malloc((ulong)sizeof(void*) * 3);
        eventListener->VirtualTable->Dtor = (delegate* unmanaged<AtkEventListener*, byte, AtkEventListener*>)(delegate* unmanaged<void>)&NullSub;
        eventListener->VirtualTable->ReceiveGlobalEvent = (delegate* unmanaged<AtkEventListener*, AtkEventType, int, AtkEvent*, AtkEventData*, void>)(delegate* unmanaged<void>)&NullSub;
        eventListener->VirtualTable->ReceiveEvent = (delegate* unmanaged<AtkEventListener*, AtkEventType, int, AtkEvent*, AtkEventData*, void>)Marshal.GetFunctionPointerForDelegate(onCallbackTriggered);
    }

    public virtual void Dispose() {
        if (eventListener is null) return;

        NativeMemoryHelper.Free(eventListener->VirtualTable, (ulong)sizeof(void*) * 3);
        NativeMemoryHelper.UiFree(eventListener);

        receiveEventDelegate = null;
        onCallbackTriggered = null;
    }

    [UnmanagedCallersOnly] private static void NullSub() { }

    public static implicit operator AtkEventListener*(CustomEventListener listener) => listener.eventListener;

    // A wrapper around the raw callback to ensure that exceptions are handled safely.
    private void HandleEventSafely(AtkEventListener* thisPtr, AtkEventType eventType, int param, AtkEvent* eventObject, AtkEventData* data) {
        try {
            receiveEventDelegate?.Invoke(thisPtr, eventType, param, eventObject, data);
        }
        catch (Exception e) {
            Services.Log.Exception(e);
        }
    }
}
