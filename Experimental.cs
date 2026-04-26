// ReSharper disable RedundantUnsafeContext

using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using InteropGenerator.Runtime;

namespace KamiToolKit;

internal unsafe class Experimental {
    public delegate bool InitializeAddonDelegate(RaptureAtkUnitManager* atkUnitManager, AtkUnitBase** atkUnitBase, CStringPointer addonName, uint valueCount, AtkValue* atkValues);

    [Signature("E8 ?? ?? ?? ?? 48 8B 45 9F 48 8D 7D 17")]
    public static InitializeAddonDelegate? InitializeAddonFunction = null!;
}
