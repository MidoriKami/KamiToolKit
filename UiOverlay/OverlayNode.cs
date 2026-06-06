using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;

namespace KamiToolKit.UiOverlay;

/// <summary>
/// Node used in <see cref="OverlayController"/> to display ui elements in the HUD.
/// </summary>
public abstract unsafe class OverlayNode : ResNode {

    /// <summary>
    /// The UI Layer that should show this node.
    /// </summary>
    public abstract OverlayLayer OverlayLayer { get; }

    /// <summary>
    /// When true, this node will automatically hide when the game hides things like nameplates
    /// </summary>
    public virtual bool HideWithNativeUi => true;

    /// <summary>
    ///  When true, this node automatically hides when the Toggle UI Display Mode hotkey is used
    /// </summary>
    public virtual bool HideWithUiToggled => true;

    /// <summary>
    /// Gets or sets whether this node should be visible.
    /// </summary>
    public override bool IsVisible { get; set; } = true;

    /// <summary>
    /// Updates the nodes automatic visibility functions and then triggers <see cref="OnUpdate"/>
    /// </summary>
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
