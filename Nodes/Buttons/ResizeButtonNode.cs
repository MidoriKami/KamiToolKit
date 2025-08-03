using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Classes.TimelineBuilding;

namespace KamiToolKit.Nodes;

// Not intended for public use, this is specialized for KamiToolKit.NodeBase.Resize
internal class ResizeButtonNode : SimpleComponentNode {

    private readonly ResizeDirection resizeDirection;

    public readonly ImageNode SelectedImageNode;
    public readonly ImageNode UnselectedImageNode;

    public ResizeButtonNode(ResizeDirection direction) {
        resizeDirection = direction;

        var rotation = direction switch {
            ResizeDirection.BottomRight => 1.0f * MathF.PI / 2.0f,
            ResizeDirection.BottomLeft => 2.0f * MathF.PI / 2.0f,
            _ => 0.0f,
        };

        UnselectedImageNode = new SimpleImageNode {
            NodeId = 3,
            TexturePath = "ui/uld/ChatLog.tex",
            TextureCoordinates = new Vector2(28.0f, 28.0f),
            TextureSize = new Vector2(28.0f, 28.0f),
            Size = new Vector2(28.0f, 28.0f),
            Origin = new Vector2(14.0f, 14.0f),
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            WrapMode = 1,
            ImageNodeFlags = 0,
        };
        UnselectedImageNode.AttachNode(this);

        SelectedImageNode = new SimpleImageNode {
            NodeId = 2,
            TexturePath = "ui/uld/ChatLog.tex",
            TextureCoordinates = new Vector2(0.0f, 28.0f),
            TextureSize = new Vector2(28.0f, 28.0f),
            Size = new Vector2(28.0f, 28.0f),
            Origin = new Vector2(14.0f, 14.0f),
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            WrapMode = 1,
            ImageNodeFlags = 0,
        };
        SelectedImageNode.AttachNode(this);

        AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 30)
            .AddLabel(1, 1, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(30, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .EndFrameSet()
            .Build());

        UnselectedImageNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 30)
            .AddFrame(1, rotation: rotation)
            .EndFrameSet()
            .Build()
        );

        SelectedImageNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 30)
            .AddFrame(1, rotation: rotation)
            .EndFrameSet()
            .Build()
        );

        Timeline?.PlayAnimation(1);
    }

    public bool IsHovered {
        get;
        set {
            field = value;
            UnselectedImageNode.IsVisible = !value;
            SelectedImageNode.IsVisible = value;
        }
    }
}
