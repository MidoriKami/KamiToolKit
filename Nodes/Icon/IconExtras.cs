using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Classes.TimelineBuilding;
using KamiToolKit.NodeParts;

namespace KamiToolKit.Nodes;

public class IconExtras : ResNode {

    public readonly AlternateCooldownNode AlternateCooldownNode;
    public readonly AntsNode AntsNode;
    public readonly ImageNode ChargeCountImageNode;
    public readonly ImageNode ClickFlashImageNode;
    public readonly CooldownNode CooldownNode;
    public readonly ImageNode HoveredBorderImageNode;
    public readonly TextNode QuantityTextNode;
    public readonly TextNode ResourceCostTextNode;

    public readonly ImageNode TimelineImageNode;

    public IconExtras() {
        TimelineImageNode = new SimpleImageNode {
            NodeId = 19,
            Size = new Vector2(40.0f, 40.0f),
            Position = new Vector2(4.0f, 4.0f),
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            WrapMode = 1,
            DrawFlags = 0x102,
            ImageNodeFlags = ImageNodeFlags.AutoFit,
        };

        TimelineImageNode.AttachNode(this);

        CooldownNode = new CooldownNode {
            NodeId = 16, Size = new Vector2(48.0f, 48.0f), NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
        };

        CooldownNode.AttachNode(this);

        AlternateCooldownNode = new AlternateCooldownNode {
            NodeId = 14, Size = new Vector2(44.0f, 48.0f), Position = new Vector2(2.0f, 0.0f), NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
        };

        AlternateCooldownNode.AttachNode(this);

        AntsNode = new AntsNode {
            NodeId = 12, Size = new Vector2(48.0f, 48.0f), NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
        };

        AntsNode.AttachNode(this);

        HoveredBorderImageNode = new ImageNode {
            NodeId = 11,
            Size = new Vector2(72.0f, 72.0f),
            Position = new Vector2(-12.0f, -12.0f),
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            PartId = 16,
            WrapMode = 1,
            ImageNodeFlags = 0,
            DrawFlags = 0x02,
        };

        IconNodeTextureHelper.LoadIconAFrameTexture(HoveredBorderImageNode);

        HoveredBorderImageNode.AttachNode(this);

        ChargeCountImageNode = new ImageNode {
            NodeId = 10,
            Size = new Vector2(20.0f, 20.0f),
            Position = new Vector2(28.0f, 28.0f),
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            WrapMode = 1,
            ImageNodeFlags = 0,
        };

        foreach (var yIndex in Enumerable.Range(0, 2))
        foreach (var xIndex in Enumerable.Range(0, 5)) {
            var coordinate = new Vector2(xIndex * 20.0f, yIndex * 20.0f);
            ChargeCountImageNode.AddPart(new Part {
                TexturePath = "ui/uld/IconA_ChargeIcon.tex", TextureCoordinates = coordinate, Size = new Vector2(20.0f, 20.0f), Id = (uint)(xIndex + yIndex),
            });
        }

        ChargeCountImageNode.AttachNode(this);

        QuantityTextNode = new TextNode {
            NodeId = 9,
            Size = new Vector2(40.0f, 12.0f),
            Position = new Vector2(4.0f, 34.0f),
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            Color = ColorHelper.GetColor(50),
            TextOutlineColor = ColorHelper.GetColor(51),
            AlignmentType = AlignmentType.Right,
            DrawFlags = 0x102,
        };

        QuantityTextNode.AttachNode(this);

        // Also cooldown time text for non-globals
        ResourceCostTextNode = new TextNode {
            NodeId = 8,
            Size = new Vector2(48.0f, 12.0f),
            Position = new Vector2(3.0f, 37.0f),
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            Color = ColorHelper.GetColor(50),
            TextOutlineColor = ColorHelper.GetColor(51),
            AlignmentType = AlignmentType.Left,
        };

        ResourceCostTextNode.AttachNode(this);

        ClickFlashImageNode = new ImageNode {
            NodeId = 7,
            Size = new Vector2(64, 64),
            Position = new Vector2(-8.0f, -8.0f),
            Origin = new Vector2(32.0f, 32.0f),
            DrawFlags = 4,
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            WrapMode = 1,
            ImageNodeFlags = 0,
            PartId = 17,
        };

        IconNodeTextureHelper.LoadIconAFrameTexture(ClickFlashImageNode);

        ClickFlashImageNode.AttachNode(this);

        BuildTimelines();
    }

    private void BuildTimelines() {
        TimelineImageNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(10, 19)
            .AddFrame(10, alpha: 0, multiplyColor: new Vector3(100.0f), addColor: new Vector3(255.0f))
            .AddFrame(12, alpha: 63, multiplyColor: new Vector3(100.0f), addColor: new Vector3(255.0f))
            .EndFrameSet()
            .BeginFrameSet(20, 29)
            .AddFrame(20, alpha: 63, multiplyColor: new Vector3(100.0f), addColor: new Vector3(255.0f))
            .EndFrameSet()
            .BeginFrameSet(40, 49)
            .AddFrame(40, alpha: 63, multiplyColor: new Vector3(100.0f), addColor: new Vector3(255.0f))
            .EndFrameSet()
            .BeginFrameSet(50, 59)
            .AddFrame(50, alpha: 63, multiplyColor: new Vector3(100.0f), addColor: new Vector3(255.0f))
            .AddFrame(52, alpha: 0, multiplyColor: new Vector3(100.0f), addColor: new Vector3(255.0f))
            .EndFrameSet()
            .Build());

        CooldownNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 165)
            .AddLabel(1, 19, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(11, 20, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(21, 21, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(31, 22, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(41, 101, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(51, 102, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabelPair(61, 142, 24)
            .AddLabelPair(143, 165, 25)
            .EndFrameSet()
            .AddFrameSetWithFrame(1, 9, 1, multiplyColor: new Vector3(100.0f), addColor: new Vector3(0.0f))
            .BeginFrameSet(10, 19)
            .AddFrame(10, multiplyColor: new Vector3(100.0f), addColor: new Vector3(0.0f))
            .AddFrame(12, multiplyColor: new Vector3(100.0f), addColor: new Vector3(16.0f))
            .EndFrameSet()
            .AddFrameSetWithFrame(20, 29, 20, multiplyColor: new Vector3(100.0f), addColor: new Vector3(16.0f))
            .AddFrameSetWithFrame(30, 39, 30, multiplyColor: new Vector3(100.0f), addColor: new Vector3(0.0f))
            .AddFrameSetWithFrame(40, 49, 40, multiplyColor: new Vector3(100.0f), addColor: new Vector3(16.0f))
            .BeginFrameSet(50, 59)
            .AddFrame(50, multiplyColor: new Vector3(100.0f), addColor: new Vector3(16.0f))
            .AddFrame(52, multiplyColor: new Vector3(100.0f), addColor: new Vector3(0.0f))
            .EndFrameSet()
            .Build());

        AlternateCooldownNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 205)
            .AddLabel(1, 17, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(11, 101, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(92, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(93, 102, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(174, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(175, 103, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(205, 0, AtkTimelineJumpBehavior.LoopForever, 103)
            .EndFrameSet()
            .Build());

        AntsNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 9)
            .AddLabel(1, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(2, 26, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(9, 0, AtkTimelineJumpBehavior.LoopForever, 26)
            .EndFrameSet()
            .Build());

        HoveredBorderImageNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(10, 19)
            .AddFrame(10, alpha: 0, multiplyColor: new Vector3(100.0f), addColor: new Vector3(0.0f))
            .AddFrame(12, alpha: 255, multiplyColor: new Vector3(100.0f), addColor: new Vector3(0.0f))
            .EndFrameSet()
            .BeginFrameSet(20, 29)
            .AddFrame(20, alpha: 255, multiplyColor: new Vector3(100.0f), addColor: new Vector3(0.0f))
            .EndFrameSet()
            .BeginFrameSet(40, 49)
            .AddFrame(40, alpha: 255, multiplyColor: new Vector3(100.0f), addColor: new Vector3(0.0f))
            .EndFrameSet()
            .BeginFrameSet(50, 59)
            .AddFrame(50, alpha: 255, multiplyColor: new Vector3(100.0f), addColor: new Vector3(0.0f))
            .AddFrame(52, alpha: 0, multiplyColor: new Vector3(100.0f), addColor: new Vector3(0.0f))
            .EndFrameSet()
            .Build());

        ClickFlashImageNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(20, 29)
            .AddFrame(20, alpha: 255, scale: new Vector2(0.1f))
            .AddFrame(29, alpha: 0, scale: new Vector2(1.0f))
            .EndFrameSet()
            .Build());
    }
}
