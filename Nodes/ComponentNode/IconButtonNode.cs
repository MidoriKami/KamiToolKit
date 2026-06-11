using System.Numerics;
using KamiToolKit.Nodes.Simplified;

namespace KamiToolKit.Nodes;

/// <summary>
/// Specialization of a button representing a clickable game icon via IconId.
/// </summary>
/// <remarks>
/// Uses a GameIconId to display that icon as the decorator for the button.
/// </remarks>
public class IconButtonNode : ButtonBase {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public NineGridNode BackgroundNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public IconImageNode ImageNode { get; }

    /// <summary>
    /// Gets or sets the iconId used for the displayed icon.
    /// </summary>
    public uint IconId {
        get => ImageNode.IconId;
        set => ImageNode.IconId = value;
    }

    public IconButtonNode() {
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

        ImageNode = new IconImageNode {
            TextureSize = new Vector2(32.0f, 32.0f),
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
