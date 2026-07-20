using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Internal.Classes;

namespace KamiToolKit.BaseTypes.ComponentNode;

public abstract unsafe partial class ComponentNode {

    /// <summary>
    /// Global event callback for events that the game wired up to this component.
    /// </summary>
    protected virtual void OnReceiveGlobalEvent(AtkComponentBase* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        IPluginLog.Get().Verbose($"[{GetType().Name}][{GetComponentType()}] Receive Global Event");

        originalVirtualTable->ReceiveGlobalEvent(thisPtr, eventType, eventParam, atkEvent, atkEventData);
    }

    /// <summary>
    /// Event callback for events that the game wired up to this component.
    /// </summary>
    protected virtual void OnReceiveEvent(AtkComponentBase* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        IPluginLog.Get().Verbose($"[{GetType().Name}][{GetComponentType()}] [{eventType}] {eventParam}");

        originalVirtualTable->ReceiveEvent(thisPtr, eventType, eventParam, atkEvent, atkEventData);
    }

    /// <summary>
    /// Initialize callback for this component.
    /// </summary>
    protected virtual void OnInitialize(AtkComponentBase* thisPtr) {
        IPluginLog.Get().Verbose($"[{GetType().Name}][{GetComponentType()}] Initialize");

        originalVirtualTable->Initialize(thisPtr);
    }

    /// <summary>
    /// Unloading callback for this component.
    /// </summary>
    protected virtual void OnDeinitialize(AtkComponentBase* thisPtr) {
        IPluginLog.Get().Verbose($"[{GetType().Name}][{GetComponentType()}] Deinitialize");

        originalVirtualTable->Deinitialize(thisPtr);
    }

    /// <summary>
    /// Per-frame update callback for this component.
    /// </summary>
    protected virtual void OnUpdate(AtkComponentBase* thisPtr, float delta) {
        IPluginLog.Get().Excessive($"[{GetType().Name}][{GetComponentType()}] Update");

        originalVirtualTable->Update(thisPtr, delta);
    }

    /// <summary>
    /// Draw callback for this component.
    /// </summary>
    protected virtual void OnDraw(AtkComponentBase* thisPtr) {
        IPluginLog.Get().Excessive($"[{GetType().Name}][{GetComponentType()}] Draw");

        originalVirtualTable->Draw(thisPtr);
    }

    /// <summary>
    /// Setup callback for this component.
    /// </summary>
    protected virtual void OnSetup(AtkComponentBase* thisPtr) {
        IPluginLog.Get().Verbose($"[{GetType().Name}][{GetComponentType()}] Setup");

        originalVirtualTable->Setup(thisPtr);
    }

    /// <summary>
    /// Enable state changed callback for this component.
    /// </summary>
    protected virtual void OnSetEnabledState(AtkComponentBase* thisPtr, bool enabled) {
        IPluginLog.Get().Verbose($"[{GetType().Name}][{GetComponentType()}] SetEnabledState");

        originalVirtualTable->SetEnabledState(thisPtr, enabled);
    }

    /// <summary>
    /// Play sound effect callback for this component.
    /// </summary>
    protected virtual void OnPlaySoundEffect(AtkComponentBase* thisPtr) {
        IPluginLog.Get().Verbose($"[{GetType().Name}][{GetComponentType()}] PlaySoundEffect");

        originalVirtualTable->PlaySoundEffect(thisPtr);
    }

    /// <summary>
    /// GetAtkResNode callback for this component.
    /// </summary>
    protected virtual AtkResNode* OnGetAtkResNode(AtkComponentBase* thisPtr) {
        IPluginLog.Get().Verbose($"[{GetType().Name}][{GetComponentType()}] PlaySoundEffect");

        var result = originalVirtualTable->GetAtkResNode(thisPtr);

        return result;
    }

    /// <summary>
    /// GetFocusNode callback for this component.
    /// </summary>
    /// <remarks>
    /// Overriden to return <see cref="FocusNode"/>.
    /// </remarks>
    protected virtual AtkResNode* OnGetFocusNode(AtkComponentBase* thisPtr) {
        IPluginLog.Get().Excessive($"[{GetType().Name}][{GetComponentType()}] GetFocusNode");

        return FocusNode;
    }

    /// <summary>
    /// Initialization from data callback for this component.
    /// </summary>
    /// <param name="thisPtr"></param>
    /// <param name="data"></param>
    protected virtual void OnInitializeFromComponentData(AtkComponentBase* thisPtr, void* data) {
        IPluginLog.Get().Verbose($"[{GetType().Name}][{GetComponentType()}] InitializeFromComponentData");

        originalVirtualTable->InitializeFromComponentData(thisPtr, data);
    }

    private AtkEventListener* Destructor(AtkComponentBase* thisPtr, byte freeFlags) {
        IPluginLog.Get().Verbose($"[{GetType().Name}][{GetComponentType()}] Destructor");

        var result = originalVirtualTable->Dtor(thisPtr, freeFlags);

        if ((freeFlags & 1) == 1) {
            // Free our custom virtual table, the game doesn't know this exists and won't clear it on its own.
            NativeMemoryHelper.Free(modifiedVirtualTable, 0x8 * VirtualTableEntryCount);
            modifiedVirtualTable = null;
        }

        return result;
    }

    private string GetComponentType()
        => ComponentBase->GetComponentType().ToString();
}
