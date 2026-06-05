using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.Internal;

namespace KamiToolKit.BaseTypes;

public unsafe partial class NativeAddon {

    private const int VirtualTableEntryCount = 200;

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
    private AtkUnitBase.Delegates.OnScreenSizeChange onScreenSizeChangedFunction = null!;

    private AtkUnitBase.AtkUnitBaseVirtualTable* modifiedVirtualTable;
    private AtkUnitBase.AtkUnitBaseVirtualTable* originalVirtualTable;

    private void RegisterVirtualTable() {

        originalVirtualTable = InternalAddon->VirtualTable;

        // Overwrite virtual table with a custom copy,
        // Note: currently there are 73 vfuncs, but there's no harm in copying more for when they add new vfuncs to the game
        modifiedVirtualTable = (AtkUnitBase.AtkUnitBaseVirtualTable*)NativeMemoryHelper.Malloc(0x8 * VirtualTableEntryCount);
        NativeMemory.Copy(InternalAddon->VirtualTable, modifiedVirtualTable, 0x8 * VirtualTableEntryCount);
        InternalAddon->VirtualTable = modifiedVirtualTable;

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
        onScreenSizeChangedFunction = ScreenSizeChange;

        modifiedVirtualTable->Initialize = (delegate* unmanaged<AtkUnitBase*, void>)Marshal.GetFunctionPointerForDelegate(initializeFunction);
        modifiedVirtualTable->OnSetup = (delegate* unmanaged<AtkUnitBase*, uint, AtkValue*, void>)Marshal.GetFunctionPointerForDelegate(onSetupFunction);
        modifiedVirtualTable->Show = (delegate* unmanaged<AtkUnitBase*, bool, uint, void>)Marshal.GetFunctionPointerForDelegate(showFunction);
        modifiedVirtualTable->Update = (delegate* unmanaged<AtkUnitBase*, float, void>)Marshal.GetFunctionPointerForDelegate(updateFunction);
        modifiedVirtualTable->Draw = (delegate* unmanaged<AtkUnitBase*, void>)Marshal.GetFunctionPointerForDelegate(drawFunction);
        modifiedVirtualTable->Hide = (delegate* unmanaged<AtkUnitBase*, bool, bool, uint, void>)Marshal.GetFunctionPointerForDelegate(hideFunction);
        modifiedVirtualTable->Hide2 = (delegate* unmanaged<AtkUnitBase*, void>)Marshal.GetFunctionPointerForDelegate(softHideFunction);
        modifiedVirtualTable->Finalizer = (delegate* unmanaged<AtkUnitBase*, void>)Marshal.GetFunctionPointerForDelegate(finalizerFunction);
        modifiedVirtualTable->Dtor = (delegate* unmanaged<AtkUnitBase*, byte, AtkEventListener*>)Marshal.GetFunctionPointerForDelegate(destructorFunction);
        modifiedVirtualTable->OnRequestedUpdate = (delegate* unmanaged<AtkUnitBase*, NumberArrayData**, StringArrayData**, void>)Marshal.GetFunctionPointerForDelegate(onRequestedUpdateFunction);
        modifiedVirtualTable->OnRefresh = (delegate* unmanaged<AtkUnitBase*, uint, AtkValue*, bool>)Marshal.GetFunctionPointerForDelegate(onRefreshFunction);
        modifiedVirtualTable->OnScreenSizeChange = (delegate* unmanaged<AtkUnitBase*, int, int, void>)Marshal.GetFunctionPointerForDelegate(onScreenSizeChangedFunction);
    }
}
