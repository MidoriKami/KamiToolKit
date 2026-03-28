using System;
using System.Collections.Concurrent;
using Dalamud.Plugin;
using KamiToolKit.Dalamud;
using Serilog.Events;

namespace KamiToolKit;

public static class KamiToolKitLibrary {
    internal static bool IsInitialized { get; private set; }
    internal static IDalamudPluginInterface? PluginInterface { get; private set; }
    
    internal static ConcurrentDictionary<nint, Type>? AllocatedNodes;
    
    internal static string? DefaultWindowSubtitle;

    internal static Experimental Experimental = new();

    /// <summary>
    /// Main initialization method for KamiToolKit. This method is required to be invoked before any KamiToolKit features are used.
    /// Failure to do so will not result in any direct warnings, but will result in undefined behavior.
    /// </summary>
    public static void Initialize(IDalamudPluginInterface pluginInterface, string? defaultWindowSubtitle = null) {
        IsInitialized = true;
        DefaultWindowSubtitle = defaultWindowSubtitle;
        PluginInterface = pluginInterface;

        // Inject non-Experimental Properties
        PluginInterface.Create<Services>();
        Services.GameInteropProvider.InitializeFromAttributes(Experimental);

        // Create node data share
        AllocatedNodes = PluginInterface.GetOrCreateData("KamiToolKitAllocatedNodes", () => new ConcurrentDictionary<nint, Type>());

        // Force enable Verbose so that users are able to get advanced logging information on request.
        Services.Log.MinimumLogLevel = LogEventLevel.Verbose;

        Services.Log.Info($"KamiToolKit initialized for {PluginInterface.InternalName}");
        
        NativeAddon.InitializeCloseCallback();
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
        NodeBase.DisposeNodes();
        NativeAddon.DisposeAddons();
        NativeAddon.DisposeCloseCallback();

        if (MainThreadSafety.TryAssertMainThread()) return;

        Services.PluginInterface.RelinquishData("KamiToolKitAllocatedNodes");
    }
}
