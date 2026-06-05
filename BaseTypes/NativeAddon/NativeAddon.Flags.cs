using KamiToolKit.Classes;

namespace KamiToolKit.BaseTypes;

public unsafe partial class NativeAddon {

    /// <summary>
    /// Disables the close button. Use with caution.
    /// </summary>
    public bool DisableClose { get; init; }

    /// <summary>
    /// Disables closing animation. (But doesn't actually...)
    /// </summary>
    public bool DisableCloseTransition { get; init; }

    /// <summary>
    /// Gets or sets whether right-clicking the header should show a context menu, for scaling or resetting this addon.
    /// </summary>
    public bool EnableContextMenu { get; init; } = true;

    /// <summary>
    /// Gets or sets whether this window should be able to be dragged off-screen.
    /// </summary>
    public bool DisableClamping { get; init; } = true;

    /// <summary>
    /// Gets or sets whether the context menu for this addon should allow changing the scale.
    /// </summary>
    public bool DisableScaleContextOption { get; init; }

    /// <summary>
    /// Gets or sets whether this addon should close when esc is pressed with no windows focused.
    /// </summary>
    public bool RespectCloseAll { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this addon should ignore AtkUnitBase.GlobalScale.
    /// </summary>
    public bool IgnoreGlobalScale { get; set; } = false;

    private void UpdateFlags() {

        // Disable Native AddonConfig
        InternalAddon->DisableAddonConfig = true;
        InternalAddon->ShouldFireCallbackAndHideOrClose = DisableClose;
        InternalAddon->DisableHideTransition = DisableCloseTransition;
        InternalAddon->EnableTitleBarContextMenu = EnableContextMenu;
        InternalAddon->DisableUserScaling = DisableScaleContextOption;

        FlagHelper.UpdateFlag(ref InternalAddon->Flags1A3, 1 << 5, DisableClamping);

        if (IsOverlayAddon) {
            SetOverlayFlags();
        }
    }

    private void SetOverlayFlags() {

        OpenWindowSoundEffectId = 0;
        InternalAddon->ShowSoundEffectId = 0;
        InternalAddon->DisableFocusability = true;
        InternalAddon->DisableFocusOnShow = true;
        InternalAddon->DisableHideTransition = true;
        InternalAddon->DisableShowHideSoundEffects = true;
        InternalAddon->IgnoreUIDisplayMode = true;

        // Disable Controller Nav
        FlagHelper.UpdateFlag(ref InternalAddon->Flags1A2, 0x2, true);

        // Enable ClickThrough
        FlagHelper.UpdateFlag(ref InternalAddon->Flags1A3, 0x40, true);
    }
}
