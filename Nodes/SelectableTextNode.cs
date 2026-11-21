using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Interfaces;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// Further specialization of <see cref="SelectableNode"/> implementing a string display.
/// </summary>
public class SelectableTextNode : SelectableNode, ISelectableTextNode {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public TextNode TextNode { get; }

    /// <inheritdoc />
    public ReadOnlySeString String {
        get => TextNode.String;
        set => TextNode.String = value;
    }

    /// <inheritdoc />
    public Vector4 TextColor {
        get => TextNode.TextColor;
        set => TextNode.TextColor = value;
    }

    /// <inheritdoc />
    public Vector4 TextOutlineColor {
        get => TextNode.TextOutlineColor;
        set => TextNode.TextOutlineColor = value;
    }

    /// <inheritdoc />
    public AlignmentType AlignmentType {
        get => TextNode.AlignmentType;
        set => TextNode.AlignmentType = value;
    }

    /// <inheritdoc />
    public FontType FontType {
        get => TextNode.FontType;
        set => TextNode.FontType = value;
    }

    /// <inheritdoc />
    public TextFlags TextFlags {
        get => TextNode.TextFlags;
        set => TextNode.TextFlags = value;
    }

    /// <inheritdoc />
    public uint FontSize {
        get => TextNode.FontSize;
        set => TextNode.FontSize = value;
    }

    /// <summary>
    /// Constructs a new <see cref="SelectableTextNode"/>
    /// </summary>
    public SelectableTextNode() {
        TextNode = new TextNode();
        TextNode.AttachNode(this);
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        TextNode.Size = Size;
        TextNode.Position = new Vector2(6.0f, 0.0f);
    }
}