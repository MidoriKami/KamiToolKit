using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.TimelineBuilding;

namespace KamiToolKit.Nodes;

public class IconIndicator : ResNode {

    public readonly ImageNode IconNode;

    public IconIndicator(uint innerNodeId) {
        IconNode = new ImageNode {
            NodeId = innerNodeId,
            Size = new Vector2(18, 18),
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            WrapMode = 2,
            ImageNodeFlags = 0,
            DrawFlags = 0x02,
            PartId = (uint)(innerNodeId == 5 ? 25 : 30),
        };

        IconNodeTextureHelper.LoadIconAFrameTexture(IconNode);

        IconNode.AttachNode(this);

        BuildTimeline();
    }

    private void BuildTimeline() {
        IconNode.AddTimeline(new TimelineBuilder()
            .AddFrameSetWithFrame(11, 20, 11, partId: 14)
            .AddFrameSetWithFrame(21, 30, 21, partId: 15)
            .AddFrameSetWithFrame(31, 40, 31, partId: 21)
            .AddFrameSetWithFrame(41, 50, 41, partId: 22)
            .AddFrameSetWithFrame(51, 60, 51, partId: 23)
            .AddFrameSetWithFrame(61, 70, 61, partId: 24)
            .AddFrameSetWithFrame(71, 79, 71, partId: 29)
            .AddFrameSetWithFrame(80, 89, 80, partId: 30)
            .AddFrameSetWithFrame(90, 99, 90, partId: 25)
            .AddFrameSetWithFrame(100, 109, 100, partId: 26)
            .AddFrameSetWithFrame(110, 119, 110, partId: 27)
            .AddFrameSetWithFrame(120, 129, 120, partId: 28)
            .Build());
    }
}
