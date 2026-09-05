using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Threading.Tasks;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using KamiToolKit.BaseTypes;
using KamiToolKit.Classes;
using KamiToolKit.Debug;
using KamiToolKit.Internal.Classes;
using Serilog.Events;

namespace KamiToolKit;

/// <summary>
/// Primary entry point for KamiToolKit, contains initialization and disposal code.
/// </summary>
public static class KamiToolKitLibrary {
    private const string NodeDataShareKey = "TypeMappedCustomNodes";

    /// <summary>
    /// Gets the <see cref="IDalamudPluginInterface"/> used for KamiToolKit.
    /// This can be accessed anytime after <see cref="InitializeAsync"/> has been called.
    /// </summary>
    public static IDalamudPluginInterface PluginInterface { get; private set; } = null!;

    internal static ConcurrentDictionary<nint, Type>? AllocatedNodes;

    internal static string? DefaultWindowSubtitle;

    internal static Experimental Experimental = new();

    internal static ResourceManager? ResourceManager;
    internal static CultureInfo? CurrentCulture;

    internal static AddonConfigFile? AddonConfigFile;

    private static bool debugMode;
    private static WindowSystem? debugWindowSystem;
    private static DebugWindow? debugWindow;

    /// <summary>
    /// Main initialization method for KamiToolKit. This method is required to be invoked before any KamiToolKit features are used.
    /// Failure to do so will not result in any direct warnings, but will result in undefined behavior.
    /// </summary>
    public static async Task InitializeAsync(IDalamudPluginInterface pluginInterface, string? defaultWindowSubtitle = null) {
        DefaultWindowSubtitle = defaultWindowSubtitle;
        PluginInterface = pluginInterface;

        // Inject non-Experimental Properties
        PluginInterface.Create<Services>();
        IGameInteropProvider.Get().InitializeFromAttributes(Experimental);

        // Create node data share
        AllocatedNodes = PluginInterface.GetOrCreateData(NodeDataShareKey, () => new ConcurrentDictionary<nint, Type>());

        // Force enable Verbose so that users are able to get advanced logging information on request.
        IPluginLog.Get().MinimumLogLevel = LogEventLevel.Verbose;

        IPluginLog.Get().Info($"KamiToolKit initialized for {PluginInterface.InternalName} Default SubTitle: '{defaultWindowSubtitle}'");

        AddonConfigFile = await LoadAddonConfigFileAsync();

        await IFramework.Get().Run(NativeAddon.InitializeCloseCallback);

        RegisterDebugHelpers();
    }

    /// <summary>
    /// Sets the resource manager that KTK will use to resolve enums.
    /// </summary>
    public static void SetResourceManager(ResourceManager resourceManager) {
        ResourceManager = resourceManager;
    }

    /// <summary>
    /// Sets the current culture that KTK will use to resolve enums.
    /// </summary>
    public static void SetCurrentCulture(CultureInfo culture) {
        CurrentCulture = culture;
    }

    /// <summary>
    /// Loads or creates AddonConfigFile, and then tries to migrate any {internalName}.addon.json files to new version.
    /// </summary>
    private static async Task<AddonConfigFile> LoadAddonConfigFileAsync() {
        var configFile = await AddonConfigFile.LoadAsync();
        await configFile.TryMigrateOldConfigs();

        return configFile;
    }

    [Conditional("DEBUG")]
    private static void RegisterDebugHelpers() {
        debugMode = true;

        debugWindowSystem = new WindowSystem($"KamiToolKit - {PluginInterface.InternalName}");

        ICommandManager.Get().AddHandler($"/ktkdebug_{PluginInterface.InternalName}", new CommandInfo(CommandHandler) {
            HelpMessage = "Plugin was built in debug mode, enabling debug command.",
        });

        PluginInterface.UiBuilder.Draw += debugWindowSystem.Draw;

        debugWindow = new DebugWindow();
        debugWindowSystem.AddWindow(debugWindow);
    }

    private static void CommandHandler(string command, string arguments) {
        if (command != $"/ktkdebug_{PluginInterface.InternalName}") return;

        IPluginLog.Get().Debug($"Command Received, opening KTK Debug Window for {PluginInterface.InternalName}");
        debugWindow?.IsOpen = !debugWindow.IsOpen;
    }

    /// <summary>
    /// Disposes KamiToolKit resources.
    /// </summary>
    public static void Dispose() {
        if (!ThreadSafety.IsMainThread) {
            IPluginLog.Get().Error("Error, KamiToolKit tried to dispose while not on the main thread. Use DisposeAsync instead.");
            return;
        }

        if (debugMode) {
            PluginInterface.UiBuilder.Draw -= debugWindowSystem!.Draw;
            ICommandManager.Get().RemoveHandler($"/ktkdebug_{PluginInterface.InternalName}");
            debugWindowSystem.RemoveAllWindows();
        }

        NativeAddon.DisposeCloseCallback();

        NodeBase.WarnLeakedNodes();
        NativeAddon.WarnLeakedAddons();

        NativeAddon.DisposeAddons();
        NodeBase.DisposeNodes();

        PluginInterface.RelinquishData(NodeDataShareKey);
    }

    /// <summary>
    /// Disposes KamiToolkit resources async.
    /// </summary>
    public static async Task DisposeAsync() {
        if (debugMode) {
            PluginInterface.UiBuilder.Draw -= debugWindowSystem!.Draw;
            ICommandManager.Get().RemoveHandler($"/ktkdebug_{PluginInterface.InternalName}");
            debugWindowSystem.RemoveAllWindows();
        }

        await IFramework.Get().Run(NativeAddon.DisposeCloseCallback);

        NodeBase.WarnLeakedNodes();
        NativeAddon.WarnLeakedAddons();

        await IFramework.Get().Run(NativeAddon.DisposeAddons);
        await IFramework.Get().Run(NodeBase.DisposeNodes);

        PluginInterface.RelinquishData(NodeDataShareKey);
    }
}
