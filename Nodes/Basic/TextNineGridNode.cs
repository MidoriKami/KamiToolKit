using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

public unsafe class TextNineGridNode : ComponentNode<AtkComponentTextNineGrid, AtkUldComponentDataTextNineGrid> {

    public readonly NineGridNode BackgroundNineGrid;
    public readonly TextNode TextNode;

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

    public ReadOnlySeString String {
        get => TextNode.String;
        set => Component->SetText(value);
    }

    public int Number {
        get => int.Parse(TextNode.String);
        set => TextNode.String = value.ToString();
    }

    public int FontSize {
        get => (int)TextNode.FontSize;
        set => TextNode.FontSize = (uint)value;
    }

    public FontType FontType {
        get => TextNode.FontType;
        set => TextNode.FontType = value;
    }

    public Vector4 TextOutlineColor {
        get => TextNode.TextOutlineColor;
        set => TextNode.TextOutlineColor = value;
    }

    public Vector4 TextColor {
        get => TextNode.TextColor;
        set => TextNode.TextColor = value;
    }

    public TextFlags TextFlags {
        get => TextNode.TextFlags;
        set => TextNode.TextFlags = value;
    }

    public AlignmentType AlignmentType {
        get => TextNode.AlignmentType;
        set => TextNode.AlignmentType = value;
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        BackgroundNineGrid.Size = Size;
        TextNode.Size = Size - new Vector2(8.0f, 2.0f);
    }
}
