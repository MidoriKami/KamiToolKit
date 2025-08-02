using System.Numerics;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Nodes;

public unsafe class TextButtonNode : ButtonBase {

    public readonly NineGridNode BackgroundNode;
    public readonly TextNode LabelNode;

    public TextButtonNode() {
        Data->Nodes[0] = 3;
        Data->Nodes[1] = 2;

        BackgroundNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/ButtonA.tex",
            TextureSize = new Vector2(100.0f, 28.0f),
            LeftOffset = 16.0f,
            RightOffset = 16.0f,
            NodeId = 2,
        };

        BackgroundNode.AttachNode(this);

        LabelNode = new TextNode {
            AlignmentType = AlignmentType.Center, Position = new Vector2(16.0f, 3.0f), NodeId = 3,
        };

        LabelNode.AttachNode(this);

        LoadTimelines();

        InitializeComponentEvents();
    }

    public SeString Label {
        get => LabelNode.Text;
        set => LabelNode.Text = value;
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        LabelNode.Size = new Vector2(Width - 32.0f, Height - 8.0f);
        BackgroundNode.Size = Size;
    }

    private void LoadTimelines()
        => LoadThreePartTimelines(this, BackgroundNode, LabelNode, new Vector2(16.0f, 3.0f));
}
