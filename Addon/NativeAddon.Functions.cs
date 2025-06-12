using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Addon;

public abstract unsafe partial class NativeAddon {
	
	protected virtual void OnSetup(AtkUnitBase* addon) { }
	protected virtual void OnShow(AtkUnitBase* addon) { }
	protected virtual void OnDraw(AtkUnitBase* addon) { }
	protected virtual void OnUpdate(AtkUnitBase* addon) { }
	protected virtual void OnHide(AtkUnitBase* addon) { }
	protected virtual void OnFinalize(AtkUnitBase* addon) { }

	private void Initialize(AtkUnitBase* thisPtr) {
		Log.Verbose($"[{InternalName}] Initialize");
		
		AtkUnitBase.StaticVirtualTablePointer->Initialize(thisPtr);

		thisPtr->UldManager.InitializeResourceRendererManager();
		
		InitializeAddon();
	}

	private void Setup(AtkUnitBase* addon, uint valueCount, AtkValue* values) {
		Log.Verbose($"[{InternalName}] Setup");
		
		SetInitialState();

		AtkUnitBase.StaticVirtualTablePointer->OnSetup(addon, valueCount, values);

		OnSetup(addon);
	}

	private void Show(AtkUnitBase* addon, bool silenceOpenSoundEffect, uint unsetShowHideFlags) {
		Log.Verbose($"[{InternalName}] Show");
		
		OnShow(addon);
		
		AtkUnitBase.StaticVirtualTablePointer->Show(addon, silenceOpenSoundEffect, unsetShowHideFlags);
	}

	private void Update(AtkUnitBase* addon, float delta) {
		Log.Excessive($"[{InternalName}] Update");
		
		OnUpdate(addon);
		
		AtkUnitBase.StaticVirtualTablePointer->Update(addon, delta);
	}

	private void Draw(AtkUnitBase* addon) {
		Log.Excessive($"[{InternalName}] Draw");
		
		OnDraw(addon);
		
		AtkUnitBase.StaticVirtualTablePointer->Draw(addon);
	}

	private void Hide(AtkUnitBase* addon, bool unkBool, bool callHideCallback, uint setShowHideFlags) {
		Log.Verbose($"[{InternalName}] Hide");
		
		OnHide(addon);
		
		AtkUnitBase.StaticVirtualTablePointer->Hide(addon, unkBool, callHideCallback, setShowHideFlags);
	}

	private void Hide2(AtkUnitBase* addon) {
		Log.Verbose($"[{InternalName}] Hide2");
		
		AtkUnitBase.StaticVirtualTablePointer->Close(addon, false);
	}
	
	private void Finalizer(AtkUnitBase* addon) {
		Log.Verbose($"[{InternalName}] Finalize");
		
		OnFinalize(addon);

		AtkUnitBase.StaticVirtualTablePointer->Finalizer(InternalAddon);
	}
	
	private AtkEventListener* Destructor(AtkUnitBase* addon, byte flags) {
		Log.Verbose($"[{InternalName}] Destructor");
		
		var result = AtkUnitBase.StaticVirtualTablePointer->Dtor(addon, flags);

		if ((flags & 1) == 1) {
			InternalAddon = null;
			disposeHandle?.Free();
			disposeHandle = null;
			CreatedAddons.Remove(this);
			
			// Free our custom virtual table, the game doesn't know this exists and won't clear it on its own.
			NativeMemoryHelper.Free(virtualTable, 0x8 * 100);
		}

		return result;
	}
}