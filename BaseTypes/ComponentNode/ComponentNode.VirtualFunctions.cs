using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.Internal;

namespace KamiToolKit.BaseTypes.ComponentNode;

public abstract unsafe partial class ComponentNode {
    protected virtual void OnReceiveGlobalEvent(AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) { }
    protected virtual void OnReceiveEvent(AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) { }
    protected virtual void OnUpdate() { }
    protected virtual void OnDraw() { }
    protected virtual void OnSetup() { }

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

    private void ReceiveGlobalEvent(AtkComponentBase* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        Services.Log.Verbose($"[{GetType().Name}][{ComponentBase->GetType().Name}] Receive Global Event");

        OnReceiveGlobalEvent(eventType, eventParam, atkEvent, atkEventData);

        originalVirtualTable->ReceiveGlobalEvent(thisPtr, eventType, eventParam, atkEvent, atkEventData);
    }

    private void ReceiveEvent(AtkComponentBase* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        Services.Log.Verbose($"[{GetType().Name}][{ComponentBase->GetType().Name}] [{eventType}] {eventParam}");

        OnReceiveEvent(eventType, eventParam, atkEvent, atkEventData);

        originalVirtualTable->ReceiveEvent(thisPtr, eventType, eventParam, atkEvent, atkEventData);
    }

    private void Initialize(AtkComponentBase* thisPtr) {
        Services.Log.Verbose($"[{GetType().Name}][{ComponentBase->GetType().Name}] Initialize");

        originalVirtualTable->Initialize(thisPtr);
    }

    private void Deinitialize(AtkComponentBase* thisPtr) {
        Services.Log.Verbose($"[{GetType().Name}][{ComponentBase->GetType().Name}] Deinitialize");

        originalVirtualTable->Deinitialize(thisPtr);
    }

    private void Update(AtkComponentBase* thisPtr, float delta) {
        Services.Log.Excessive($"[{GetType().Name}][{ComponentBase->GetType().Name}] Update");

        OnUpdate();

        originalVirtualTable->Update(thisPtr, delta);
    }

    private void Draw(AtkComponentBase* thisPtr) {
        Services.Log.Excessive($"[{GetType().Name}][{ComponentBase->GetType().Name}] Draw");

        OnDraw();

        originalVirtualTable->Draw(thisPtr);
    }

    private void Setup(AtkComponentBase* thisPtr) {
        Services.Log.Verbose($"[{GetType().Name}][{ComponentBase->GetType().Name}] Setup");

        OnSetup();

        originalVirtualTable->Setup(thisPtr);
    }

    private void SetEnabledState(AtkComponentBase* thisPtr, bool enabled) {
        Services.Log.Verbose($"[{GetType().Name}][{ComponentBase->GetType().Name}] SetEnabledState");

        originalVirtualTable->SetEnabledState(thisPtr, enabled);
    }

    private void PlaySoundEffect(AtkComponentBase* thisPtr) {
        Services.Log.Verbose($"[{GetType().Name}][{ComponentBase->GetType().Name}] PlaySoundEffect");

        originalVirtualTable->PlaySoundEffect(thisPtr);
    }

    private AtkResNode* GetAtkResNode(AtkComponentBase* thisPtr) {
        Services.Log.Verbose($"[{GetType().Name}][{ComponentBase->GetType().Name}] PlaySoundEffect");

        var result = originalVirtualTable->GetAtkResNode(thisPtr);

        return result;
    }

    private AtkResNode* GetFocusNode(AtkComponentBase* thisPtr) {
        Services.Log.Excessive($"[{GetType().Name}][{ComponentBase->GetType().Name}] GetFocusNode");

        return FocusNode;
    }

    private void InitializeFromComponentData(AtkComponentBase* thisPtr, void* data) {
        Services.Log.Verbose($"[{GetType().Name}][{ComponentBase->GetType().Name}] InitializeFromComponentData");

        originalVirtualTable->InitializeFromComponentData(thisPtr, data);
    }
}
