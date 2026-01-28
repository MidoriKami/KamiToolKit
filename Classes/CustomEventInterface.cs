using System;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using static FFXIVClientStructs.FFXIV.Component.GUI.AtkModuleInterface;

namespace KamiToolKit.Classes;

public unsafe class CustomEventInterface : IDisposable {

    private readonly AtkEventInterface* eventInterface;

    private AtkEventInterface.Delegates.ReceiveEvent? receiveEventDelegate;
    private AtkEventInterface.Delegates.ReceiveEventWithResult? receiveEventWithResultDelegate;

    public CustomEventInterface(AtkEventInterface.Delegates.ReceiveEvent eventHandler, AtkEventInterface.Delegates.ReceiveEventWithResult? receiveEventWithResult = null) {
        receiveEventDelegate = eventHandler;
        receiveEventWithResultDelegate = receiveEventWithResult;

        eventInterface = NativeMemoryHelper.UiAlloc<AtkEventInterface>();
        eventInterface->VirtualTable = (AtkEventInterface.AtkEventInterfaceVirtualTable*)NativeMemoryHelper.Malloc((ulong)sizeof(void*) * 2);
        eventInterface->VirtualTable->ReceiveEvent = (delegate* unmanaged<AtkEventInterface*, AtkValue*, AtkValue*, uint, ulong, AtkValue*>)Marshal.GetFunctionPointerForDelegate(receiveEventDelegate);

        if (receiveEventWithResultDelegate is not null) {
            eventInterface->VirtualTable->ReceiveEventWithResult = (delegate* unmanaged<AtkEventInterface*, AtkValue*, AtkValue*, uint, ulong, AtkValue*>)Marshal.GetFunctionPointerForDelegate(receiveEventWithResultDelegate);
        }
        else {
            eventInterface->VirtualTable->ReceiveEventWithResult = (delegate* unmanaged<AtkEventInterface*, AtkValue*, AtkValue*, uint, ulong, AtkValue*>)(delegate* unmanaged<void>)&NullSub;
        }
    }

    public void Dispose() {
        if (eventInterface is null) return;

        NativeMemoryHelper.Free(eventInterface->VirtualTable, (ulong)sizeof(void*) * 2);
        NativeMemoryHelper.UiFree(eventInterface);

        receiveEventDelegate = null;
        receiveEventWithResultDelegate = null;
    }

    [UnmanagedCallersOnly] private static void NullSub() { }

    public static implicit operator AtkEventInterface*(CustomEventInterface listener) => listener.eventInterface;
}
