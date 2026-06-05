using System;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.Internal;

namespace KamiToolKit.Classes;

/// <summary>
/// Managed wrapper around a AtkEventListener.
/// This class is intended to be used to wire native events to managed event handlers.
/// </summary>
/// <remarks>
/// This version is specifically to be used for ATK/UI events.
/// </remarks>
public unsafe class CustomEventListener : IDisposable {

    /// <summary>
    /// Public implicit operator to be able to use this instance as a AtkEventListener* directly.
    /// </summary>
    public static implicit operator AtkEventListener*(CustomEventListener listener) => listener.eventListener;

    public CustomEventListener(AtkEventListener.Delegates.ReceiveEvent eventHandler) {
        receiveEventDelegate = eventHandler;

        receiveEventWrapper = ReceiveEventWrapper;

        eventListener = NativeMemoryHelper.UiAlloc<AtkEventListener>();
        eventListener->VirtualTable = (AtkEventListener.AtkEventListenerVirtualTable*)NativeMemoryHelper.Malloc((ulong)sizeof(void*) * 3);
        eventListener->VirtualTable->Dtor = (delegate* unmanaged<AtkEventListener*, byte, AtkEventListener*>)(delegate* unmanaged<void>)&NullSub;
        eventListener->VirtualTable->ReceiveGlobalEvent = (delegate* unmanaged<AtkEventListener*, AtkEventType, int, AtkEvent*, AtkEventData*, void>)(delegate* unmanaged<void>)&NullSub;
        eventListener->VirtualTable->ReceiveEvent = (delegate* unmanaged<AtkEventListener*, AtkEventType, int, AtkEvent*, AtkEventData*, void>)Marshal.GetFunctionPointerForDelegate(receiveEventWrapper);
    }

    public virtual void Dispose() {
        if (eventListener is null) return;

        NativeMemoryHelper.Free(eventListener->VirtualTable, (ulong)sizeof(void*) * 3);
        NativeMemoryHelper.UiFree(eventListener);

        receiveEventDelegate = null;
        receiveEventWrapper = null;
    }

    private readonly AtkEventListener* eventListener;

    private AtkEventListener.Delegates.ReceiveEvent? receiveEventDelegate;
    private AtkEventListener.Delegates.ReceiveEvent? receiveEventWrapper;

    [UnmanagedCallersOnly] private static void NullSub() { }

    private void ReceiveEventWrapper(AtkEventListener* thisPtr, AtkEventType eventType, int param, AtkEvent* eventObject, AtkEventData* data) {
        try {
            receiveEventDelegate?.Invoke(thisPtr, eventType, param, eventObject, data);
        }
        catch (Exception e) {
            Services.Log.Exception(e);
        }
    }
}
