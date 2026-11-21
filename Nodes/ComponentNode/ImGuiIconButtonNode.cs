using System.Numerics;
using Dalamud.Interface.Textures.TextureWraps;
using KamiToolKit.Interfaces;
using KamiToolKit.Nodes.Simplified;

namespace KamiToolKit.Nodes;

/// <summary>
/// Specialization of a button that uses a IDalamudTextureWrap or file from disk as a button.
/// </summary>
public class ImGuiIconButtonNode : ButtonBase, IImGuiIconButtonNode {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public NineGridNode BackgroundNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ImGuiImageNode ImageNode { get; }

    /// <inheritdoc/>
    public bool ShowBackground {
        get => BackgroundNode.IsVisible;
        set => BackgroundNode.IsVisible = value;
    }

    /// <inheritdoc/>
    public string TexturePath {
        get => ImageNode.TexturePath;
        set => ImageNode.TexturePath = value;
    }

    /// <inheritdoc/>
    public void LoadTexture(IDalamudTextureWrap texture)
        => ImageNode.LoadTexture(texture);

    /// <inheritdoc/>
    public void LoadTexture(string path)
        => ImageNode.TexturePath = path;


    /// <summary>
    /// Constructs a new <see cref="ImGuiIconButtonNode"/>.
    /// </summary>
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

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        ImageNode.Size = Size - new Vector2(16.0f, 16.0f);
        ImageNode.Position = BackgroundNode.Position + new Vector2(BackgroundNode.LeftOffset, BackgroundNode.TopOffset);
        BackgroundNode.Size = Size;
    }

    private void LoadTimelines()
        => LoadThreePartTimelines(this, BackgroundNode, ImageNode, new Vector2(8.0f, 8.0f));
}
