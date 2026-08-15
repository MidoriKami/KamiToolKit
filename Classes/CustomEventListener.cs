using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Internal.Classes;

namespace KamiToolKit.Classes;

/// <summary>
/// Managed wrapper around a AtkEventListener.
/// This class is intended to be used to wire native events to managed event handlers.
/// </summary>
/// <remarks>
/// This version is specifically to be used for ATK/UI events.
/// </remarks>
public class CustomEventListener : IDisposable, IAsyncDisposable {

    /// <summary>
    /// Public implicit operator to be able to use this instance as a AtkEventListener* directly.
    /// </summary>
    public static unsafe implicit operator AtkEventListener*(CustomEventListener listener) => listener.eventListener;

    /// <summary>
    /// Constructs a new <see cref="CustomEventListener"/> instance.
    /// </summary>
    public unsafe CustomEventListener(AtkEventListener.Delegates.ReceiveEvent eventHandler) {
        receiveEventDelegate = eventHandler;

        receiveEventWrapper = ReceiveEventWrapper;

        eventListener = IMemorySpace.GetUISpace()->MallocZeroed<AtkEventListener>();
        eventListener->VirtualTable = IMemorySpace.GetUISpace()->AllocateZeroedArray<AtkEventListener.AtkEventListenerVirtualTable>(3);
        eventListener->VirtualTable->Dtor = (delegate* unmanaged<AtkEventListener*, byte, AtkEventListener*>)(delegate* unmanaged<void>)&NullSub;
        eventListener->VirtualTable->ReceiveGlobalEvent = (delegate* unmanaged<AtkEventListener*, AtkEventType, int, AtkEvent*, AtkEventData*, void>)(delegate* unmanaged<void>)&NullSub;
        eventListener->VirtualTable->ReceiveEvent = (delegate* unmanaged<AtkEventListener*, AtkEventType, int, AtkEvent*, AtkEventData*, void>)Marshal.GetFunctionPointerForDelegate(receiveEventWrapper);
    }

    /// <inheritdoc />
    public virtual unsafe void Dispose() {
        if (eventListener is null) return;

        IMemorySpace.Free(eventListener->VirtualTable);
        IMemorySpace.Free(eventListener);

        receiveEventDelegate = null;
        receiveEventWrapper = null;
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
        => await IFramework.Get().Run(Dispose);

    private readonly unsafe AtkEventListener* eventListener;

    private AtkEventListener.Delegates.ReceiveEvent? receiveEventDelegate;
    private AtkEventListener.Delegates.ReceiveEvent? receiveEventWrapper;

    [UnmanagedCallersOnly] private static void NullSub() { }

    private unsafe void ReceiveEventWrapper(AtkEventListener* thisPtr, AtkEventType eventType, int param, AtkEvent* eventObject, AtkEventData* data) {
        try {
            receiveEventDelegate?.Invoke(thisPtr, eventType, param, eventObject, data);
        }
        catch (Exception e) {
            IPluginLog.Get().Exception(e);
        }
    }
}
