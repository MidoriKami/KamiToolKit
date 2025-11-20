using Dalamud.Plugin;
using KamiToolKit.Addon;
using KamiToolKit.Classes;
using KamiToolKit.NodeBaseClasses;
using Serilog.Events;

namespace KamiToolKit;

public static class KamiToolKitLibrary {
    /// <summary>
    /// Main initialization method for KamiToolKit. This method is required to be invoked before any KamiToolKit features are used.
    /// Failure to do so will not result in any direct warnings, but will result in undefined behavior.
    /// </summary>
    public static void Initialize(IDalamudPluginInterface pluginInterface) {
        // Inject non-Experimental Properties
        pluginInterface.Inject(DalamudInterface.Instance);
        DalamudInterface.Instance.GameInteropProvider.InitializeFromAttributes(DalamudInterface.Instance);

        // Inject Experimental Properties
        pluginInterface.Inject(Experimental.Instance);
        DalamudInterface.Instance.GameInteropProvider.InitializeFromAttributes(Experimental.Instance);

        Experimental.Instance.EnableHooks();

        // Force enable Verbose so that users are able to get advanced logging information on request.
        DalamudInterface.Instance.Log.MinimumLogLevel = LogEventLevel.Verbose;
    }

    /// <summary>
    /// Alias for Cleanup
    /// </summary>
    public static void Dispose() => Cleanup();
    
    /// <summary>
    /// Alias for Cleanup
    /// </summary>
    public static void Shutdown() => Cleanup();

    /// <summary>
    /// Cleans up any potentially leaked resources that KamiToolKit has allocated.
    /// </summary>
    public static void Cleanup() {
        if (MainThreadSafety.TryAssertMainThread()) return;

        NodeBase.DisposeNodes();
        NativeAddon.DisposeAddons();

        Experimental.Instance.DisposeHooks();
    }
}
