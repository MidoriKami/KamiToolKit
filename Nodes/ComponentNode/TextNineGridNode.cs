using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes.ComponentNode;
using KamiToolKit.Classes;
using KamiToolKit.Premade.Node.Simple;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// Implementation of the games TextNineGridNode and associated component.
/// </summary>
public unsafe class TextNineGridNode : ComponentNode<AtkComponentTextNineGrid, AtkUldComponentDataTextNineGrid> {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public NineGridNode BackgroundNineGrid { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public TextNode TextNode { get; }

    /// <summary>
    /// Gets or sets the displayed text.
    /// </summary>
    public ReadOnlySeString String {
        get => TextNode.String;
        set => Component->SetText(value);
    }

    /// <summary>
    /// Gets or sets the displayed number.
    /// </summary>
    public int Number {
        get => int.Parse(TextNode.String);
        set => TextNode.String = value.ToString();
    }

    /// <summary>
    /// Gets or sets the font size.
    /// </summary>
    public int FontSize {
        get => (int)TextNode.FontSize;
        set => TextNode.FontSize = (uint)value;
    }

    /// <summary>
    /// Gets or sets the used font.
    /// </summary>
    public FontType FontType {
        get => TextNode.FontType;
        set => TextNode.FontType = value;
    }

    /// <summary>
    /// Gets or sets outline color.
    /// </summary>
    /// <remarks>
    /// Expects values between 0.0f and 1.0f.
    /// </remarks>
    public Vector4 TextOutlineColor {
        get => TextNode.TextOutlineColor;
        set => TextNode.TextOutlineColor = value;
    }

    /// <summary>
    /// Gets or sets text color.
    /// </summary>
    /// <remarks>
    /// Expects values between 0.0f and 1.0f.
    /// </remarks>
    public Vector4 TextColor {
        get => TextNode.TextColor;
        set => TextNode.TextColor = value;
    }

    /// <summary>
    /// Gets or sets the text flags.
    /// </summary>
    public TextFlags TextFlags {
        get => TextNode.TextFlags;
        set => TextNode.TextFlags = value;
    }

    /// <summary>
    /// Gets or sets the text alignment.
    /// </summary>
    public AlignmentType AlignmentType {
        get => TextNode.AlignmentType;
        set => TextNode.AlignmentType = value;
    }

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

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        BackgroundNineGrid.Size = Size;
        TextNode.Size = Size - new Vector2(8.0f, 2.0f);
    }
}
