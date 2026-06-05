using System.Linq;
using System.Numerics;
using Lumina.Text.ReadOnly;

namespace KamiToolKit;

/// <summary>
/// NativeAddon partial for various properties.
/// </summary>
public unsafe partial class NativeAddon {

    /// <summary>
    /// Gets or inits the addons internal name.
    /// </summary>
    /// <remarks>
    /// Names are limited to 31 characters.
    /// </remarks>
    public required string InternalName {
        get;
        init => field = new string(value.Replace(" ", "").Take(31).ToArray());
    }

    /// <summary>
    /// Gets or sets the addons main title string.
    /// </summary>
    public required ReadOnlySeString Title { get; set; }

    /// <summary>
    /// Gets or sets the addons subtitle string, defaults to <see cref="KamiToolKitLibrary.DefaultWindowSubtitle"/> set via <see cref="KamiToolKitLibrary.Initialize"/>.
    /// </summary>
    /// <remarks>
    /// It is recommended to only change this if your windows main title is already representative of your plugins name.
    /// </remarks>
    public ReadOnlySeString? Subtitle { get; set; }

    /// <summary>
    /// Sound effect to play when opening or closing this addon.
    /// </summary>
    public int OpenWindowSoundEffectId { get; set; } = 23;

    /// <summary>
    /// Gets or sets this addons size, defaults to 400px by 400px.
    /// </summary>
    public Vector2 Size { get; set; } = new(400.0f, 400.0f);

    /// <summary>
    /// Gets the position of the content body start.
    /// </summary>
    /// <remarks>
    /// This is the bottom left of the header node plus some <see cref="ContentPadding"/>.
    /// </remarks>
    public Vector2 ContentStartPosition
        => (WindowNode?.ContentStartPosition ?? Vector2.Zero) + ContentPadding;

    /// <summary>
    /// Gets the size of the body of the window.
    /// </summary>
    /// <remarks>
    /// This is the size of the window minus the size of the header, minus 2x <see cref="ContentPadding"/>
    /// </remarks>
    public Vector2 ContentSize
        => (WindowNode?.ContentSize ?? Vector2.Zero) - ContentPadding * 2.0f - new Vector2(0.0f, 4.0f);

    /// <summary>
    /// Gets or sets the padding used for the content area.
    /// </summary>
    public Vector2 ContentPadding { get; set; } = new(8.0f, 0.0f);

    /// <summary>
    /// Gets or sets the depth layer this window will open on.
    /// </summary>
    public int DepthLayer { get; init; } = 5;

    /// <summary>
    /// Gets whether this window is open and visible.
    /// </summary>
    public bool IsOpen
        => InternalAddon is not null && InternalAddon->IsVisible;

    /// <summary>
    /// Gets this addons ID.
    /// </summary>
    public int AddonId
        => InternalAddon is null ? 0 : InternalAddon->Id;

    /// <summary>
    /// Gets or sets whether this addon should remove its close position.
    /// </summary>
    public bool RememberClosePosition { get; set; } = true;

    /// <summary>
    /// Gets or sets if this addon should be forced into the viewable area when opening.
    /// </summary>
    public bool OpenInBounds { get; init; } = true;

    internal Vector2 LastClosePosition = Vector2.Zero;

    internal bool IsOverlayAddon { get; init; }
}
