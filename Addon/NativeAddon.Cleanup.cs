using System;
using KamiToolKit.Classes;

namespace KamiToolKit.Addon;

public partial class NativeAddon : IDisposable {
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
		}
        
		isDisposed = true;
	}
}