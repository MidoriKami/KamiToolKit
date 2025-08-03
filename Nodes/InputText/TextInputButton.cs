using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.TimelineBuilding;

namespace KamiToolKit.Nodes;

public unsafe class TextInputButtonNode : ButtonBase {

    public readonly NineGridNode BackgroundNode;
    public readonly TextNode LabelNode;

    public TextInputButtonNode() {
        BackgroundNode = new SimpleNineGridNode {
            NodeId = 3,
            Size = new Vector2(160.0f, 24.0f),
            LeftOffset = 16.0f,
            RightOffset = 1.0f,
            NodeFlags = NodeFlags.AnchorLeft | NodeFlags.AnchorRight | NodeFlags.Enabled | NodeFlags.Fill | NodeFlags.EmitsEvents,
            IsVisible = true,
            TexturePath = "ui/uld/ListItemA.tex",
            TextureCoordinates = new Vector2(0.0f, 22.0f),
            TextureSize = new Vector2(63.0f, 22.0f),
        };

        BackgroundNode.AttachNode(this);

        LabelNode = new TextNode {
            NodeId = 2,
            Position = new Vector2(12.0f, 2.0f),
            Size = new Vector2(140.0f, 18.0f),
            NodeFlags = NodeFlags.AnchorLeft | NodeFlags.AnchorRight | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            AlignmentType = AlignmentType.Left,
            TextFlags = TextFlags.AutoAdjustNodeSize,
            TextColor = KnownColor.White.Vector(),
            TextOutlineColor = KnownColor.White.Vector(),
            BackgroundColor = KnownColor.Black.Vector(),
            IsVisible = true,
        };

        LabelNode.AttachNode(this);

        Data->Nodes[0] = LabelNode.NodeId;
        Data->Nodes[1] = BackgroundNode.NodeId;

        LoadTimeline();

        InitializeComponentEvents();
    }

    private void LoadTimeline() {
        AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 59)
            .AddLabelPair(1, 9, 1)
            .AddLabelPair(10, 19, 2)
            .AddLabelPair(20, 29, 3)
            .AddLabelPair(30, 39, 7)
            .AddLabelPair(40, 49, 6)
            .AddLabelPair(50, 59, 4)
            .EndFrameSet()
            .Build());

        BackgroundNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(10, 19)
            .AddFrame(10, alpha: 0)
            .AddFrame(13, alpha: 255)
            .EndFrameSet()
            .AddFrameSetWithFrame(20, 29, 20, alpha: 255)
            .AddFrameSetWithFrame(40, 49, 40, alpha: 255)
            .BeginFrameSet(50, 59)
            .AddFrame(50, alpha: 255)
            .AddFrame(52, alpha: 0)
            .EndFrameSet()
            .Build());

        LabelNode.AddTimeline(new TimelineBuilder()
            .AddFrameSetWithFrame(1, 29, 1, alpha: 255, multiplyColor: new Vector3(100.0f))
            .AddFrameSetWithFrame(30, 39, 30, alpha: 153, multiplyColor: new Vector3(80.0f))
            .AddFrameSetWithFrame(40, 59, 40, alpha: 255, multiplyColor: new Vector3(100.0f))
            .Build());
    }
}
