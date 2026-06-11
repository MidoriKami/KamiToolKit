using System.Numerics;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes.Simplified;

/// <summary>
/// Simplified implementation of <see cref="NineGridNode"/>
/// </summary>
public unsafe class SimpleNineGridNode : NineGridNode {

    /// <summary>
    /// Gets or sets the textures U coordinate.
    /// </summary>
    public float U {
        get => PartsList[0]->U;
        set => PartsList[0]->U = (ushort)value;
    }

    /// <summary>
    /// Gets or sets the textures V coordinate.
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
    /// Gets or sets the textures width.
    /// </summary>
    public float TextureWidth {
        get => PartsList[0]->Width;
        set => PartsList[0]->Width = (ushort)value;
    }

    /// <summary>
    /// Gets or sets the textures height.
    /// </summary>
    public float TextureHeight {
        get => PartsList[0]->Height;
        set => PartsList[0]->Height = (ushort)value;
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
    /// When setting loads the texture from the game or filesystem.
    /// </remarks>
    public string TexturePath {
        get => PartsList[0]->LoadedPath;
        set => PartsList[0]->LoadTexture(value);
    }

    public SimpleNineGridNode() {
        PartsList.Add(new Part());
    }
}
