using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Dalamud.Plugin.Services;
using KamiToolKit.Internal.Classes;

namespace KamiToolKit.Classes;

/// <summary>
/// Data model for reading and writing addon configs to and from a single file instead of one file per addon./plus
/// </summary>
internal class AddonConfigFile {

    public Dictionary<string, AddonConfig> AddonConfigs = [];

    /// <summary>
    /// Gets using an addons internal name, gets existing addon config, or creates a new one.
    /// </summary>
    /// <param name="addonName"></param>
    /// <returns></returns>
    public AddonConfig GetAddonConfig(string addonName) {
        if (AddonConfigs.TryGetValue(addonName, out var addonConfig)) return addonConfig;

        var newAddonConfig = new AddonConfig();
        AddonConfigs.Add(addonName, newAddonConfig);
        return newAddonConfig;
    }

    /// <summary>
    /// Saves to Plugin's ConfigDirectory under the name of "addonconfig.json"
    /// </summary>
    public async Task SaveAsync() {
        var directory = KamiToolKitLibrary.PluginInterface.ConfigDirectory;
        var fileInfo = new FileInfo(Path.Combine(directory.FullName, "addonconfig.json"));

        var data = JsonSerializer.Serialize(this, SerializerOptions);

        await IReliableFileStorage.Get().WriteAllTextAsync(fileInfo.FullName, data);
    }

    /// <summary>
    /// Loads from Plugin's ConfigDirectory for the name of "addonconfig.json"
    /// </summary>
    /// <returns></returns>
    public static async Task<AddonConfigFile> LoadAsync() {
        var directory = KamiToolKitLibrary.PluginInterface.ConfigDirectory;
        var fileInfo = new FileInfo(Path.Combine(directory.FullName, "addonconfig.json"));

        // If file doesn't exist yet, make it, and save it.
        if (!fileInfo.Exists) {
            fileInfo.Create().Close();

            var newAddonConfig = new AddonConfigFile();
            await newAddonConfig.SaveAsync();

            return newAddonConfig;
        }

        // File does exist, load it
        AddonConfigFile? addonConfig;

        try {
            var data = await IReliableFileStorage.Get().ReadAllTextAsync(fileInfo.FullName);
            addonConfig = JsonSerializer.Deserialize<AddonConfigFile>(data, SerializerOptions);

            // Somehow didn't throw an error, but file is still null, make new, and save it.
            if (addonConfig is null) {
                addonConfig ??= new AddonConfigFile();
                await addonConfig.SaveAsync();
            }
        }
        catch (Exception e) {

            // We had an error, make new, and save it.
            IPluginLog.Get().Error(e, "Exception while deserializing AddonConfig, creating new config.", "KamiToolKit");

            addonConfig = new AddonConfigFile();
            await addonConfig.SaveAsync();
        }

        return addonConfig;
    }

    /// <summary>
    /// If there are any "{internalName}.addon.json" files in the directory, migrate them into AddonConfigFile and delete the files.
    /// </summary>
    public async Task TryMigrateOldConfigs() {
        var configDirectory = KamiToolKitLibrary.PluginInterface.ConfigDirectory;
        var anyMigrated = false;

        foreach (var file in configDirectory.EnumerateFiles()) {
            if (!file.Name.Contains("addon.json")) continue;

            try {
                var fileData = await IReliableFileStorage.Get().ReadAllTextAsync(file.FullName);
                var addonConfig = JsonSerializer.Deserialize<AddonConfig>(fileData, SerializerOptions);

                if (addonConfig is null) continue;

                if (AddonConfigs.TryAdd(file.Name.Split(".").First(), addonConfig)) {
                    file.Delete();
                    anyMigrated = true;
                }
                else {
                    IPluginLog.Get().Debug($"File already migrated to AddonConfigFile: {file.Name}");
                }
            }
            catch (Exception) {
                IPluginLog.Get().Warning($"Failed to migrate {file.Name}, created new AddonConfig instead.");
                if (AddonConfigs.TryAdd(file.Name.Split(".").First(), new AddonConfig())) {
                    anyMigrated = true;
                }
            }
        }

        if (anyMigrated) {
            await SaveAsync();
        }
    }

    private static readonly JsonSerializerOptions SerializerOptions = new() {
        WriteIndented = true,
        IncludeFields = true,
    };
}
