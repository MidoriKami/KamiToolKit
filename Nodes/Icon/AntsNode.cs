using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.TimelineBuilding;

namespace KamiToolKit.Nodes;

public class AntsNode : ResNode {

    public readonly ImageNode AntsImageNode;

    public AntsNode() {
        AntsImageNode = new ImageNode {
            NodeId = 13,
            Size = new Vector2(48, 48),
            NodeFlags = NodeFlags.Enabled | NodeFlags.EmitsEvents,
            WrapMode = 1,
            ImageNodeFlags = 0,
            DrawFlags = 0x02,
            PartId = 13,
        };

        IconNodeTextureHelper.LoadIconAFrameTexture(AntsImageNode);

        AntsImageNode.AttachNode(this);

        BuildTimeline();
    }

    private void BuildTimeline() {
        AntsImageNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(2, 9)
            .AddFrame(2, partId: 6)
            .AddFrame(9, partId: 13)
            .EndFrameSet()
            .Build());
    }
}
