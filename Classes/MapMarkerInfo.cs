using System.Numerics;
using Dalamud.Interface.Textures.TextureWraps;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Classes;

public class MapMarkerInfo {
    public uint MapId { get; set; }
    public Vector2? Position { get; set; }
    public Vector2? Size { get; set; } = new Vector2(32.0f, 32.0f);
    public uint? IconId { get; set; }
    public IDalamudTextureWrap? Texture { get; set; }
    public string? TexturePath { get; set; }
    public ReadOnlySeString? Tooltip { get; set; }
    public bool AllowAnyMap { get; set; }
}
