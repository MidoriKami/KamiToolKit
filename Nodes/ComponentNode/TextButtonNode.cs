using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Interfaces;
using KamiToolKit.Nodes.Simplified;
using Lumina.Data.Parsing.Uld;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// Specialization of a button representing a standard text button.
/// </summary>
public unsafe class TextButtonNode : ButtonBase, ITextButtonNode {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public NineGridNode BackgroundNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public TextNode LabelNode { get; }

    /// <inheritdoc/>
    public Vector4 TextColor {
        get => LabelNode.TextColor;
        set => LabelNode.TextColor = value;
    }

    /// <inheritdoc/>
    public Vector4 TextOutlineColor {
        get => LabelNode.TextOutlineColor;
        set => LabelNode.TextOutlineColor = value;
    }

    /// <inheritdoc/>
    public ReadOnlySeString String {
        get => LabelNode.String;
        set => LabelNode.String = value;
    }

    /// <inheritdoc/>
    public AlignmentType AlignmentType {
        get => LabelNode.AlignmentType;
        set => LabelNode.AlignmentType = value;
    }

    /// <inheritdoc/>
    public FontType FontType {
        get => LabelNode.FontType;
        set => LabelNode.FontType = value;
    }

    /// <inheritdoc/>
    public TextFlags TextFlags {
        get => LabelNode.TextFlags;
        set => LabelNode.TextFlags = value;
    }

    /// <inheritdoc/>
    public uint FontSize {
        get => LabelNode.FontSize;
        set => LabelNode.FontSize = value;
    }

    /// <inheritdoc/>
    public uint TextId {
        get => LabelNode.TextId;
        set => LabelNode.TextId = value;
    }

    /// <inheritdoc/>
    public NodeData.SheetType SheetType {
        get => LabelNode.SheetType;
        set => LabelNode.SheetType = value;
    }

    /// <summary>
    /// Constructs a new <see cref="TextButtonNode"/>
    /// </summary>
    public TextButtonNode() {
        BackgroundNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/ButtonA.tex",
            TextureSize = new Vector2(100.0f, 28.0f),
            LeftOffset = 16.0f,
            RightOffset = 16.0f,
        };
        BackgroundNode.AttachNode(this);

        LabelNode = new TextNode {
            AlignmentType = AlignmentType.Center,
            Position = new Vector2(16.0f, 3.0f),
            TextColor = ColorHelper.GetColor(50),
        };
        LabelNode.AttachNode(this);

        LoadTimelines();

        Data->Nodes[0] = LabelNode.NodeId;
        Data->Nodes[1] = BackgroundNode.NodeId;

        InitializeComponentEvents();
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        LabelNode.Size = new Vector2(Width - 32.0f, Height - 8.0f);
        BackgroundNode.Size = Size;
    }

    private void LoadTimelines()
        => LoadThreePartTimelines(this, BackgroundNode, LabelNode, new Vector2(16.0f, 3.0f));
}
