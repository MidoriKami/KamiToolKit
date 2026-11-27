using System.Numerics;
using KamiToolKit.Core.Enums;
using KamiToolKit.UI.Nodes.Parts;
using KamiToolKit.Core.Interfaces.Nodes;

namespace KamiToolKit.UI.Nodes.Simplified;

/// <summary>
/// Simplified implementation of an <see cref="ImageNode"/> meant to represent a single texture.
/// </summary>
/// <remarks>
/// This node is not intended to be used with multiple <see cref="Part" />'s.
/// </remarks>
public unsafe class SimpleImageNode : ImageNode, ISingleTextureNode, ITextureLoader {

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
    public virtual string TexturePath {
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

    /// <inheritdoc />
    public void LoadUldPart(string uldPath, uint partId, uint partSet, bool resolveTheme = true)
        => PartsList[0]->TryLoadFromUld(uldPath, partId, partSet, resolveTheme);

    /// <summary>
    /// Constructs a new <see cref="SimpleImageNode"/>
    /// </summary>
    public SimpleImageNode()
        => PartsList.Add(new Part());

    /// <summary>
    /// Constructs a new <see cref="SimpleImageNode"/> with a single part loaded from uld.
    /// </summary>
    public SimpleImageNode(string uldPath, uint partId, uint partSet = 0, bool resolveTheme = true) {
        PartsList.Add(new Part());
        LoadUldPart(uldPath, partId, partSet, resolveTheme);
        WrapMode = WrapMode.Stretch;
    }
}
