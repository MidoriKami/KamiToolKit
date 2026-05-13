using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
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
    public virtual bool HideWithUiToggled => true;

    public override bool IsVisible { get; set; } = true;

    public void Update() {
        OnUpdate();

        var showWithNativeUi = !(HideWithNativeUi && !IsNameplateVisible());
        var showWithUiToggled = !(HideWithUiToggled && IsUiHidden());

        base.IsVisible = IsVisible && showWithNativeUi && showWithUiToggled;
    }

    protected abstract void OnUpdate();

    private static bool IsNameplateVisible() {
        var nameplateAddon = RaptureAtkUnitManager.Instance()->GetAddonByName("NamePlate");
        if (nameplateAddon is null) return false;

        return nameplateAddon->IsVisible;
    }

    private static bool IsUiHidden()
        => RaptureAtkUnitManager.Instance()->Flags.HasFlag(AtkUnitManagerFlags.UiHidden);
}
