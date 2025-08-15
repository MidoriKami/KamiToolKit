using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Addon;

public abstract unsafe partial class NativeAddon {

    private AtkUnitBase.Delegates.Dtor destructorFunction = null!;
    private AtkUnitBase.Delegates.Draw drawFunction = null!;
    private AtkUnitBase.Delegates.Finalizer finalizerFunction = null!;
    private AtkUnitBase.Delegates.Hide hideFunction = null!;
    private AtkUnitBase.Delegates.Initialize initializeFunction = null!;
    private AtkUnitBase.Delegates.OnSetup onSetupFunction = null!;
    private AtkUnitBase.Delegates.Show showFunction = null!;
    private AtkUnitBase.Delegates.Hide2 softHideFunction = null!;
    private AtkUnitBase.Delegates.Update updateFunction = null!;
    private AtkUnitBase.Delegates.OnRequestedUpdate onRequestedUpdateFunction = null!;
    private AtkUnitBase.Delegates.OnRefresh onRefreshFunction = null!;

    private AtkUnitBase.AtkUnitBaseVirtualTable* virtualTable;

    private void RegisterVirtualTable() {

        // Overwrite virtual table with a custom copy,
        // Note: currently there are 73 vfuncs, but there's no harm in copying more for when they add new vfuncs to the game
        virtualTable = (AtkUnitBase.AtkUnitBaseVirtualTable*)NativeMemoryHelper.Malloc(0x8 * 100);
        NativeMemory.Copy(InternalAddon->VirtualTable, virtualTable, 0x8 * 100);
        InternalAddon->VirtualTable = virtualTable;

        initializeFunction = Initialize;
        onSetupFunction = Setup;
        showFunction = Show;
        updateFunction = Update;
        drawFunction = Draw;
        hideFunction = Hide;
        softHideFunction = Hide2;
        finalizerFunction = Finalizer;
        destructorFunction = Destructor;
        onRequestedUpdateFunction = RequestedUpdate;
        onRefreshFunction = Refresh;

        virtualTable->Initialize = (delegate* unmanaged<AtkUnitBase*, void>)Marshal.GetFunctionPointerForDelegate(initializeFunction);
        virtualTable->OnSetup = (delegate* unmanaged<AtkUnitBase*, uint, AtkValue*, void>)Marshal.GetFunctionPointerForDelegate(onSetupFunction);
        virtualTable->Show = (delegate* unmanaged<AtkUnitBase*, bool, uint, void>)Marshal.GetFunctionPointerForDelegate(showFunction);
        virtualTable->Update = (delegate* unmanaged<AtkUnitBase*, float, void>)Marshal.GetFunctionPointerForDelegate(updateFunction);
        virtualTable->Draw = (delegate* unmanaged<AtkUnitBase*, void>)Marshal.GetFunctionPointerForDelegate(drawFunction);
        virtualTable->Hide = (delegate* unmanaged<AtkUnitBase*, bool, bool, uint, void>)Marshal.GetFunctionPointerForDelegate(hideFunction);
        virtualTable->Hide2 = (delegate* unmanaged<AtkUnitBase*, void>)Marshal.GetFunctionPointerForDelegate(softHideFunction);
        virtualTable->Finalizer = (delegate* unmanaged<AtkUnitBase*, void>)Marshal.GetFunctionPointerForDelegate(finalizerFunction);
        virtualTable->Dtor = (delegate* unmanaged<AtkUnitBase*, byte, AtkEventListener*>)Marshal.GetFunctionPointerForDelegate(destructorFunction);
        virtualTable->OnRequestedUpdate = (delegate* unmanaged<AtkUnitBase*, NumberArrayData**, StringArrayData**, void>)Marshal.GetFunctionPointerForDelegate(onRequestedUpdateFunction);
        virtualTable->OnRefresh = (delegate* unmanaged<AtkUnitBase*, uint, AtkValue*, bool>)Marshal.GetFunctionPointerForDelegate(onRefreshFunction);
    }
}
