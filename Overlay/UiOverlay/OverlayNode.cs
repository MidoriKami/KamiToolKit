using FFXIVClientStructs.FFXIV.Client.UI;
using KamiToolKit.Enums;
using KamiToolKit.Premade.Node.Simple;

namespace KamiToolKit.Overlay.UiOverlay;

public abstract unsafe class OverlayNode : SimpleOverlayNode {

    public abstract OverlayLayer OverlayLayer { get; }

    /// <summary>
    /// When true, this node will automatically hide when the game hides things like nameplates
    /// </summary>
    public virtual bool HideWithNativeUi => true;

    /// <summary>
    ///  When true, this node automatically hides when the Toggle UI Display Mode hotkey is used
    /// </summary>
    public virtual bool HideWithUIToggled => true;

    public override bool IsVisible { get; set; } = true;

    public void Update() {
        OnUpdate();

        base.IsVisible = IsVisible && !(HideWithNativeUi && !IsNameplateVisible()) && !(HideWithUIToggled && IsUIHidden());
    }

    protected abstract void OnUpdate();

    private static bool IsNameplateVisible() {
        var nameplateAddon = RaptureAtkUnitManager.Instance()->GetAddonByName("NamePlate");
        if (nameplateAddon is null) return false;

        return nameplateAddon->IsVisible;
    }

    private static bool IsUIHidden() => (RaptureAtkUnitManager.Instance()->Flags & AtkUnitManagerFlags.UiHidden) != 0;
}
