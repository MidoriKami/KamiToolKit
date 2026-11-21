using System.Numerics;
using KamiToolKit.Classes;
using KamiToolKit.Interfaces;

namespace KamiToolKit.Nodes.Simplified;

/// <summary>
/// A simplified implementation of a <see cref="ClippingMaskNode"/> with a pre-allocated texture part.
/// </summary>
public unsafe class SimpleClippingMaskNode : ClippingMaskNode, ISingleTextureNode, ITextureLoader {

    /// <inheritdoc />
    public float U {
        get => PartsList[0]->U;
        set => PartsList[0]->U = (ushort)value;
    }

    /// <inheritdoc />
    public float V {
        get => PartsList[0]->V;
        set => PartsList[0]->V = (ushort)value;
    }

    /// <inheritdoc />
    public Vector2 TextureCoordinates {
        get => new(U, V);
        set {
            U = value.X;
            V = value.Y;
        }
    }

    /// <inheritdoc />
    public float TextureHeight {
        get => PartsList[0]->Height;
        set => PartsList[0]->Height = (ushort)value;
    }

    /// <inheritdoc />
    public float TextureWidth {
        get => PartsList[0]->Width;
        set => PartsList[0]->Width = (ushort)value;
    }

    /// <inheritdoc />
    public Vector2 TextureSize {
        get => new(TextureWidth, TextureHeight);
        set {
            TextureWidth = value.X;
            TextureHeight = value.Y;
        }
    }

    /// <inheritdoc />
    public string TexturePath {
        get => PartsList[0]->LoadedPath;
        set => PartsList[0]->LoadTexture(value);
    }

    /// <inheritdoc />
    public Vector2 ActualTextureSize
        => PartsList[0]->LoadedTextureSize;

    /// <inheritdoc />
    public void LoadTexture(string path, bool resolveTheme = true)
        => PartsList[0]->LoadTexture(path, resolveTheme);

    /// <inheritdoc />
    public void LoadIcon(uint iconId)
        => PartsList[0]->LoadIcon(iconId);

    /// <summary>
    /// Constructs a new <see cref="SimpleClippingMaskNode"/>
    /// </summary>
    public SimpleClippingMaskNode()
        => PartsList.Add(new Part());
}
