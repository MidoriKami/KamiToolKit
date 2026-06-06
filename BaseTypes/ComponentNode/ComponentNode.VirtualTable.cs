using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.Internal;

namespace KamiToolKit.BaseTypes.ComponentNode;

public abstract unsafe partial class ComponentNode {

    private const int VirtualTableEntryCount = 100;

    private AtkComponentBase.Delegates.Dtor destructorFunction = null!;
    private AtkComponentBase.Delegates.ReceiveGlobalEvent receiveGlobalEventFunction = null!;
    private AtkComponentBase.Delegates.ReceiveEvent receiveEventFunction = null!;
    private AtkComponentBase.Delegates.Initialize initializeFunction = null!;
    private AtkComponentBase.Delegates.Deinitialize deinitializeFunction = null!;
    private AtkComponentBase.Delegates.Update updateFunction = null!;
    private AtkComponentBase.Delegates.Draw drawFunction = null!;
    private AtkComponentBase.Delegates.Setup setupFunction = null!;
    private AtkComponentBase.Delegates.SetEnabledState setEnabledStateFunction = null!;
    private AtkComponentBase.Delegates.PlaySoundEffect playSoundEffectFunction = null!;
    private AtkComponentBase.Delegates.GetAtkResNode getAtkResNodeFunction = null!;
    private AtkComponentBase.Delegates.GetFocusNode getFocusNodeFunction = null!;
    private AtkComponentBase.Delegates.InitializeFromComponentData initializeFromComponentData = null!;

    private AtkComponentBase.AtkComponentBaseVirtualTable* modifiedVirtualTable;
    private AtkComponentBase.AtkComponentBaseVirtualTable* originalVirtualTable;

    protected void RegisterVirtualTable() {
        originalVirtualTable = ComponentBase->VirtualTable;

        modifiedVirtualTable = (AtkComponentBase.AtkComponentBaseVirtualTable*) NativeMemoryHelper.Malloc(0x8 * VirtualTableEntryCount);
        NativeMemory.Copy(ComponentBase->VirtualTable, modifiedVirtualTable, 0x8 * VirtualTableEntryCount);
        ComponentBase->VirtualTable = modifiedVirtualTable;

        destructorFunction = Destructor;
        receiveGlobalEventFunction = OnReceiveGlobalEvent;
        receiveEventFunction = OnReceiveEvent;
        initializeFunction = OnInitialize;
        deinitializeFunction = OnDeinitialize;
        updateFunction = OnUpdate;
        drawFunction = OnDraw;
        setupFunction = OnSetup;
        setEnabledStateFunction = OnSetEnabledState;
        playSoundEffectFunction = OnPlaySoundEffect;
        getAtkResNodeFunction = OnGetAtkResNode;
        getFocusNodeFunction = OnGetFocusNode;
        initializeFromComponentData = OnInitializeFromComponentData;

        modifiedVirtualTable->Dtor = (delegate* unmanaged<AtkComponentBase*, byte, AtkEventListener*>) Marshal.GetFunctionPointerForDelegate(destructorFunction);
        modifiedVirtualTable->ReceiveGlobalEvent = (delegate* unmanaged<AtkComponentBase*, AtkEventType, int, AtkEvent*, AtkEventData*, void>) Marshal.GetFunctionPointerForDelegate(receiveGlobalEventFunction);
        modifiedVirtualTable->ReceiveEvent = (delegate* unmanaged<AtkComponentBase*, AtkEventType, int, AtkEvent*, AtkEventData*, void>) Marshal.GetFunctionPointerForDelegate(receiveEventFunction);
        modifiedVirtualTable->Initialize = (delegate* unmanaged<AtkComponentBase*, void>) Marshal.GetFunctionPointerForDelegate(initializeFunction);
        modifiedVirtualTable->Deinitialize = (delegate* unmanaged<AtkComponentBase*, void>) Marshal.GetFunctionPointerForDelegate(deinitializeFunction);
        modifiedVirtualTable->Update = (delegate* unmanaged<AtkComponentBase*, float, void>) Marshal.GetFunctionPointerForDelegate(updateFunction);
        modifiedVirtualTable->Draw = (delegate* unmanaged<AtkComponentBase*, void>) Marshal.GetFunctionPointerForDelegate(drawFunction);
        modifiedVirtualTable->Setup = (delegate* unmanaged<AtkComponentBase*, void>) Marshal.GetFunctionPointerForDelegate(setupFunction);
        modifiedVirtualTable->SetEnabledState = (delegate* unmanaged<AtkComponentBase*, bool, void>) Marshal.GetFunctionPointerForDelegate(setEnabledStateFunction);
        modifiedVirtualTable->PlaySoundEffect = (delegate* unmanaged<AtkComponentBase*, void>) Marshal.GetFunctionPointerForDelegate(playSoundEffectFunction);
        modifiedVirtualTable->GetAtkResNode = (delegate* unmanaged<AtkComponentBase*, AtkResNode*>) Marshal.GetFunctionPointerForDelegate(getAtkResNodeFunction);
        modifiedVirtualTable->GetFocusNode = (delegate* unmanaged<AtkComponentBase*, AtkResNode*>) Marshal.GetFunctionPointerForDelegate(getFocusNodeFunction);
        modifiedVirtualTable->InitializeFromComponentData = (delegate* unmanaged<AtkComponentBase*, void*, void>) Marshal.GetFunctionPointerForDelegate(initializeFromComponentData);
    }
}
