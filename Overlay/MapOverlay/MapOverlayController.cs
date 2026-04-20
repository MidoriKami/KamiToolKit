using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Overlay.MapOverlay;

public unsafe class MapOverlayController() : MapOverlayControllerBase<AddonAreaMap>("AreaMap") {
    protected override AtkComponentNode* GetMapComponentNode(AddonAreaMap* addon)
        => addon->GetComponentNodeById(53);

    protected override Atk2DMap* GetMapDataPointer(AddonAreaMap* addon)
        => &addon->AreaMap.Atk2DMap;
}
