using System;
using System.Runtime.CompilerServices;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace KamiToolKit.Extensions;

public static unsafe class RaptureAtkModuleExtensions {
    extension(ref RaptureAtkModule raptureAtkModule) {
        public RaptureAtkModule.AddonFactoryInfo* GetAddonFactoryInfo(string addonName) {
            var addonIndex = raptureAtkModule.AddonNames.FindIndex(name => string.Equals(name.ExtractText(), addonName, StringComparison.OrdinalIgnoreCase));
            if (addonIndex is -1) return null;

            var addonFactories = raptureAtkModule.AddonFactories;
            if (addonFactories.Length < addonIndex) return null;

            return (RaptureAtkModule.AddonFactoryInfo*) Unsafe.AsPointer(ref addonFactories[addonIndex]);
        }
    }
}
