using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Internal.Classes;

namespace KamiToolKit.BaseTypes.ComponentNode;

public abstract unsafe partial class ComponentNode {

    protected virtual void OnReceiveGlobalEvent(AtkComponentBase* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        Services.Log.Verbose($"[{GetType().Name}][{GetComponentType()}] Receive Global Event");

        originalVirtualTable->ReceiveGlobalEvent(thisPtr, eventType, eventParam, atkEvent, atkEventData);
    }

    protected virtual void OnReceiveEvent(AtkComponentBase* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        Services.Log.Verbose($"[{GetType().Name}][{GetComponentType()}] [{eventType}] {eventParam}");

        originalVirtualTable->ReceiveEvent(thisPtr, eventType, eventParam, atkEvent, atkEventData);
    }

    protected virtual void OnInitialize(AtkComponentBase* thisPtr) {
        Services.Log.Verbose($"[{GetType().Name}][{GetComponentType()}] Initialize");

        originalVirtualTable->Initialize(thisPtr);
    }

    protected virtual void OnDeinitialize(AtkComponentBase* thisPtr) {
        Services.Log.Verbose($"[{GetType().Name}][{GetComponentType()}] Deinitialize");

        originalVirtualTable->Deinitialize(thisPtr);
    }

    protected virtual void OnUpdate(AtkComponentBase* thisPtr, float delta) {
        Services.Log.Excessive($"[{GetType().Name}][{GetComponentType()}] Update");

        originalVirtualTable->Update(thisPtr, delta);
    }

    protected virtual void OnDraw(AtkComponentBase* thisPtr) {
        Services.Log.Excessive($"[{GetType().Name}][{GetComponentType()}] Draw");

        originalVirtualTable->Draw(thisPtr);
    }

    protected virtual void OnSetup(AtkComponentBase* thisPtr) {
        Services.Log.Verbose($"[{GetType().Name}][{GetComponentType()}] Setup");

        originalVirtualTable->Setup(thisPtr);
    }

    protected virtual void OnSetEnabledState(AtkComponentBase* thisPtr, bool enabled) {
        Services.Log.Verbose($"[{GetType().Name}][{GetComponentType()}] SetEnabledState");

        originalVirtualTable->SetEnabledState(thisPtr, enabled);
    }

    protected virtual void OnPlaySoundEffect(AtkComponentBase* thisPtr) {
        Services.Log.Verbose($"[{GetType().Name}][{GetComponentType()}] PlaySoundEffect");

        originalVirtualTable->PlaySoundEffect(thisPtr);
    }

    protected virtual AtkResNode* OnGetAtkResNode(AtkComponentBase* thisPtr) {
        Services.Log.Verbose($"[{GetType().Name}][{GetComponentType()}] PlaySoundEffect");

        var result = originalVirtualTable->GetAtkResNode(thisPtr);

        return result;
    }

    protected virtual AtkResNode* OnGetFocusNode(AtkComponentBase* thisPtr) {
        Services.Log.Excessive($"[{GetType().Name}][{GetComponentType()}] GetFocusNode");

        return FocusNode;
    }

    protected virtual void OnInitializeFromComponentData(AtkComponentBase* thisPtr, void* data) {
        Services.Log.Verbose($"[{GetType().Name}][{GetComponentType()}] InitializeFromComponentData");

        originalVirtualTable->InitializeFromComponentData(thisPtr, data);
    }

    private AtkEventListener* Destructor(AtkComponentBase* thisPtr, byte freeFlags) {
        Services.Log.Verbose($"[{GetType().Name}][{GetComponentType()}] Destructor");

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
