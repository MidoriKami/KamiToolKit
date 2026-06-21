using System.Numerics;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes.Simplified;

/// <summary>
/// A simplified implementation of a <see cref="ClippingMaskNode"/> with a pre-allocated texture part.
/// </summary>
public unsafe class SimpleClippingMaskNode : ClippingMaskNode {

    /// <summary>
    /// Gets or sets the textures U value for texture positioning.
    /// </summary>
    public float U {
        get => PartsList[0]->U;
        set => PartsList[0]->U = (ushort)value;
    }

    /// <summary>
    /// Gets or sets the textures V value for texture positioning.
    /// </summary>
    public float V {
        get => PartsList[0]->V;
        set => PartsList[0]->V = (ushort)value;
    }

    /// <summary>
    /// Gets or sets the textures UV values for positioning.
    /// </summary>
    public Vector2 TextureCoordinates {
        get => new(U, V);
        set {
            U = value.X;
            V = value.Y;
        }
    }

    /// <summary>
    /// Gets or sets the textures height.
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
    /// Gets or sets the texture path.
    /// </summary>
    /// <remarks>
    /// Setting a path will cause it to be loaded from the game or filesystems.
    /// </remarks>
    public string TexturePath {
        get => PartsList[0]->LoadedPath;
        set => PartsList[0]->LoadTexture(value);
    }

    /// <summary>
    /// Gets the textures actual texture size or Vector2.Zero if not ready.
    /// </summary>
    public Vector2 ActualTextureSize
        => PartsList[0]->LoadedTextureSize;

    /// <summary>
    /// Loads the specified texture.
    /// </summary>
    public void LoadTexture(string path)
        => PartsList[0]->LoadTexture(path);

    /// <summary>
    /// Loads the specified icon.
    /// </summary>
    /// <param name="iconId"></param>
    public void LoadIcon(uint iconId)
        => PartsList[0]->LoadIcon(iconId);

    /// <summary>
    /// Constructs a new <see cref="SimpleClippingMaskNode"/>
    /// </summary>
    public SimpleClippingMaskNode() {
        PartsList.Add(new Part());
    }
}
