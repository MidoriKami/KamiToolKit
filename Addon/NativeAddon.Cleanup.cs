using System;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Addon;

public unsafe partial class NativeAddon : IDisposable {
	~NativeAddon() => Dispose(false);

	protected virtual void Dispose(bool disposing) {
		if (disposing) {
			Dispose();
		}
	}

	public void Dispose() {
		if (!isDisposed) {
			Log.Debug($"[KamiToolKit] Disposing Addon {GetType()}");
			
			windowNode.DetachNode();
			windowNode.Dispose();
			
			rootNode.DetachNode();
			rootNode.Dispose();

			AtkUnitBase.StaticVirtualTablePointer->Close(InternalAddon, false);
			AtkUnitBase.StaticVirtualTablePointer->Finalizer(InternalAddon);
			
			NativeMemoryHelper.UiFree(InternalAddon);
			GC.SuppressFinalize(this);
		}
        
		isDisposed = true;
	}
}