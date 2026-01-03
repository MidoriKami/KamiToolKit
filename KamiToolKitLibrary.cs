using System;
using System.Collections.Concurrent;
using Dalamud.Plugin;
using KamiToolKit.Classes;
using Serilog.Events;

namespace KamiToolKit;

public static class KamiToolKitLibrary {
    internal static bool IsInitialized { get; private set; }
    
    internal static ConcurrentDictionary<nint, Type>? AllocatedNodes;
    
    /// <summary>
    /// Main initialization method for KamiToolKit. This method is required to be invoked before any KamiToolKit features are used.
    /// Failure to do so will not result in any direct warnings, but will result in undefined behavior.
    /// </summary>
    public static void Initialize(IDalamudPluginInterface pluginInterface) {
        IsInitialized = true;

        // Inject non-Experimental Properties
        pluginInterface.Inject(DalamudInterface.Instance);
        DalamudInterface.Instance.GameInteropProvider.InitializeFromAttributes(DalamudInterface.Instance);

        // Create node datashare
        AllocatedNodes = DalamudInterface.Instance.PluginInterface.GetOrCreateData("KamiToolKitAllocatedNodes", () => new ConcurrentDictionary<nint, Type>());
        
        // Inject Experimental Properties
        pluginInterface.Inject(Experimental.Instance);
        DalamudInterface.Instance.GameInteropProvider.InitializeFromAttributes(Experimental.Instance);

        Experimental.Instance.EnableHooks();

        // Force enable Verbose so that users are able to get advanced logging information on request.
        DalamudInterface.Instance.Log.MinimumLogLevel = LogEventLevel.Verbose;

        DalamudInterface.Instance.Log.Info($"KamiToolKit initialized for {pluginInterface.InternalName}");
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

        DalamudInterface.Instance.PluginInterface.RelinquishData("KamiToolKitAllocatedNodes");
        
        Experimental.Instance.DisposeHooks();
    }
}
