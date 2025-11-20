using KamiToolKit.Classes;

namespace KamiToolKit.Addon;

public unsafe partial class NativeAddon {

    private void UpdateFlags() {
        FlagHelper.UpdateFlag(ref InternalAddon->Flags1A1, 0x4, DisableClose);

        FlagHelper.UpdateFlag(ref InternalAddon->Flags1A2, 0x8, DisableCloseTransition);
        FlagHelper.UpdateFlag(ref InternalAddon->Flags1A2, 0x40, DisableAddonConfig);

        FlagHelper.UpdateFlag(ref InternalAddon->Flags1A3, 0x20, DisableClamping);
        FlagHelper.UpdateFlag(ref InternalAddon->Flags1A3, 0x1, EnableContextMenu);
        
        FlagHelper.UpdateFlag(ref InternalAddon->Flags1C8, 0x800, DisableScaleContextOption);
    }

    public bool DisableClose { get; init; }

    public bool DisableCloseTransition { get; init; }

    internal bool DisableAddonConfig { get; init; } = true;

    public bool EnableContextMenu { get; init; } = true;

    public bool DisableClamping { get; init; } = true;

    public bool DisableScaleContextOption { get; init; }

    public bool RespectCloseAll { get; set; } = true;
}
