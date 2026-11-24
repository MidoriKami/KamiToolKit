using System;
using System.IO;

namespace KamiToolKit.Services;

internal static class AssetServices {
    public static string GetAssetPath(string assetName)
        => Path.Combine(GetAssetDirectoryPath(), assetName);

    private static string GetAssetDirectoryPath()
        => Path.Combine(KamiToolKitLibrary.PluginInterface.AssemblyLocation.DirectoryName ?? throw new Exception("Directory from Dalamud is Invalid Somehow"), "Assets");
}
