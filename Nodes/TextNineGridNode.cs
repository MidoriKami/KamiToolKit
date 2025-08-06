using System.Numerics;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public unsafe class TextNineGridNode : ComponentNode<AtkComponentTextNineGrid, AtkUldComponentDataTextNineGrid> {

    public readonly NineGridNode BackgroundNineGrid;
    public readonly TextNode TextNode;

    public TextNineGridNode() {
        SetInternalComponentType(ComponentType.TextNineGrid);

        BackgroundNineGrid = new SimpleNineGridNode {
            NodeId = 3,
            TexturePath = "ui/uld/ToolTipS.tex",
            TextureCoordinates = new Vector2(0.0f, 0.0f),
            TextureSize = new Vector2(32.0f, 24.0f),
            TopOffset = 10,
            BottomOffset = 10,
            LeftOffset = 15,
            RightOffset = 15,
            PartsRenderType = 148,
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.Fill | NodeFlags.EmitsEvents,
        };
        BackgroundNineGrid.AttachNode(this);

        TextNode = new TextNode {
            NodeId = 2,
            TextOutlineColor = ColorHelper.GetColor(55),
            Position = new Vector2(4.0f, 1.0f),
            FontSize = 23,
            AlignmentType = AlignmentType.Right,
            FontType = FontType.TrumpGothic,
            TextFlags = TextFlags.Edge,
            NodeFlags = NodeFlags.AnchorLeft | NodeFlags.AnchorRight | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
        };
        TextNode.AttachNode(this);

        Data->Nodes[0] = TextNode.NodeId;
        Data->Nodes[1] = 0;

        InitializeComponentEvents();
    }

    public SeString Label {
        get => TextNode.String;
        set => Component->SetText(value.ToString());
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

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        BackgroundNineGrid.Size = Size;
        TextNode.Size = Size - new Vector2(8.0f, 2.0f);
    }
}
