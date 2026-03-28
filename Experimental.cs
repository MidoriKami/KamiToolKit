using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit;

internal unsafe class Experimental {
    public delegate void PopulateTextLabelsDelegate(AtkUldManager* atkUldManager);

    [Signature("E8 ?? ?? ?? ?? 45 84 F6 74 59")]
    public static PopulateTextLabelsDelegate? SetupTextRecursive = null;
}
