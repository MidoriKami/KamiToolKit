using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KamiToolKit.Internal.Classes;
using Serilog;

namespace KamiToolKit.BaseTypes;

public partial class NativeAddon : IDisposable, IAsyncDisposable {

    /// <summary>
    /// Triggers the disposal of this addon. <br/> <br/>
    /// This will not await the addon to actually close which happens several frames later
    /// due to the addons closing animation. If you need to fully wait for the window to close use <see cref="DisposeAsync"/> and await the result.
    /// </summary>
    /// <code>await thisInstance.DisposeAsync();</code>
    public virtual void Dispose() {
        if (IsOverlayAddon) {
            // Intentionally leak OverlayAddons,
            // until Dalamud can implement OverlayAddons globally.
            CreatedAddons.Remove(this);
            GC.SuppressFinalize(this);
            return;
        }

        if (!isDisposed) {
            Services.Log.Debug($"Disposing addon {GetType()}");

            Close();

            // Close will remove this node automatically on AtkUnitBase.Finalize,
            // However, this is after the plugin unloads,
            // and will trigger a warning in auto-dispose if we don't remove this now.
            CreatedAddons.Remove(this);

            GC.SuppressFinalize(this);
        }

        isDisposed = true;
    }

    /// <summary>
    /// Triggers the disposal of this addon, and awaits for it to fully close before returning <see cref="ValueTask.CompletedTask"/>
    /// </summary>
    /// <remarks>
    /// This <em>must not</em> be called from the main thread, or it will deadlock the game.
    /// </remarks>
    public virtual async ValueTask DisposeAsync() {
        if (IsOverlayAddon) {
            // Intentionally leak OverlayAddons,
            // until Dalamud can implement OverlayAddons globally.
            CreatedAddons.Remove(this);
            GC.SuppressFinalize(this);
            return;
        }

        if (!isDisposed) {
            Services.Log.Debug($"Disposing addon {GetType()}");

            await CloseAsync();

            CreatedAddons.Remove(this);

            GC.SuppressFinalize(this);
        }

        isDisposed = true;
    }

    ~NativeAddon() {
        Log.Warning("KamiToolKit Addon Title: '{title}' InternalName: '{internalName}' was disposed via GC, this shouldn't happen.", Title, InternalName);
        Task.Run(DisposeAsync);
    }

    internal static void WarnLeakedAddons() {
        foreach (var addon in CreatedAddons.ToArray()) {
            if (addon.IsOverlayAddon) continue;

            Services.Log.Warning($"Addon {addon.GetType()} was not disposed properly please ensure you call dispose at an appropriate time.");
            Services.Log.Debug($"Automatically disposing addon {addon.GetType()} as a safety measure.");
        }
    }

    internal static void DisposeAddons() {
        foreach (var addon in CreatedAddons.ToArray()) {
            if (addon.IsOverlayAddon) continue;

            addon.Dispose();
        }

        CreatedAddons.Clear();
    }

    private static readonly List<NativeAddon> CreatedAddons = [];

    private bool isDisposed;
}
