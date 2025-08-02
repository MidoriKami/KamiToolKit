using System;
using System.Collections.Generic;
using KamiToolKit.Classes;

namespace KamiToolKit.Addon;

public abstract partial class NativeAddon : IDisposable {

    private static readonly List<NativeAddon> CreatedAddons = [];

    private bool isDisposed;

    public void Dispose() {
        if (!isDisposed) {
            Log.Debug($"Disposing addon {GetType()}");

            Close();

            // Close will remove this node automatically on AtkUnitBase.Finalize,
            // However, this is after the plugin unloads,
            // and will trigger a warning in auto-dispose if we don't remove this now.
            CreatedAddons.Remove(this);

            GC.SuppressFinalize(this);
        }

        isDisposed = true;
    }

    ~NativeAddon() {
        Dispose(false);
    }

    protected virtual void Dispose(bool disposing) {
        if (disposing) {
            Dispose();
        }
    }

    internal static void DisposeAddons() {
        foreach (var addon in CreatedAddons.ToArray()) {
            Log.Warning($"Addon {addon.GetType()} was not disposed properly please ensure you call dispose at an appropriate time.");
            Log.Debug($"Automatically disposing addon {addon.GetType()} as a safety measure.");

            addon.Dispose();
        }
    }
}
