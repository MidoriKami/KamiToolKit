using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Interfaces;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// A <see cref="TextNode"/> with a <see cref="HorizontalLineNode"/> underneath it for stylish texting.
/// </summary>
public class UnderlinedTextNode : ResNode, IUnderlinedTextNode {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public CategoryTextNode LabelTextNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public HorizontalLineNode LineNode { get; }

    /// <inheritdoc />
    public ReadOnlySeString String {
        get => LabelTextNode.String;
        set {
            LabelTextNode.String = value;
            RecalculateLineSize();
        }
    }

    /// <inheritdoc />
    public Vector4 TextColor {
        get => LabelTextNode.TextColor;
        set => LabelTextNode.TextColor = value;
    }

    /// <inheritdoc />
    public Vector4 TextOutlineColor {
        get => LabelTextNode.TextOutlineColor;
        set => LabelTextNode.TextOutlineColor = value;
    }

    /// <inheritdoc />
    public AlignmentType AlignmentType {
        get => LabelTextNode.AlignmentType;
        set => LabelTextNode.AlignmentType = value;
    }

    /// <inheritdoc />
    public FontType FontType {
        get => LabelTextNode.FontType;
        set => LabelTextNode.FontType = value;
    }

    /// <inheritdoc />
    public TextFlags TextFlags {
        get => LabelTextNode.TextFlags;
        set => LabelTextNode.TextFlags = value;
    }

    /// <inheritdoc />
    public uint FontSize {
        get => LabelTextNode.FontSize;
        set => LabelTextNode.FontSize = value;
    }

    /// <summary>
    /// Constructs a new <see cref="UnderlinedTextNode"/>
    /// </summary>
    public UnderlinedTextNode() {
        LabelTextNode = new CategoryTextNode();
        LabelTextNode.AttachNode(this);

        LineNode = new HorizontalLineNode {
            Height = 4.0f,
        };
        LineNode.AttachNode(this);
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        LabelTextNode.Size = new Vector2(Width, Height - 4.0f);
        LabelTextNode.Position = new Vector2(0.0f, 0.0f);

        LineNode.Position = new Vector2(0.0f, LabelTextNode.Bounds.Bottom - 4.0f);
        RecalculateLineSize();
    }

    private void RecalculateLineSize() {
        var textSize = LabelTextNode.GetTextDrawSize();
        LineNode.Width = textSize.X + 32.0f;
    }
}