using System.Numerics;
using Dalamud.Interface.Textures.TextureWraps;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Classes;

/// <summary>
/// Data object representing a map marker.
/// </summary>
public class MapMarkerInfo {

    /// <summary>
    /// The map ID to limit this marker to. Use <see cref="AllowAnyMap"/> to allow in any map.
    /// </summary>
    public uint MapId { get; set; }

    /// <summary>
    /// The 2-Dimensional position of this Map marker, range is [0 -> 2048] for both X and Y, with 1024,1024 being the center of the map.
    /// </summary>
    public Vector2? Position { get; set; }

    /// <summary>
    /// The size of this map marker, default is 32x32
    /// </summary>
    public Vector2? Size { get; set; } = new Vector2(32.0f, 32.0f);

    /// <summary>
    /// The icon ID of this marker, if a Texture is set before setting this, it'll be replaced with the game icon.
    /// </summary>
    public uint? IconId { get; set; }

    /// <summary>
    /// The texture for this marker, if a IconId is set before setting this, it'll be replaced with the provided texture.
    /// </summary>
    public IDalamudTextureWrap? Texture { get; set; }

    /// <summary>
    /// A path to a file on the filesystem, if a IconId or Texture is set before setting this, they'll be replaced with the loaded texture.
    /// </summary>
    public string? TexturePath { get; set; }

    /// <summary>
    /// Text tooltip to show when mousing over the map marker.
    /// </summary>
    public ReadOnlySeString? Tooltip { get; set; }

    /// <summary>
    /// If <see langword="true"/> will show this marker in any map. If <see langword="false"/> will only show in the map that matches <see cref="MapId"/>
    /// </summary>
    public bool AllowAnyMap { get; set; }
}
