using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Overlay.MapOverlay;

public class MiniMapOverlayController() : MapOverlayControllerBase<AddonNaviMap>("_NaviMap") {
    protected override unsafe AtkComponentNode* GetMapComponentNode(AddonNaviMap* addon)
        => addon->GetComponentNodeById(18);

    protected override unsafe Atk2DMap* GetMapDataPointer(AddonNaviMap* addon)
        => &addon->NaviMap.Atk2DMap;
}
