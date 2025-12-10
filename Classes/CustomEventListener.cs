using System;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Classes;

public unsafe class CustomEventListener : IDisposable {

    private readonly AtkEventListener* eventListener;

    private AtkEventListener.Delegates.ReceiveEvent? receiveEventDelegate;

    public CustomEventListener(AtkEventListener.Delegates.ReceiveEvent eventHandler) {
        receiveEventDelegate = eventHandler;

        eventListener = NativeMemoryHelper.UiAlloc<AtkEventListener>();
        eventListener->VirtualTable = (AtkEventListener.AtkEventListenerVirtualTable*)NativeMemoryHelper.Malloc((ulong)sizeof(void*) * 3);
        eventListener->VirtualTable->Dtor = (delegate* unmanaged<AtkEventListener*, byte, AtkEventListener*>)(delegate* unmanaged<void>)&NullSub;
        eventListener->VirtualTable->ReceiveGlobalEvent = (delegate* unmanaged<AtkEventListener*, AtkEventType, int, AtkEvent*, AtkEventData*, void>)(delegate* unmanaged<void>)&NullSub;
        eventListener->VirtualTable->ReceiveEvent = (delegate* unmanaged<AtkEventListener*, AtkEventType, int, AtkEvent*, AtkEventData*, void>)Marshal.GetFunctionPointerForDelegate(receiveEventDelegate);
    }

    public virtual void Dispose() {
        if (eventListener is null) return;

        NativeMemoryHelper.Free(eventListener->VirtualTable, (ulong)sizeof(void*) * 3);
        NativeMemoryHelper.UiFree(eventListener);

        receiveEventDelegate = null;
    }

    [UnmanagedCallersOnly] private static void NullSub() { }

    public static implicit operator AtkEventListener*(CustomEventListener listener) => listener.eventListener;
}
