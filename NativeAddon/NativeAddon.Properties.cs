using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Text.ReadOnly;

namespace KamiToolKit;

public abstract unsafe partial class NativeAddon {
    public void SetWindowPosition(Vector2 windowPosition) {
        if (InternalAddon is null) return;
        InternalAddon->SetPosition((short)windowPosition.X, (short)windowPosition.Y);
    }

    public void SetWindowSize(Vector2 windowSize) {
        if (InternalAddon is null) return;

        Size = windowSize;
        InternalAddon->SetSize((ushort)Size.X, (ushort)Size.Y);

        WindowNode?.Size = Size;
    }

    protected void SetWindowSize(float width, float height)
        => SetWindowSize(new Vector2(width, height));

    public required string InternalName {
        get;
        init => field = new string(value.Replace(" ", "").Take(31).ToArray());
    }

    public required ReadOnlySeString Title { get; set; }

    public ReadOnlySeString? Subtitle { get; set; }

    public int OpenWindowSoundEffectId { get; set; } = 23;

    public Vector2 Size { get; set; } = new(400.0f, 400.0f);

    public Vector2 ContentStartPosition => (WindowNode?.ContentStartPosition ?? Vector2.Zero) + ContentPadding;

    public Vector2 ContentSize => (WindowNode?.ContentSize ?? Vector2.Zero) - ContentPadding * 2.0f - new Vector2(0.0f, 4.0f);

    public Vector2 ContentPadding { get; set; } = new(8.0f, 0.0f);

    public int DepthLayer { get; init; } = 5;

    public bool IsOpen => InternalAddon is not null && InternalAddon->IsVisible;

    public int AddonId => InternalAddon is null ? 0 : InternalAddon->Id;

    public bool RememberClosePosition { get; set; } = true;
    
    internal Vector2 LastClosePosition = Vector2.Zero;

    public static implicit operator AtkUnitBase*(NativeAddon addon) => addon.InternalAddon;

    internal bool IsOverlayAddon { get; init; }

    public bool OpenInBounds { get; init; } = true;
}
