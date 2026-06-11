using System.Numerics;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes.Simplified;

/// <summary>
/// Simplified implementation of an <see cref="ImageNode"/> meant to represent a single texture.
/// </summary>
/// <remarks>
/// This node is not intended to be used with multiple <see cref="Part" />'s.
/// </remarks>
public unsafe class SimpleImageNode : ImageNode {

    /// <summary>
    /// Gets or sets the textures U position.
    /// </summary>
    public float U {
        get => PartsList[0]->U;
        set => PartsList[0]->U = (ushort)value;
    }

    /// <summary>
    /// Gets or sets the textures V position.
    /// </summary>
    public float V {
        get => PartsList[0]->V;
        set => PartsList[0]->V = (ushort)value;
    }

    /// <summary>
    /// Gets or sets the textures coordinates.
    /// </summary>
    public Vector2 TextureCoordinates {
        get => new(U, V);
        set {
            U = value.X;
            V = value.Y;
        }
    }

    /// <summary>
    /// Gets or sets the textures hegith.
    /// </summary>
    public float TextureHeight {
        get => PartsList[0]->Height;
        set => PartsList[0]->Height = (ushort)value;
    }

    /// <summary>
    /// Gets or sets the textures width.
    /// </summary>
    public float TextureWidth {
        get => PartsList[0]->Width;
        set => PartsList[0]->Width = (ushort)value;
    }

    /// <summary>
    /// Gets or sets the textures size.
    /// </summary>
    public Vector2 TextureSize {
        get => new(TextureWidth, TextureHeight);
        set {
            TextureWidth = value.X;
            TextureHeight = value.Y;
        }
    }

    /// <summary>
    /// Gets or sets the textures path.
    /// </summary>
    /// <remarks>
    /// Setting this will cause the texture to be loaded from game or from filesystem.
    /// </remarks>
    public virtual string TexturePath {
        get => PartsList[0]->LoadedPath;
        set => PartsList[0]->LoadTexture(value);
    }

    /// <summary>
    /// Gets the textures actual size.
    /// </summary>
    /// <remarks>
    /// Is Vector2.Zero when texture is invalid or not ready.
    /// </remarks>
    public Vector2 ActualTextureSize
        => PartsList[0]->LoadedTextureSize;

    /// <summary>
    /// Loads a texture of the given path, optionally with theme resolution disabled.
    /// </summary>
    public void LoadTexture(string path, bool resolveTheme = true)
        => PartsList[0]->LoadTexture(path, resolveTheme);

    /// <summary>
    /// Loads a specified iconId directly.
    /// </summary>
    public void LoadIcon(uint iconId)
        => PartsList[0]->LoadIcon(iconId);

    public SimpleImageNode() {
        PartsList.Add(new Part());
    }
}
