using System;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using static FFXIVClientStructs.FFXIV.Component.GUI.AtkModuleInterface;

namespace KamiToolKit.Classes;

public unsafe class CustomEventInterface : IDisposable {

    private readonly AtkEventInterface* eventInterface;

    private AtkEventInterface.Delegates.ReceiveEvent? receiveEventDelegate;
    private AtkEventInterface.Delegates.ReceiveEvent2? receiveEventDelegate2;

    public CustomEventInterface(AtkEventInterface.Delegates.ReceiveEvent eventHandler, AtkEventInterface.Delegates.ReceiveEvent2? eventHandler2 = null) {
        receiveEventDelegate = eventHandler;
        receiveEventDelegate2 = eventHandler2;

        eventInterface = NativeMemoryHelper.UiAlloc<AtkEventInterface>();
        eventInterface->VirtualTable = (AtkEventInterface.AtkEventInterfaceVirtualTable*)NativeMemoryHelper.Malloc((ulong)sizeof(void*) * 2);
        eventInterface->VirtualTable->ReceiveEvent = (delegate* unmanaged<AtkEventInterface*, AtkValue*, AtkValue*, uint, ulong, AtkValue*>)Marshal.GetFunctionPointerForDelegate(receiveEventDelegate);

        if (receiveEventDelegate2 is not null) {
            eventInterface->VirtualTable->ReceiveEvent2 = (delegate* unmanaged<AtkEventInterface*, AtkValue*, AtkValue*, uint, ulong, AtkValue*>)Marshal.GetFunctionPointerForDelegate(receiveEventDelegate2);
        }
        else {
            eventInterface->VirtualTable->ReceiveEvent2 = (delegate* unmanaged<AtkEventInterface*, AtkValue*, AtkValue*, uint, ulong, AtkValue*>)(delegate* unmanaged<void>)&NullSub;
        }
    }

    public void Dispose() {
        if (eventInterface is null) return;

        NativeMemoryHelper.Free(eventInterface->VirtualTable, (ulong)sizeof(void*) * 2);
        NativeMemoryHelper.UiFree(eventInterface);

        receiveEventDelegate = null;
        receiveEventDelegate2 = null;
    }

    [UnmanagedCallersOnly] private static void NullSub() { }

    public static implicit operator AtkEventInterface*(CustomEventInterface listener) => listener.eventInterface;
}
