using System;
using System.IO;
using System.Numerics;
using System.Text.Json;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit;

public unsafe partial class NativeAddon {
    private readonly JsonSerializerOptions serializerOptions = new() {
        WriteIndented = true,
        IncludeFields = true,
    };
    
    private AddonConfig LoadAddonConfig() {
        var directory = DalamudInterface.Instance.PluginInterface.ConfigDirectory;
        var file = new FileInfo(Path.Combine(directory.FullName, $"{InternalName}.addon.json"));
        if (!file.Exists) {
            file.Create().Close();

            var newConfig = new AddonConfig();
            SaveAddonConfig(newConfig);
            return newConfig;
        }

        AddonConfig? addonConfig;
        
        try {
            var data = File.ReadAllText(file.FullName);
            addonConfig = JsonSerializer.Deserialize<AddonConfig>(data, serializerOptions);
            addonConfig ??= new AddonConfig();
        }
        catch (Exception e) {
            DalamudInterface.Instance.Log.Error(e, "Exception while deserializing AddonConfig, creating new config.");
            addonConfig = new AddonConfig();
            SaveAddonConfig(addonConfig);
        }
        
        return addonConfig;
    }

    private void SaveAddonConfig(AddonConfig addonConfig) {
        var directory = DalamudInterface.Instance.PluginInterface.ConfigDirectory;
        var file = new FileInfo(Path.Combine(directory.FullName, $"{InternalName}.addon.json"));

        var data = JsonSerializer.Serialize(addonConfig, serializerOptions);
        
        FilesystemUtil.WriteAllTextSafe(file.FullName, data);
    }
    
    private void SaveAddonConfig() {
        var configData = new AddonConfig {
            Position = new Vector2(InternalAddon->X, InternalAddon->Y),
            Scale = InternalAddon->Scale / AtkUnitBase.GetGlobalUIScale(),
        };
        
        SaveAddonConfig(configData);
    }
}
