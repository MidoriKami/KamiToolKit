using System;
using System.Runtime.CompilerServices;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace KamiToolKit.Extensions;

/// <summary>
/// RaptureAtkModule Extensions.
/// </summary>
public static unsafe class RaptureAtkModuleExtensions {
    extension(ref RaptureAtkModule raptureAtkModule) {

        /// <summary>
        /// Gets a pointer to an addon factory for the provided addon name.
        /// </summary>
        /// <param name="addonName">Addon name to search for.</param>
        /// <returns>AddonFactory pointer, or null if invalid.</returns>
        public RaptureAtkModule.AddonFactoryInfo* GetAddonFactoryInfo(string addonName) {
            var addonIndex = raptureAtkModule.AddonNames.FindIndex(name => string.Equals(name.ExtractText(), addonName, StringComparison.OrdinalIgnoreCase));
            if (addonIndex is -1) return null;

            var addonFactories = raptureAtkModule.AddonFactories;
            if (addonFactories.Length < addonIndex) return null;

            return (RaptureAtkModule.AddonFactoryInfo*) Unsafe.AsPointer(ref addonFactories[addonIndex]);
        }
    }
}
