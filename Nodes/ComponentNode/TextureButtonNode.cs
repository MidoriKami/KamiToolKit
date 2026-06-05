using System.Numerics;
using KamiToolKit.Enums;
using KamiToolKit.Premade.Node.Simple;

namespace KamiToolKit.Nodes;

/// <summary>
/// Specialization of a button that allows setting a texture part as a button.
/// </summary>
public class TextureButtonNode : ButtonBase {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public SimpleImageNode ImageNode { get; }

    /// <summary>
    /// Gets or sets the texture path used for the image.
    /// </summary>
    public string TexturePath {
        get => ImageNode.TexturePath;
        set => ImageNode.TexturePath = value;
    }

    /// <summary>
    /// Gets or sets the UV coordinates of the texture.
    /// </summary>
    public Vector2 TextureCoordinates {
        get => ImageNode.TextureCoordinates;
        set => ImageNode.TextureCoordinates = value;
    }

    /// <summary>
    /// Gets or sets the texture Width/Height.
    /// </summary>
    public Vector2 TextureSize {
        get => ImageNode.TextureSize;
        set => ImageNode.TextureSize = value;
    }

    public TextureButtonNode() {
        ImageNode = new ImGuiImageNode {
            WrapMode = WrapMode.Stretch,
        };
        ImageNode.AttachNode(this);

        LoadTimelines();

        InitializeComponentEvents();
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();
        ImageNode.Size = Size;
    }

    private void LoadTimelines()
        => LoadTwoPartTimelines(this, ImageNode);
}
