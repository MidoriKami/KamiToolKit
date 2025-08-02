using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Addon;

public abstract unsafe partial class NativeAddon {

    public required string InternalName { get; init; } = "NameNotSet";

    public required string Title { get; set; } = "TitleNotSet";

    public string Subtitle { get; set; } = string.Empty;

    public required NativeController NativeController { get; init; }

    public int OpenWindowSoundEffectId { get; set; } = 23;

    public TitleMenuOptions TitleMenuOptions {
        get;
        set {
            field = value;
            if (InternalAddon is not null) {
                UpdateFlags();
            }
        }
    } = new();

    public WindowOptions WindowOptions {
        get;
        set {
            field = value;
            if (InternalAddon is not null) {
                UpdateFlags();
            }
        }
    } = new();

    public required Vector2 Size { get; set; }

    public Vector2 ContentStartPosition => WindowNode.ContentStartPosition + ContentPadding;

    public Vector2 ContentSize => WindowNode.ContentSize - ContentPadding * 2.0f;

    public Vector2 ContentPadding => new(8.0f, 8.0f);

    private Vector2? InternalPosition { get; set; }

    public Vector2 Position {
        get => GetPosition();
        set => InternalPosition = value;
    }

    public bool IsOpen => InternalAddon is not null && InternalAddon->IsVisible;

    public int AddonId => InternalAddon is null ? 0 : InternalAddon->Id;

    public bool RememberClosePosition { get; set; } = true;

    public static explicit operator AtkUnitBase*(NativeAddon addon) => addon.InternalAddon;

    private void SetInitialState() {
        WindowNode.SetTitle(Title, Subtitle);

        InternalAddon->OpenSoundEffectId = (short)OpenWindowSoundEffectId;

        InternalAddon->SetSize((ushort)Size.X, (ushort)Size.Y);
        WindowNode.Size = Size;

        var screenSize = new Vector2(AtkStage.Instance()->ScreenSize.Width, AtkStage.Instance()->ScreenSize.Height);
        var defaultPosition = screenSize / 2.0f - Size / 2.0f;

        InternalAddon->SetPosition((short)defaultPosition.X, (short)defaultPosition.Y);

        UpdateFlags();
        UpdatePosition();
    }

    private void UpdateFlags() {
        // Note, some flags are default on, need to invert enable to clear them

        FlagHelper.UpdateFlag(ref InternalAddon->Flags1A3, 0x20, WindowOptions.DisableClamping);
        FlagHelper.UpdateFlag(ref InternalAddon->Flags1A3, 0x40, WindowOptions.EnableClickThrough);
        FlagHelper.UpdateFlag(ref InternalAddon->Flags1A3, 0x1, TitleMenuOptions.Enable);
        FlagHelper.UpdateFlag(ref InternalAddon->Flags1A1, 0x4, !TitleMenuOptions.ShowClose);
        FlagHelper.UpdateFlag(ref InternalAddon->Flags1C8, 0x800, !TitleMenuOptions.ShowScale);
    }

    public Vector2 GetPosition()
        => new(InternalAddon->X, InternalAddon->Y);

    private void UpdatePosition() {
        if (InternalPosition is { } position) {
            InternalAddon->SetPosition((short)position.X, (short)position.Y);
            InternalPosition = null;
        }
    }
}

public class TitleMenuOptions {

    /// <summary>
    ///     Enables right-clicking on the window header to open the window context menu
    /// </summary>
    public bool Enable { get; set; } = true;

    /// <summary>
    ///     Enable showing a close button in the context menu
    /// </summary>
    public bool ShowClose { get; set; } = true;

    /// <summary>
    ///     Enable showing the scale selector in the window context menu
    /// </summary>
    public bool ShowScale { get; set; } = true;
}

public class WindowOptions {

    /// <summary>
    ///     Setting to <em>True</em> allows the window to be moved past the edge of the window.
    /// </summary>
    public bool DisableClamping { get; set; } = true;

    public bool EnableClickThrough { get; set; }
}
