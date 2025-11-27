using System.Linq;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace KamiToolKit.Overlay;

public unsafe partial class OverlayController {

    private bool? lastNamePlateVisibility;

    private void OnNamePlateUpdate(AddonEvent type, AddonArgs args) {
        var isNamePlateVisible = IsNameplateVisible();
        lastNamePlateVisibility ??= isNamePlateVisible;

        if (lastNamePlateVisibility != isNamePlateVisible) {

            foreach (var node in overlayNodes.SelectMany(pair => pair.Value)) {
                if (node.HideWithNativeUi) {
                    if (isNamePlateVisible) {
                        node.IsVisible = node.PreAutoHideState;
                    }
                    else {
                        node.PreAutoHideState = node.IsVisible;
                        node.IsVisible = false;
                    }
                }
            }

            lastNamePlateVisibility = isNamePlateVisible;
        }
    }

    private static bool IsNameplateVisible() {
        var nameplateAddon = RaptureAtkUnitManager.Instance()->GetAddonByName("NamePlate");
        if (nameplateAddon is null) return false;

        return nameplateAddon->IsVisible;
    }
}
