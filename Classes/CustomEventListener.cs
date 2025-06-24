using System;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Classes;

public unsafe class CustomEventListener : IDisposable {
    
    private AtkEventListener.Delegates.ReceiveEvent? receiveEventDelegate;
    public readonly AtkEventListener* EventListener;

    public CustomEventListener(AtkEventListener.Delegates.ReceiveEvent eventHandler) {
        receiveEventDelegate = eventHandler;

        EventListener = NativeMemoryHelper.UiAlloc<AtkEventListener>();
        EventListener->VirtualTable = (AtkEventListener.AtkEventListenerVirtualTable*) NativeMemoryHelper.Malloc((ulong) sizeof(void*) * 3);
        EventListener->VirtualTable->Dtor = (delegate* unmanaged<AtkEventListener*, byte, AtkEventListener*>)(delegate* unmanaged<void>) &NullSub;
        EventListener->VirtualTable->ReceiveGlobalEvent = (delegate* unmanaged<AtkEventListener*, AtkEventType, int, AtkEvent*, AtkEventData*, void>)(delegate* unmanaged<void>) &NullSub;
        EventListener->VirtualTable->ReceiveEvent = (delegate* unmanaged<AtkEventListener*, AtkEventType, int, AtkEvent*, AtkEventData*, void>) Marshal.GetFunctionPointerForDelegate(receiveEventDelegate);
    }

    public void Dispose() {
        if (EventListener is null) return;

        NativeMemoryHelper.Free(EventListener->VirtualTable, (ulong) sizeof(void*) * 3);
        NativeMemoryHelper.UiFree(EventListener);

        receiveEventDelegate = null;
    }
    
    [UnmanagedCallersOnly] private static void NullSub() { }
}
