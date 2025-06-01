using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Addon;

public abstract unsafe partial class NativeAddon {
	
	protected virtual void OnSetup(AtkUnitBase* addon) { }
	protected virtual void OnShow(AtkUnitBase* addon) { }
	protected virtual void OnHide(AtkUnitBase* addon) { }
	protected virtual void OnDraw(AtkUnitBase* addon) { }
	protected virtual void OnUpdate(AtkUnitBase* addon) { }
	protected virtual void OnFinalize(AtkUnitBase* addon) { }

	private void Initialize(AtkUnitBase* thisPtr) {
		Log.Verbose($"[KamiToolKit] [{InternalName}] Initialize");
		
		AtkUnitBase.StaticVirtualTablePointer->Initialize(thisPtr);

		thisPtr->UldManager.InitializeResourceRendererManager();
		
		InitializeAddon();
	}

	private void Setup(AtkUnitBase* addon, uint valueCount, AtkValue* values) {
		Log.Verbose($"[KamiToolKit] [{InternalName}] Setup");
		
		SetInitialState();

		AtkUnitBase.StaticVirtualTablePointer->OnSetup(addon, valueCount, values);

		OnSetup(addon);
	}

	private void Show(AtkUnitBase* thisPtr, bool silenceOpenSoundEffect, uint unsetShowHideFlags) {
		Log.Verbose($"[KamiToolKit] [{InternalName}] Show");
		
		OnShow(thisPtr);
		
		AtkUnitBase.StaticVirtualTablePointer->Show(thisPtr, silenceOpenSoundEffect, unsetShowHideFlags);
	}

	private void Update(AtkUnitBase* addon, float delta) {
		Log.Excessive($"[KamiToolKit] [{InternalName}] Update");
		
		OnUpdate(addon);
		
		AtkUnitBase.StaticVirtualTablePointer->Update(addon, delta);
	}

	private void Draw(AtkUnitBase* addon) {
		Log.Excessive($"[KamiToolKit] [{InternalName}] Draw");
		
		OnDraw(addon);
		
		AtkUnitBase.StaticVirtualTablePointer->Draw(addon);
	}

	private void Hide(AtkUnitBase* thisPtr, bool unkBool, bool callHideCallback, uint setShowHideFlags) {
		Log.Verbose($"[KamiToolKit] [{InternalName}] Hide");
		
		OnHide(thisPtr);
		
		AtkUnitBase.StaticVirtualTablePointer->Hide(thisPtr, unkBool, callHideCallback, setShowHideFlags);
	}

	private void Hide2(AtkUnitBase* addon) {
		Log.Verbose($"[KamiToolKit] [{InternalName}] SoftHide (Hide2)");
		
		AtkUnitBase.StaticVirtualTablePointer->Close(addon, false);
	}
	
	private void Finalizer(AtkUnitBase* addon) {
		Log.Verbose($"[KamiToolKit] [{InternalName}] Finalize");
		
		OnFinalize(addon);

		AtkUnitBase.StaticVirtualTablePointer->Finalizer(InternalAddon);
	}
	
	private AtkEventListener* Destructor(AtkUnitBase* addon, byte flags) {
		Log.Verbose($"[KamiToolKit] [{InternalName}] Destructor");
		
		var result = AtkUnitBase.StaticVirtualTablePointer->Dtor(addon, flags);

		if ((flags & 1) == 1) {
			InternalAddon = null;
			disposeHandle?.Free();
			disposeHandle = null;
		}

		return result;
	}
}