using System.Numerics;
using KamiToolKit.Core.Enums;
using KamiToolKit.Core.Interfaces;
using KamiToolKit.UI.Nodes.Simplified;

namespace KamiToolKit.UI.Nodes;

/// <summary>
/// Specialization of a button that allows setting a texture part as a button.
/// </summary>
public class TextureButtonNode : ButtonBase, ITextureButtonNode {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public SimpleImageNode ImageNode { get; }

    /// <inheritdoc/>
    public string TexturePath {
        get => ImageNode.TexturePath;
        set => ImageNode.TexturePath = value;
    }

    /// <inheritdoc/>
    public Vector2 TextureCoordinates {
        get => ImageNode.TextureCoordinates;
        set => ImageNode.TextureCoordinates = value;
    }

    /// <inheritdoc/>
    public Vector2 TextureSize {
        get => ImageNode.TextureSize;
        set => ImageNode.TextureSize = value;
    }

    /// <summary>
    /// Constructs a new <see cref="TextureButtonNode"/>
    /// </summary>
    public TextureButtonNode() {
        ImageNode = new SimpleImageNode {
            WrapMode = WrapMode.Stretch,
        };
        ImageNode.AttachNode(this);

        LoadTimelines();

        InitializeComponentEvents();
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();
        ImageNode.Size = Size;
    }

    private void LoadTimelines()
        => LoadTwoPartTimelines(this, ImageNode);
}
