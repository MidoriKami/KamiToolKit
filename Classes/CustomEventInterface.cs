using System;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Internal.Classes;
using static FFXIVClientStructs.FFXIV.Component.GUI.AtkModuleInterface;

namespace KamiToolKit.Classes;

/// <summary>
/// Managed wrapper around a AtkEventInterface.
/// This class is intended to be used to wire native events to managed event handlers.
/// </summary>
/// <remarks>
/// This version specifically to be used for ContextMenus, as those use AtkEventInterface to trigger their callbacks.
/// </remarks>
public unsafe class CustomEventInterface : IDisposable {

    /// <summary>
    /// Public implicit operator to be able to use this instance as a AtkEventInterface* directly.
    /// </summary>
    public static implicit operator AtkEventInterface*(CustomEventInterface listener) => listener.eventInterface;

    public CustomEventInterface(AtkEventInterface.Delegates.ReceiveEvent eventHandler, AtkEventInterface.Delegates.ReceiveEventWithResult? receiveEventWithResult = null) {
        receiveEventDelegate = eventHandler;
        receiveEventWithResultDelegate = receiveEventWithResult;

        receiveWrapper = ReceiveEventWrapper;
        receiveWithResultWrapper = ReceiveWithResultWrapper;

        eventInterface = NativeMemoryHelper.UiAlloc<AtkEventInterface>();
        eventInterface->VirtualTable = (AtkEventInterface.AtkEventInterfaceVirtualTable*)NativeMemoryHelper.Malloc((ulong)sizeof(void*) * 2);
        eventInterface->VirtualTable->ReceiveEvent = (delegate* unmanaged<AtkEventInterface*, AtkValue*, AtkValue*, uint, ulong, AtkValue*>)Marshal.GetFunctionPointerForDelegate(receiveWrapper);
        eventInterface->VirtualTable->ReceiveEventWithResult = (delegate* unmanaged<AtkEventInterface*, AtkValue*, AtkValue*, uint, ulong, AtkValue*>)Marshal.GetFunctionPointerForDelegate(receiveWithResultWrapper);
    }

    public void Dispose() {
        if (eventInterface is null) return;

        NativeMemoryHelper.Free(eventInterface->VirtualTable, (ulong)sizeof(void*) * 2);
        NativeMemoryHelper.UiFree(eventInterface);

        receiveEventDelegate = null;
        receiveEventWithResultDelegate = null;

        receiveWrapper = null;
        receiveWithResultWrapper = null;
    }

    private readonly AtkEventInterface* eventInterface;

    private AtkEventInterface.Delegates.ReceiveEvent? receiveEventDelegate;
    private AtkEventInterface.Delegates.ReceiveEventWithResult? receiveEventWithResultDelegate;

    private AtkEventInterface.Delegates.ReceiveEvent? receiveWrapper;
    private AtkEventInterface.Delegates.ReceiveEventWithResult? receiveWithResultWrapper;

    [UnmanagedCallersOnly] private static void NullSub() { }

    private AtkValue* ReceiveEventWrapper(AtkEventInterface* thisPtr, AtkValue* returnValue, AtkValue* values, uint valueCount, ulong eventKind) {
        try {
            if (receiveEventDelegate is not null) {
                return receiveEventDelegate.Invoke(thisPtr, returnValue, values, valueCount, eventKind);
            }
        }
        catch (Exception e) {
            Services.Log.Exception(e);
        }

        return returnValue;
    }

    private AtkValue* ReceiveWithResultWrapper(AtkEventInterface* thisPtr, AtkValue* returnValue, AtkValue* values, uint valueCount, ulong eventKind) {
        try {
            if (receiveEventWithResultDelegate is not null) {
                return receiveEventWithResultDelegate.Invoke(thisPtr, returnValue, values, valueCount, eventKind);
            }
        }
        catch (Exception e) {
            Services.Log.Exception(e);
        }

        return returnValue;
    }
}
