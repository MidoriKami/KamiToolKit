using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes.ComponentNode;
using KamiToolKit.Classes;
using KamiToolKit.Interfaces;
using KamiToolKit.Nodes.Simplified;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// Implementation of the game's TextNineGridNode and associated component.
/// </summary>
public unsafe class TextNineGridNode : ComponentNode<AtkComponentTextNineGrid, AtkUldComponentDataTextNineGrid>, IBackgroundTextNode {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public NineGridNode BackgroundNineGrid { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public TextNode TextNode { get; }

    /// <inheritdoc />
    public ReadOnlySeString String {
        get => TextNode.String;
        set => Component->SetText(value);
    }

    /// <inheritdoc />
    public int Number {
        get => int.Parse(TextNode.String);
        set => TextNode.String = value.ToString();
    }

    /// <inheritdoc />
    public uint FontSize {
        get => TextNode.FontSize;
        set => TextNode.FontSize = value;
    }

    /// <inheritdoc />
    public FontType FontType {
        get => TextNode.FontType;
        set => TextNode.FontType = value;
    }

    /// <inheritdoc />
    public Vector4 TextOutlineColor {
        get => TextNode.TextOutlineColor;
        set => TextNode.TextOutlineColor = value;
    }

    /// <inheritdoc />
    public Vector4 TextColor {
        get => TextNode.TextColor;
        set => TextNode.TextColor = value;
    }

    /// <inheritdoc />
    public TextFlags TextFlags {
        get => TextNode.TextFlags;
        set => TextNode.TextFlags = value;
    }

    /// <inheritdoc />
    public AlignmentType AlignmentType {
        get => TextNode.AlignmentType;
        set => TextNode.AlignmentType = value;
    }

    /// <inheritdoc />
    public bool ShowBackground {
        get => BackgroundNineGrid.IsVisible;
        set => BackgroundNineGrid.IsVisible = value;
    }

    /// <inheritdoc />
    public Vector4 BackgroundColor {
        get => new(
            BackgroundNineGrid.AddColor.X,
            BackgroundNineGrid.AddColor.Y,
            BackgroundNineGrid.AddColor.Z,
            BackgroundNineGrid.Alpha
        );
        set {
            BackgroundNineGrid.Color = new Vector4(0.0f, 0.0f, 0.0f, value.W);
            BackgroundNineGrid.AddColor = value.AsVector3Color();
        }
    }

    /// <inheritdoc />
    public bool FitToString { get; set; }

    /// <summary>
    /// Constructs a new <see cref="TextNineGridNode"/>
    /// </summary>
    public TextNineGridNode() {
        SetInternalComponentType(ComponentType.TextNineGrid);

        BackgroundNineGrid = new SimpleNineGridNode {
            TexturePath = "ui/uld/ToolTipS.tex",
            TextureCoordinates = new Vector2(0.0f, 0.0f),
            TextureSize = new Vector2(32.0f, 24.0f),
            TopOffset = 10,
            BottomOffset = 10,
            LeftOffset = 15,
            RightOffset = 15,
        };
        BackgroundNineGrid.AttachNode(this);

        TextNode = new TextNode {
            TextOutlineColor = ColorHelper.GetColor(55),
            Position = new Vector2(4.0f, 1.0f),
            FontSize = 23,
            AlignmentType = AlignmentType.Right,
            FontType = FontType.TrumpGothic,
            TextFlags = TextFlags.Edge,
        };
        TextNode.AttachNode(this);

        Data->Nodes[0] = TextNode.NodeId;
        Data->Nodes[1] = 0;

        InitializeComponentEvents();

        // Disable ParentNode else SetText
        // causes this node to resize itself incorrectly.
        Component->ParentNode = null;
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        BackgroundNineGrid.Size = Size;
        TextNode.Size = Size - new Vector2(8.0f, 2.0f);
    }
}
