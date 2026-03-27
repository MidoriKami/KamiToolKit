using KamiToolKit.Classes;

namespace KamiToolKit;

public unsafe partial class NativeAddon {

    private void UpdateFlags() {

        // Disable Native AddonConfig
        InternalAddon->DisableAddonConfig = true;
        InternalAddon->DisableUserClose = DisableClose;
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

        // Disable Controller Nav
        FlagHelper.UpdateFlag(ref InternalAddon->Flags1A2, 0x2, true);

        // Enable ClickThrough
        FlagHelper.UpdateFlag(ref InternalAddon->Flags1A3, 0x40, true);
    }

    public bool DisableClose { get; init; }

    public bool DisableCloseTransition { get; init; }

    public bool EnableContextMenu { get; init; } = true;

    public bool DisableClamping { get; init; } = true;

    public bool DisableScaleContextOption { get; init; }

    public bool RespectCloseAll { get; set; } = true;

    public bool IgnoreGlobalScale { get; set; } = false;
}
