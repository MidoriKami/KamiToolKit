using System.Numerics;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a AtkUldTexture Part and info.
/// </summary>
public interface ITexturePart {

    /// <summary>
    /// Gets or sets the texture path used for the part.
    /// </summary>
    string TexturePath { get; set; }

    /// <summary>
    /// Gets or sets the UV coordinates of the texture.
    /// </summary>
    Vector2 TextureCoordinates { get; set; }

    /// <summary>
    /// Gets or sets the texture Width/Height.
    /// </summary>
    /// <remarks>
    /// This is the size of the part, not the full size of the spritesheet.
    /// </remarks>
    Vector2 TextureSize { get; set; }
}
