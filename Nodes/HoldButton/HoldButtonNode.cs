using System;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Addon.Events.EventDataTypes;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.TimelineBuilding;

namespace KamiToolKit.Nodes;

public unsafe class HoldButtonNode : ComponentNode<AtkComponentHoldButton, AtkUldComponentDataHoldButton> {

    public readonly NineGridNode BackgroundNode;
    public readonly NineGridNode FrameNode;
    public readonly HoldButtonProgressNode ProgressNode;
    public readonly TextNode TextNode;

    public HoldButtonNode() {
        SetInternalComponentType(ComponentType.HoldButton);

        BackgroundNode = new SimpleNineGridNode {
            NodeId = 6,
            TexturePath = "ui/uld/LongPressButtonA.tex",
            TextureCoordinates = new Vector2(0.0f, 0.0f),
            TextureSize = new Vector2(100.0f, 36.0f),
            Size = new Vector2(100.0f, 36.0f),
            LeftOffset = 16,
            RightOffset = 16,
            NodeFlags = NodeFlags.AnchorLeft | NodeFlags.AnchorRight | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
        };
        BackgroundNode.AttachNode(this);

        ProgressNode = new HoldButtonProgressNode {
            NodeId = 4, Size = new Vector2(100.0f, 36.0f), NodeFlags = NodeFlags.AnchorLeft | NodeFlags.AnchorRight | NodeFlags.Enabled | NodeFlags.Visible | NodeFlags.EmitsEvents,
        };
        ProgressNode.AttachNode(this);

        FrameNode = new SimpleNineGridNode {
            NodeId = 3,
            TexturePath = "ui/uld/LongPressButtonA.tex",
            TextureCoordinates = new Vector2(0.0f, 72.0f),
            TextureSize = new Vector2(100.0f, 36.0f),
            Size = new Vector2(100.0f, 36.0f),
            NodeFlags = NodeFlags.AnchorLeft | NodeFlags.AnchorRight | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
        };
        FrameNode.AttachNode(this);

        TextNode = new TextNode {
            NodeId = 2,
            Position = new Vector2(16.0f, 8.0f),
            Size = new Vector2(68.0f, 20.0f),
            AlignmentType = AlignmentType.Center,
            String = "OK",
            NodeFlags = NodeFlags.AnchorLeft | NodeFlags.AnchorRight | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
        };
        TextNode.AttachNode(this);

        Data->Nodes[0] = TextNode.NodeId;
        Data->Nodes[1] = BackgroundNode.NodeId;
        Data->Nodes[2] = ProgressNode.NodeId;
        Data->Nodes[3] = ProgressNode.ImageNode.NodeId;

        InitializeComponentEvents();

        AddEvent(AddonEventType.ButtonClick, ClickHandler);

        BuildTimelines();
    }

    public bool UnlockAfterClick { get; set; }

    public Action? OnClick { get; set; }

    public SeString Label {
        get => TextNode.Text;
        set => TextNode.Text = value;
    }

    private void ClickHandler(AddonEventData obj) {
        OnClick?.Invoke();

        if (UnlockAfterClick) {
            Reset();
        }
    }

    public void Reset() {
        Component->IsTargetReached = false;
        Component->IsEventFired = false;
        Component->Progress.StartValue = 0;
        Component->Progress.TargetValue = 0;
        Component->Progress.CurrentValue = 0;
        Component->Progress.EndValue = 0;
    }

    private void BuildTimelines() {
        AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 20)
            .AddLabel(1, 17, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(10, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(11, 101, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(20, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .EndFrameSet()
            .Build()
        );

        BackgroundNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 10)
            .AddFrame(1, new Vector2(0, 0))
            .AddFrame(1, alpha: 255)
            .AddFrame(1, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(11, 17)
            .AddFrame(11, new Vector2(0, 0))
            .AddFrame(13, new Vector2(0, 0))
            .AddFrame(11, alpha: 255)
            .AddFrame(13, alpha: 255)
            .AddFrame(11, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(13, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(18, 26)
            .AddFrame(18, new Vector2(0, 1))
            .AddFrame(18, alpha: 255)
            .AddFrame(18, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(27, 36)
            .AddFrame(27, new Vector2(0, 0))
            .AddFrame(27, alpha: 178)
            .AddFrame(27, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(50, 50, 50))
            .EndFrameSet()
            .BeginFrameSet(37, 46)
            .AddFrame(37, new Vector2(0, 0))
            .AddFrame(37, alpha: 255)
            .AddFrame(37, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(47, 53)
            .AddFrame(47, new Vector2(0, 0))
            .AddFrame(53, new Vector2(0, 0))
            .AddFrame(47, alpha: 255)
            .AddFrame(53, alpha: 255)
            .AddFrame(47, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(53, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(54, 64)
            .AddFrame(54, new Vector2(0, 0))
            .AddFrame(54, alpha: 255)
            .AddFrame(54, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(65, 71)
            .AddFrame(65, new Vector2(0, 0))
            .AddFrame(71, new Vector2(0, 0))
            .AddFrame(65, alpha: 255)
            .AddFrame(71, alpha: 255)
            .AddFrame(65, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(71, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .Build()
        );

        ProgressNode.AddTimeline(new TimelineBuilder()
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

        FrameNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 10)
            .AddFrame(1, new Vector2(0, 0))
            .AddFrame(1, alpha: 255)
            .AddFrame(1, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(11, 17)
            .AddFrame(11, new Vector2(0, 0))
            .AddFrame(13, new Vector2(0, 0))
            .AddFrame(11, alpha: 255)
            .AddFrame(13, alpha: 255)
            .AddFrame(11, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(13, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(95, 95, 95))
            .EndFrameSet()
            .BeginFrameSet(18, 26)
            .AddFrame(18, new Vector2(0, 0))
            .AddFrame(18, alpha: 255)
            .AddFrame(18, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(95, 95, 95))
            .EndFrameSet()
            .BeginFrameSet(27, 36)
            .AddFrame(27, new Vector2(0, 0))
            .AddFrame(27, alpha: 178)
            .AddFrame(27, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(50, 50, 50))
            .EndFrameSet()
            .BeginFrameSet(37, 46)
            .AddFrame(37, new Vector2(0, 0))
            .AddFrame(37, alpha: 255)
            .AddFrame(37, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(95, 95, 95))
            .EndFrameSet()
            .BeginFrameSet(47, 53)
            .AddFrame(47, new Vector2(0, 0))
            .AddFrame(53, new Vector2(0, 0))
            .AddFrame(47, alpha: 255)
            .AddFrame(53, alpha: 255)
            .AddFrame(47, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(95, 95, 95))
            .AddFrame(53, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(54, 64)
            .AddFrame(54, new Vector2(0, 0))
            .AddFrame(54, alpha: 255)
            .AddFrame(54, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(95, 95, 95))
            .EndFrameSet()
            .BeginFrameSet(65, 71)
            .AddFrame(65, new Vector2(0, 0))
            .AddFrame(71, new Vector2(0, 0))
            .AddFrame(65, alpha: 255)
            .AddFrame(71, alpha: 255)
            .AddFrame(65, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(95, 95, 95))
            .AddFrame(71, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .Build()
        );

        TextNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 10)
            .AddFrame(1, new Vector2(16, 8))
            .AddFrame(1, alpha: 255)
            .AddFrame(1, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(11, 17)
            .AddFrame(11, new Vector2(16, 8))
            .AddFrame(11, alpha: 255)
            .AddFrame(11, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(18, 26)
            .AddFrame(18, new Vector2(16, 9))
            .AddFrame(18, alpha: 255)
            .AddFrame(18, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(27, 36)
            .AddFrame(27, new Vector2(16, 8))
            .AddFrame(27, alpha: 153)
            .AddFrame(27, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(80, 80, 80))
            .EndFrameSet()
            .BeginFrameSet(37, 46)
            .AddFrame(37, new Vector2(16, 8))
            .AddFrame(37, alpha: 255)
            .AddFrame(37, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(47, 53)
            .AddFrame(47, new Vector2(16, 8))
            .AddFrame(47, alpha: 255)
            .AddFrame(47, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(54, 64)
            .AddFrame(54, new Vector2(16, 8))
            .AddFrame(54, alpha: 255)
            .AddFrame(54, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(65, 71)
            .AddFrame(65, new Vector2(16, 8))
            .AddFrame(65, alpha: 255)
            .AddFrame(65, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .Build()
        );
    }
}
