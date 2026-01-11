using System;
using System.IO;
using System.Runtime.CompilerServices;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace KamiToolKit.Classes;

internal class DalamudInterface {

    private static DalamudInterface? instance;
    public static DalamudInterface Instance => instance ??= new DalamudInterface();

    [PluginService] public IPluginLog Log { get; set; } = null!;
    [PluginService] public IAddonLifecycle AddonLifecycle { get; set; } = null!;
    [PluginService] public IDataManager DataManager { get; set; } = null!;
    [PluginService] public ITextureProvider TextureProvider { get; set; } = null!;
    [PluginService] public IFramework Framework { get; set; } = null!;
    [PluginService] public IAddonEventManager AddonEventManager { get; set; } = null!;
    [PluginService] public IDalamudPluginInterface PluginInterface { get; set; } = null!;
    [PluginService] public IGameGui GameGui { get; set; } = null!;
    [PluginService] public IGameInteropProvider GameInteropProvider { get; set; } = null!;
    [PluginService] public ISeStringEvaluator SeStringEvaluator { get; set; } = null!;

    private DalamudInterface() {
        if (!KamiToolKitLibrary.IsInitialized)
            throw new Exception("KamiToolKit not initialized! You must call KamiToolKitLibrary.Initialize() before using KamiToolKit.\n" +
                                "Don't forget to call KamiToolKitLibrary.Dispose() in your plugins dispose to ensure all assets are freed and to trigger bad practice warnings.");
    }

    public string GetAssetDirectoryPath()
        => Path.Combine(PluginInterface.AssemblyLocation.DirectoryName ?? throw new Exception("Directory from Dalamud is Invalid Somehow"), "Assets");
    
    public string GetAssetPath(string assetName)
        => Path.Combine(GetAssetDirectoryPath(), assetName);

    public IDalamudTextureWrap? LoadAsset(string assetName)
        => TextureProvider.GetFromFile(GetAssetPath(assetName)).GetWrapOrDefault();
}

internal static class Log {

    private static readonly bool ExcessiveLogging = false;

    internal static void Debug(string message) {
        DalamudInterface.Instance.Log.Debug($"[KamiToolKit] {message}");
    }

    internal static void Fatal(string message) {
        DalamudInterface.Instance.Log.Fatal($"[KamiToolKit] {message}");
    }

    internal static void Warning(string message) {
        DalamudInterface.Instance.Log.Warning($"[KamiToolKit] {message}");
    }

    internal static void Verbose(string message) {
        DalamudInterface.Instance.Log.Verbose($"[KamiToolKit] {message}");
    }

    internal static void Excessive(string message) {
        if (ExcessiveLogging) {
            Verbose($"[KamiToolKit] {message}");
        }
    }

    internal static void Error(string message) {
        DalamudInterface.Instance.Log.Error($"[KamiToolKit] {message}");
    }

    internal static void Exception(Exception exception, [CallerMemberName] string? callerName = null) {
        DalamudInterface.Instance.Log.Error(exception, $"Exception in {callerName}");
    }
}
