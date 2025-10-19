using System;
using System.Collections.Generic;
using KamiToolKit.Classes;

namespace KamiToolKit.Addon;

public abstract partial class NativeAddon : IDisposable {

    private static readonly List<NativeAddon> CreatedAddons = [];

    private bool isDisposed;

    public virtual void Dispose() {
        if (!isDisposed) {
            Log.Debug($"正在释放 Addon {GetType()}");

            Close();

            // Close will remove this node automatically on AtkUnitBase.Finalize,
            // However, this is after the plugin unloads,
            // and will trigger a warning in auto-dispose if we don't remove this now.
            CreatedAddons.Remove(this);

            GC.SuppressFinalize(this);
        }

        isDisposed = true;
        DisposeExtras();
    }

    ~NativeAddon() => Dispose();

    internal static void DisposeAddons() {
        foreach (var addon in CreatedAddons.ToArray()) {
            Log.Warning($"Addon {addon.GetType()} 未被正确释放，请确认在合适的时机调用 Dispose。");
            Log.Debug($"出于安全考虑，正在自动释放 Addon {addon.GetType()}。");

            addon.Dispose();
        }
    }
}
