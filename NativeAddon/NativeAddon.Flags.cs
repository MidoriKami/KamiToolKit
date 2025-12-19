using KamiToolKit.Classes;

namespace KamiToolKit;

public unsafe partial class NativeAddon {

    private void UpdateFlags() {

        // Disable Native AddonConfig
        FlagHelper.UpdateFlag(ref InternalAddon->Flags1A2, 0x40, true);
        
        FlagHelper.UpdateFlag(ref InternalAddon->Flags1A1, 0x4, DisableClose);

        FlagHelper.UpdateFlag(ref InternalAddon->Flags1A2, 0x8, DisableCloseTransition);
        FlagHelper.UpdateFlag(ref InternalAddon->Flags1A2, 0x40, DisableAddonConfig);

        FlagHelper.UpdateFlag(ref InternalAddon->Flags1A3, 0x20, DisableClamping);
        FlagHelper.UpdateFlag(ref InternalAddon->Flags1A3, 0x1, EnableContextMenu);
        
        FlagHelper.UpdateFlag(ref InternalAddon->Flags1C8, 0x800, DisableScaleContextOption);

        if (IsOverlayAddon) {
            SetOverlayFlags();
        }
    }

    private void SetOverlayFlags() {

        OpenWindowSoundEffectId = 0;
        InternalAddon->OpenSoundEffectId = 0;

        // Disable ability to focus window
        FlagHelper.UpdateFlag(ref InternalAddon->Flags1A0, 0x80, true);
            
        // Don't load into FocusedAddons list
        FlagHelper.UpdateFlag(ref InternalAddon->Flags1A1, 0x40, true);

        // Disable open/close transitions
        FlagHelper.UpdateFlag(ref InternalAddon->Flags1A2, 0x8, true);
        
        // Disable open/close sounds
        FlagHelper.UpdateFlag(ref InternalAddon->Flags1A2, 0x20, true);
        
        // Enable ClickThrough
        FlagHelper.UpdateFlag(ref InternalAddon->Flags1A3, 0x40, true);
    }

    public bool DisableClose { get; init; }

    public bool DisableCloseTransition { get; init; }

    internal bool DisableAddonConfig { get; init; } = true;

    public bool EnableContextMenu { get; init; } = true;

    public bool DisableClamping { get; init; } = true;

    public bool DisableScaleContextOption { get; init; }

    public bool RespectCloseAll { get; set; } = true;

    public bool IgnoreGlobalScale { get; set; } = false;
}
