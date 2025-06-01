using System;
using System.Collections.Generic;
using KamiToolKit.Classes;

namespace KamiToolKit.Addon;

public abstract partial class NativeAddon : IDisposable {

	private static readonly List<NativeAddon> CreatedAddons = [];

	private bool isDisposed;
	
	~NativeAddon() => Dispose(false);

	protected virtual void Dispose(bool disposing) {
		if (disposing) {
			Dispose();
		}
	}

	public void Dispose() {
		if (!isDisposed) {
			Log.Debug($"[KamiToolKit] Disposing addon {GetType()}");

			Close();
			GC.SuppressFinalize(this);
			CreatedAddons.Remove(this);
		}
        
		isDisposed = true;
	}

	internal static void DisposeAddons() {
		foreach (var addon in CreatedAddons.ToArray()) {
			Log.Warning($"[KamiToolKit] Addon {addon.GetType()} was not disposed properly please ensure you call dispose at an appropriate time.");
			Log.Debug($"[KamiToolKit] Automatically disposing addon {addon.GetType()} as a safety measure.");
			
			addon.Dispose();
		}
	}
}