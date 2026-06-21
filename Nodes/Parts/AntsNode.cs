using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Enums;
using KamiToolKit.Timelines;

namespace KamiToolKit.Nodes;

/// <summary>
/// Node part used for <see cref="IconNode"/>, not intended for external use.
/// </summary>
public class AntsNode : ResNode {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ImageNode AntsImageNode { get; }

    /// <summary>
    /// Constructs a new <see cref="AntsNode"/>
    /// </summary>
    public AntsNode() {
        AntsImageNode = new ImageNode {
            NodeId = 13,
            Size = new Vector2(48, 48),
            NodeFlags = NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            WrapMode = WrapMode.Tile,
            PartId = 13,
        };

        IconNodeTextureHelper.LoadIconAFrameTexture(AntsImageNode);

        AntsImageNode.AttachNode(this);

        BuildTimeline();
    }

    private void BuildTimeline() => AntsImageNode.AddTimeline(new TimelineBuilder()
        .BeginFrameSet(2, 9)
        .AddFrame(2, partId: 6)
        .AddFrame(9, partId: 13)
        .EndFrameSet()
        .Build()
    );
}
