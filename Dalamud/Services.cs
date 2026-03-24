using System;
using System.IO;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace KamiToolKit.Dalamud;

internal class Services {
    [PluginService] public static IPluginLog Log { get; set; } = null!;
    [PluginService] public static IAddonLifecycle AddonLifecycle { get; set; } = null!;
    [PluginService] public static IDataManager DataManager { get; set; } = null!;
    [PluginService] public static ITextureProvider TextureProvider { get; set; } = null!;
    [PluginService] public static IFramework Framework { get; set; } = null!;
    [PluginService] public static IAddonEventManager AddonEventManager { get; set; } = null!;
    [PluginService] public static IDalamudPluginInterface PluginInterface { get; set; } = null!;
    [PluginService] public static IGameGui GameGui { get; set; } = null!;
    [PluginService] public static IGameInteropProvider GameInteropProvider { get; set; } = null!;
    [PluginService] public static ISeStringEvaluator SeStringEvaluator { get; set; } = null!;
    [PluginService] public static IClientState ClientState { get; set; } = null!;

    public static string GetAssetPath(string assetName)
        => Path.Combine(GetAssetDirectoryPath(), assetName);

    private static string GetAssetDirectoryPath()
        => Path.Combine(PluginInterface.AssemblyLocation.DirectoryName ?? throw new Exception("Directory from Dalamud is Invalid Somehow"), "Assets");
}
