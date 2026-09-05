using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Internal.Classes;

namespace KamiToolKit.BaseTypes;

public unsafe partial class NativeAddon {
    private AddonConfig LoadAddonConfig() {
        if (KamiToolKitLibrary.AddonConfigFile is null) {
            IPluginLog.Get().Error("Addon Config File failed to load. KamiToolKit.InitialzeAsync was not called or failed.");
            return new AddonConfig();
        }

        return KamiToolKitLibrary.AddonConfigFile.GetAddonConfig(InternalName);
    }

    private void SaveAddonConfig() {
        if (KamiToolKitLibrary.AddonConfigFile is null) {
            IPluginLog.Get().Error("Addon Config File failed to load. KamiToolKit.InitialzeAsync was not called or failed.");
            return;
        }

        var addonConfig = LoadAddonConfig();
        addonConfig.Position = new Vector2(InternalAddon->X, InternalAddon->Y);
        addonConfig.Scale = InternalAddon->Scale / AtkUnitBase.GetGlobalUIScale();

        Task.Run(KamiToolKitLibrary.AddonConfigFile.SaveAsync);
    }
}
