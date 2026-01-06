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
    protected new bool IsVisible { get; set; }

    public void Update() { 
        OnUpdate();

        base.IsVisible = IsVisible && !(HideWithNativeUi && !IsNameplateVisible());
    }

    protected abstract void OnUpdate();

    private static bool IsNameplateVisible() {
        var nameplateAddon = RaptureAtkUnitManager.Instance()->GetAddonByName("NamePlate");
        if (nameplateAddon is null) return false;

        return nameplateAddon->IsVisible;
    }
}
