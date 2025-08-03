using System.Numerics;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Classes.TimelineBuilding;

namespace KamiToolKit.Nodes;

public unsafe class ListButtonNode : ButtonBase {

    public readonly NineGridNode HoverBackgroundNode;
    public readonly TextNode LabelNode;
    public readonly NineGridNode SelectedBackgroundNode;

    public ListButtonNode() {
        HoverBackgroundNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/ListItemA.tex",
            TextureCoordinates = new Vector2(0.0f, 22.0f),
            TextureSize = new Vector2(64.0f, 22.0f),
            LeftOffset = 16,
            RightOffset = 1,
        };

        HoverBackgroundNode.AttachNode(this);

        SelectedBackgroundNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/ListItemA.tex",
            TextureCoordinates = new Vector2(0.0f, 0.0f),
            TextureSize = new Vector2(64.0f, 22.0f),
            LeftOffset = 16,
            RightOffset = 1,
        };

        SelectedBackgroundNode.AttachNode(this);

        LabelNode = new TextNode {
            Position = new Vector2(10.0f, 1.0f),
            TextColor = ColorHelper.GetColor(8),
            TextOutlineColor = ColorHelper.GetColor(7),
            FontType = FontType.Axis,
            FontSize = 14,
            AlignmentType = AlignmentType.Left,
            IsVisible = true,
            Text = "Label Not Set",
        };

        LabelNode.AttachNode(this);

        LoadTimelines();

        InitializeComponentEvents();
    }

    public bool Selected {
        get => Component->IsChecked;
        set => Component->SetChecked(value);
    }

    public SeString Label {
        get => LabelNode.Text;
        set => LabelNode.Text = value;
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        HoverBackgroundNode.Size = Size;
        SelectedBackgroundNode.Size = Size;
        LabelNode.Size = new Vector2(Width - 10.0f, Height - 1.0f);
    }

    private void LoadTimelines() {
        AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 120)
            .AddLabel(1, 1, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(9, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(10, 2, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(19, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(20, 3, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(29, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(30, 7, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(39, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(40, 6, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(49, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(50, 4, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(59, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(60, 8, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(69, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(70, 9, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(79, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(80, 10, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(89, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(90, 14, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(99, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(100, 13, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(109, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(110, 11, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(120, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .EndFrameSet()
            .Build()
        );

        HoverBackgroundNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 9)
            .AddFrame(1, alpha: 0)
            .EndFrameSet()
            .BeginFrameSet(10, 19)
            .AddFrame(10, alpha: 0)
            .AddFrame(13, alpha: 255)
            .EndFrameSet()
            .BeginFrameSet(20, 29)
            .AddFrame(20, alpha: 255)
            .EndFrameSet()
            .BeginFrameSet(40, 49)
            .AddFrame(40, alpha: 255)
            .EndFrameSet()
            .BeginFrameSet(50, 59)
            .AddFrame(50, alpha: 255)
            .AddFrame(52, alpha: 0)
            .EndFrameSet()
            .Build()
        );

        SelectedBackgroundNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(60, 69)
            .AddFrame(60, alpha: 214)
            .AddFrame(60, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(70, 79)
            .AddFrame(70, alpha: 214)
            .AddFrame(72, alpha: 255)
            .AddFrame(70, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(72, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(80, 89)
            .AddFrame(80, alpha: 255)
            .AddFrame(80, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(90, 99)
            .AddFrame(90, alpha: 178)
            .AddFrame(90, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(50, 50, 50))
            .EndFrameSet()
            .BeginFrameSet(100, 109)
            .AddFrame(100, alpha: 255)
            .AddFrame(100, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(110, 120)
            .AddFrame(110, alpha: 255)
            .AddFrame(112, alpha: 214)
            .AddFrame(110, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(112, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .Build()
        );

        LabelNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 9)
            .AddFrame(1, alpha: 255)
            .EndFrameSet()
            .BeginFrameSet(10, 19)
            .AddFrame(10, alpha: 255)
            .EndFrameSet()
            .BeginFrameSet(20, 29)
            .AddFrame(20, alpha: 255)
            .EndFrameSet()
            .BeginFrameSet(30, 39)
            .AddFrame(30, alpha: 127)
            .EndFrameSet()
            .BeginFrameSet(40, 49)
            .AddFrame(40, alpha: 255)
            .EndFrameSet()
            .BeginFrameSet(50, 59)
            .AddFrame(50, alpha: 255)
            .EndFrameSet()
            .BeginFrameSet(60, 69)
            .AddFrame(60, alpha: 255)
            .EndFrameSet()
            .BeginFrameSet(70, 79)
            .AddFrame(70, alpha: 255)
            .EndFrameSet()
            .BeginFrameSet(80, 89)
            .AddFrame(80, alpha: 255)
            .EndFrameSet()
            .BeginFrameSet(90, 99)
            .AddFrame(90, alpha: 127)
            .EndFrameSet()
            .BeginFrameSet(100, 109)
            .AddFrame(100, alpha: 255)
            .EndFrameSet()
            .BeginFrameSet(110, 120)
            .AddFrame(110, alpha: 255)
            .EndFrameSet()
            .Build()
        );
    }
}
