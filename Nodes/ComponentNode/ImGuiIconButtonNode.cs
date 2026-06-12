using System.Numerics;
using Dalamud.Interface.Textures.TextureWraps;
using KamiToolKit.Nodes.Simplified;

namespace KamiToolKit.Nodes;

/// <summary>
/// Specialization of a button that uses a IDalamudTextureWrap or file from disk as a button.
/// </summary>
public class ImGuiIconButtonNode : ButtonBase {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public NineGridNode BackgroundNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ImGuiImageNode ImageNode { get; }

    /// <summary>
    /// Gets or sets whether the border/background of the button should be shown.
    /// </summary>
    public bool ShowBackground {
        get => BackgroundNode.IsVisible;
        set => BackgroundNode.IsVisible = value;
    }

    /// <summary>
    /// Gets or sets the texture path for the image to display.
    /// Setting this will trigger it to be loaded from disk/game.
    /// </summary>
    public string TexturePath {
        get => ImageNode.TexturePath;
        set => ImageNode.TexturePath = value;
    }

    /// <summary>
    /// Use a texture that has already been created.
    /// </summary>
    /// <remarks>
    /// This texture must continue to exist during the life of this node.
    /// </remarks>
    public void LoadTexture(IDalamudTextureWrap texture)
        => ImageNode.LoadTexture(texture);

    /// <summary>
    /// Loads a texture from the provided path, if the path is a game path will load from game files.
    /// Else the texture will be loaded from disk.
    /// </summary>
    public void LoadTextureFromFile(string path)
        => ImageNode.TexturePath = path;

    public ImGuiIconButtonNode() {
        BackgroundNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/BgParts.tex",
            TextureSize = new Vector2(32.0f, 32.0f),
            TextureCoordinates = new Vector2(33.0f, 65.0f),
            TopOffset = 8.0f,
            LeftOffset = 8.0f,
            RightOffset = 8.0f,
            BottomOffset = 8.0f,
        };
        BackgroundNode.AttachNode(this);

        ImageNode = new ImGuiImageNode {
            FitTexture = true,
        };
        ImageNode.AttachNode(this);

        LoadTimelines();

        InitializeComponentEvents();
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        ImageNode.Size = Size - new Vector2(16.0f, 16.0f);
        ImageNode.Position = BackgroundNode.Position + new Vector2(BackgroundNode.LeftOffset, BackgroundNode.TopOffset);
        BackgroundNode.Size = Size;
    }

    private void LoadTimelines()
        => LoadThreePartTimelines(this, BackgroundNode, ImageNode, new Vector2(8.0f, 8.0f));
}
