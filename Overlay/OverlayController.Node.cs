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
    private bool? lastNamePlateVisible;

    public void OnUpdate() { 
        Update();
        UpdateAutoHide();
    }

    protected abstract void Update();

    private void UpdateAutoHide() {
        if (HideWithNativeUi) {
            var isNamePlateVisible = IsNameplateVisible();

            if (lastNamePlateVisible is null || lastNamePlateVisible != isNamePlateVisible) {
                if (lastNamePlateVisible is null) {
                    preAutoHideState = IsVisible;
                }
                
                if (isNamePlateVisible) {
                    IsVisible = preAutoHideState;
                }
                else {
                    preAutoHideState = IsVisible;
                    IsVisible = false;
                }
                
                lastNamePlateVisible = isNamePlateVisible;
            }
        }
    }

    private static bool IsNameplateVisible() {
        var nameplateAddon = RaptureAtkUnitManager.Instance()->GetAddonByName("NamePlate");
        if (nameplateAddon is null) return false;

        return nameplateAddon->IsVisible;
    }
}
