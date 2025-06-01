using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Addon;

public abstract unsafe partial class NativeAddon {

	private AtkUnitBase.AtkUnitBaseVirtualTable* virtualTable;

	private AtkUnitBase.Delegates.Dtor destructorFunction = null!;
	private AtkUnitBase.Delegates.Initialize initializeFunction = null!;
	private AtkUnitBase.Delegates.Finalizer finalizerFunction = null!;
	private AtkUnitBase.Delegates.Hide2 softHideFunction = null!;
	private AtkUnitBase.Delegates.OnSetup onSetupFunction = null!;
	private AtkUnitBase.Delegates.Draw drawFunction = null!;
	private AtkUnitBase.Delegates.Update updateFunction = null!;
	private AtkUnitBase.Delegates.Show showFunction = null!;
	private AtkUnitBase.Delegates.Hide hideFunction = null!;

	private void RegisterVirtualTable() {
		initializeFunction = Initialize;
		onSetupFunction = Setup;
		showFunction = Show;
		updateFunction = Update;
		drawFunction = Draw;
		hideFunction = Hide;
		softHideFunction = Hide2;
		finalizerFunction = Finalizer;
		destructorFunction = Destructor;
		
		virtualTable->Initialize = (delegate* unmanaged<AtkUnitBase*, void>) Marshal.GetFunctionPointerForDelegate(initializeFunction);
		virtualTable->OnSetup = (delegate* unmanaged<AtkUnitBase*, uint, AtkValue*, void>) Marshal.GetFunctionPointerForDelegate(onSetupFunction);
		virtualTable->Show = (delegate* unmanaged<AtkUnitBase*, bool, uint, void>) Marshal.GetFunctionPointerForDelegate(showFunction);
		virtualTable->Update = (delegate* unmanaged<AtkUnitBase*, float, void>) Marshal.GetFunctionPointerForDelegate(updateFunction);
		virtualTable->Draw = (delegate* unmanaged<AtkUnitBase*, void>) Marshal.GetFunctionPointerForDelegate(drawFunction);
		virtualTable->Hide = (delegate* unmanaged<AtkUnitBase*, bool, bool, uint, void>) Marshal.GetFunctionPointerForDelegate(hideFunction);
		virtualTable->Hide2 = (delegate* unmanaged<AtkUnitBase*, void>) Marshal.GetFunctionPointerForDelegate(softHideFunction);
		virtualTable->Finalizer = (delegate* unmanaged<AtkUnitBase*, void>) Marshal.GetFunctionPointerForDelegate(finalizerFunction);
		virtualTable->Dtor = (delegate* unmanaged<AtkUnitBase*, byte, AtkEventListener*>) Marshal.GetFunctionPointerForDelegate(destructorFunction);
	}
}