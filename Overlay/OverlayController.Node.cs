using FFXIVClientStructs.FFXIV.Client.UI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace KamiToolKit.Overlay;

public abstract unsafe class OverlayNode : SimpleOverlayNode {

    public abstract OverlayLayer OverlayLayer { get; }

    /// <summary>
    /// When true, this node will automatically hide when the game hides things like nameplates
    /// </summary>
    public virtual bool HideWithNativeUi => true;

    private bool preAutoHideState;

    public virtual void Update() {
        UpdateAutoHide();
    }

    private void UpdateAutoHide() {
        if (HideWithNativeUi) {
            if (IsNameplateVisible()) {
                IsVisible = preAutoHideState;
            }
            else {
                preAutoHideState = IsVisible;
                IsVisible = false;
            }
        }
    }

    private static bool IsNameplateVisible() {
        var nameplateAddon = RaptureAtkUnitManager.Instance()->GetAddonByName("NamePlate");
        if (nameplateAddon is null) return false;

        return nameplateAddon->IsVisible;
    }
}
