using System.Numerics;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a node that displays a single texture with configurable coordinates and size.
/// Inherits <see cref="ITexturePart"/> for shared texture path, coordinates, and size properties.
/// </summary>
public interface ISingleTextureNode : ITexturePart {

    /// <summary>
    /// Gets or sets the texture's U coordinate for positioning.
    /// </summary>
    float U { get; set; }

    /// <summary>
    /// Gets or sets the texture's V coordinate for positioning.
    /// </summary>
    float V { get; set; }

    /// <summary>
    /// Gets or sets the texture's width.
    /// </summary>
    float TextureWidth { get; set; }

    /// <summary>
    /// Gets or sets the texture's height.
    /// </summary>
    float TextureHeight { get; set; }

    /// <summary>
    /// Gets the texture's actual size, or <see cref="Vector2.Zero"/> if not ready.
    /// </summary>
    Vector2 ActualTextureSize { get; }
}
