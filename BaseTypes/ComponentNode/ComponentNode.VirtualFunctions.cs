using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.Internal;

namespace KamiToolKit.BaseTypes.ComponentNode;

public abstract unsafe partial class ComponentNode {

    protected virtual void OnReceiveGlobalEvent(AtkComponentBase* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        Services.Log.Verbose($"[{GetType().Name}][{ComponentBase->GetType().Name}] Receive Global Event");

        originalVirtualTable->ReceiveGlobalEvent(thisPtr, eventType, eventParam, atkEvent, atkEventData);
    }

    protected virtual void OnReceiveEvent(AtkComponentBase* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        Services.Log.Verbose($"[{GetType().Name}][{ComponentBase->GetType().Name}] [{eventType}] {eventParam}");

        originalVirtualTable->ReceiveEvent(thisPtr, eventType, eventParam, atkEvent, atkEventData);
    }

    protected virtual void OnInitialize(AtkComponentBase* thisPtr) {
        Services.Log.Verbose($"[{GetType().Name}][{ComponentBase->GetType().Name}] Initialize");

        originalVirtualTable->Initialize(thisPtr);
    }

    protected virtual void OnDeinitialize(AtkComponentBase* thisPtr) {
        Services.Log.Verbose($"[{GetType().Name}][{ComponentBase->GetType().Name}] Deinitialize");

        originalVirtualTable->Deinitialize(thisPtr);
    }

    protected virtual void OnUpdate(AtkComponentBase* thisPtr, float delta) {
        Services.Log.Excessive($"[{GetType().Name}][{ComponentBase->GetType().Name}] Update");

        originalVirtualTable->Update(thisPtr, delta);
    }

    protected virtual void OnDraw(AtkComponentBase* thisPtr) {
        Services.Log.Excessive($"[{GetType().Name}][{ComponentBase->GetType().Name}] Draw");

        originalVirtualTable->Draw(thisPtr);
    }

    protected virtual void OnSetup(AtkComponentBase* thisPtr) {
        Services.Log.Verbose($"[{GetType().Name}][{ComponentBase->GetType().Name}] Setup");

        originalVirtualTable->Setup(thisPtr);
    }

    protected virtual void OnSetEnabledState(AtkComponentBase* thisPtr, bool enabled) {
        Services.Log.Verbose($"[{GetType().Name}][{ComponentBase->GetType().Name}] SetEnabledState");

        originalVirtualTable->SetEnabledState(thisPtr, enabled);
    }

    protected virtual void OnPlaySoundEffect(AtkComponentBase* thisPtr) {
        Services.Log.Verbose($"[{GetType().Name}][{ComponentBase->GetType().Name}] PlaySoundEffect");

        originalVirtualTable->PlaySoundEffect(thisPtr);
    }

    protected virtual AtkResNode* OnGetAtkResNode(AtkComponentBase* thisPtr) {
        Services.Log.Verbose($"[{GetType().Name}][{ComponentBase->GetType().Name}] PlaySoundEffect");

        var result = originalVirtualTable->GetAtkResNode(thisPtr);

        return result;
    }

    protected virtual AtkResNode* OnGetFocusNode(AtkComponentBase* thisPtr) {
        Services.Log.Excessive($"[{GetType().Name}][{ComponentBase->GetType().Name}] GetFocusNode");

        return FocusNode;
    }

    protected virtual void OnInitializeFromComponentData(AtkComponentBase* thisPtr, void* data) {
        Services.Log.Verbose($"[{GetType().Name}][{ComponentBase->GetType().Name}] InitializeFromComponentData");

        originalVirtualTable->InitializeFromComponentData(thisPtr, data);
    }

    private AtkEventListener* Destructor(AtkComponentBase* thisPtr, byte freeFlags) {
        Services.Log.Verbose($"[{GetType().Name}][{ComponentBase->GetType().Name}] Destructor");

        var result = originalVirtualTable->Dtor(thisPtr, freeFlags);

        if ((freeFlags & 1) == 1) {
            // Free our custom virtual table, the game doesn't know this exists and won't clear it on its own.
            NativeMemoryHelper.Free(modifiedVirtualTable, 0x8 * VirtualTableEntryCount);
            modifiedVirtualTable = null;
        }

        return result;
    }
}
