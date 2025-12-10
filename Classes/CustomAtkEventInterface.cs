using System;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using static FFXIVClientStructs.FFXIV.Component.GUI.AtkModuleInterface;

namespace KamiToolKit.Classes;

public sealed unsafe class CustomAtkEventInterface : IDisposable {

    public readonly AtkEventInterface* EventInterface;

    private AtkEventInterface.Delegates.ReceiveEvent? receiveEventDelegate;

    public CustomAtkEventInterface(AtkEventInterface.Delegates.ReceiveEvent eventHandler) {
        receiveEventDelegate = eventHandler;

        EventInterface = NativeMemoryHelper.UiAlloc<AtkEventInterface>();
        EventInterface->VirtualTable = (AtkEventInterface.AtkEventInterfaceVirtualTable*)NativeMemoryHelper.Malloc((ulong)sizeof(void*) * 2);
        EventInterface->VirtualTable->ReceiveEvent = (delegate* unmanaged<AtkEventInterface*, AtkValue*, AtkValue*, uint, ulong, AtkValue*>)Marshal.GetFunctionPointerForDelegate(receiveEventDelegate);
        EventInterface->VirtualTable->ReceiveEvent2 = (delegate* unmanaged<AtkEventInterface*, AtkValue*, AtkValue*, uint, ulong, AtkValue*>)(delegate* unmanaged<void>)&NullSub;
    }

    public void Dispose() {
        if (EventInterface is null) return;

        NativeMemoryHelper.Free(EventInterface->VirtualTable, (ulong)sizeof(void*) * 2);
        NativeMemoryHelper.UiFree(EventInterface);

        receiveEventDelegate = null;
    }

    [UnmanagedCallersOnly] private static void NullSub(){}

    public static implicit operator AtkEventInterface*(CustomAtkEventInterface listener) => listener.EventInterface;
}
