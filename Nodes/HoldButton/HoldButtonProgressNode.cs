using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.TimelineBuilding;

namespace KamiToolKit.Nodes;

public class HoldButtonProgressNode : ResNode {

    public readonly ImageNode ImageNode;

    public HoldButtonProgressNode() {
        ImageNode = new SimpleImageNode {
            NodeId = 5,
            TexturePath = "ui/uld/LongPressButtonA.tex",
            TextureCoordinates = new Vector2(0.0f, 36.0f),
            TextureSize = new Vector2(100.0f, 36.0f),
            Size = new Vector2(0.0f, 36.0f),
            WrapMode = 1,
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            ImageNodeFlags = 0,
        };
        ImageNode.AttachNode(this);

        BuildTimelines();
    }

    private void BuildTimelines() {
        AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 83)
            .AddLabel(1, 29, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(60, 30, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(61, 31, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(73, 32, AtkTimelineJumpBehavior.PlayOnce, 31)
            .AddLabel(74, 33, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(83, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .EndFrameSet()
            .BeginFrameSet(18, 26)
            .AddEmptyFrame(18)
            .EndFrameSet()
            .BeginFrameSet(37, 53)
            .AddEmptyFrame(37)
            .EndFrameSet()
            .BeginFrameSet(54, 71)
            .AddEmptyFrame(54)
            .EndFrameSet()
            .Build()
        );

        ImageNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 60)
            .AddFrame(1, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(61, 73)
            .AddFrame(61, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(74, 83)
            .AddFrame(74, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(76, addColor: new Vector3(150, 150, 100), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(83, addColor: new Vector3(20, 20, 20), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .Build()
        );
    }
}
