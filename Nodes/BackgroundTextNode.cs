using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes.Simplified;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// Alternative to <see cref="TextNineGridNode"/>, as TextNineGridNode behaves weirdly often.
/// Only supports centered text.
/// Represents a text node with a background texture to make the text more readable.
/// </summary>
public class BackgroundTextNode : ResNode {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public NineGridNode BackgroundTextureNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public TextNode TextNode { get; set; }

    /// <summary>
    /// Gets or sets the displayed text string.
    /// </summary>
    public ReadOnlySeString String {
        get => TextNode.String;
        set {
            TextNode.String = value;
            TryResizeForContents();
        }
    }

    /// <summary>
    /// Gets or sets the used fontsize.
    /// </summary>
    public uint FontSize {
        get => TextNode.FontSize;
        set {
            TextNode.FontSize = value;
            TryResizeForContents();
        }
    }

    /// <summary>
    /// Gets or sets the used font type.
    /// </summary>
    public FontType FontType {
        get => TextNode.FontType;
        set {
            TextNode.FontType = value;
            TryResizeForContents();
        }
    }

    /// <summary>
    /// Gets or sets the used text flags.
    /// </summary>
    public TextFlags TextFlags {
        get => TextNode.TextFlags;
        set => TextNode.TextFlags = value;
    }

    /// <summary>
    /// Gets or sets the text color.
    /// </summary>
    public Vector4 TextColor
    {
        get => TextNode.TextColor;
        set => TextNode.TextColor = value;
    }

    /// <summary>
    /// Gets or sets the text outline color.
    /// </summary>
    public Vector4 TextOutlineColor
    {
        get => TextNode.TextOutlineColor;
        set => TextNode.TextOutlineColor = value;
    }

    /// <summary>
    /// Gets or sets the background visibility.
    /// </summary>
    public bool ShowBackground {
        get => BackgroundTextureNode.IsVisible;
        set => BackgroundTextureNode.IsVisible = value;
    }

    /// <summary>
    /// Gets or sets the background color.
    /// </summary>
    public Vector4 BackgroundColor {
        get => new(
            BackgroundTextureNode.AddColor.X,
            BackgroundTextureNode.AddColor.Y,
            BackgroundTextureNode.AddColor.Z,
            BackgroundTextureNode.Alpha
        );
        set {
            BackgroundTextureNode.Color = new Vector4(0.0f, 0.0f, 0.0f, value.W);
            BackgroundTextureNode.AddColor = value.AsVector3Color();
        }
    }

    /// <summary>
    /// Gets or sets whether the node will be resized to fit the string when using <see cref="String"/>.
    /// </summary>
    public bool FitToString { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="BackgroundTextNode"/>
    /// </summary>
    public BackgroundTextNode() {
        BackgroundTextureNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/ToolTipS.tex",
            TextureCoordinates = new Vector2(0.0f, 0.0f),
            TextureSize = new Vector2(32.0f, 24.0f),
            TopOffset = 10,
            BottomOffset = 10,
            LeftOffset = 15,
            RightOffset = 15,
        };
        BackgroundTextureNode.AttachNode(this);

        TextNode = new TextNode {
            AlignmentType = AlignmentType.Center,
            TextFlags = TextFlags.Edge,
            TextOutlineColor = ColorHelper.GetColor(55),
            FontSize = 23,
            FontType = FontType.TrumpGothic,
        };
        TextNode.AttachNode(this);
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        BackgroundTextureNode.Size = Size;

        TextNode.Size = Size - new Vector2(20.0f, 0.0f);
        TextNode.Position = new Vector2(10.0f, 0.0f);
    }

    private void TryResizeForContents() {
        if (FitToString) {
            Size = TextNode.GetTextDrawSize() + new Vector2(20.0f, 0.0f);
        }
    }
}
