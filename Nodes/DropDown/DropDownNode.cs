using System.Numerics;
using Dalamud.Game.Addon.Events;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Classes.TimelineBuilding;

namespace KamiToolKit.Nodes;

public abstract unsafe class DropDownNode<T, TU> : SimpleComponentNode where T : ListNode<TU>, new() {

    public readonly NineGridNode BackgroundNode;
    public readonly ImageNode CollapseArrowNode;
    public readonly CollisionNode DropDownFocusCollisionNode;
    public readonly TextNode LabelNode;
    public readonly T OptionListNode;

    protected DropDownNode() {
        BackgroundNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/DropDownA.tex",
            TextureSize = new Vector2(44.0f, 23.0f),
            TextureCoordinates = new Vector2(0.0f, 0.0f),
            Size = new Vector2(250.0f, 24.0f),
            Height = 23.0f,
            IsVisible = true,
            LeftOffset = 16.0f,
            RightOffset = 16.0f,
        };

        BackgroundNode.AttachNode(this);

        CollapseArrowNode = new SimpleImageNode {
            TexturePath = "ui/uld/DropDownA.tex",
            TextureCoordinates = new Vector2(44.0f, 0.0f),
            TextureSize = new Vector2(12.0f, 12.0f),
            Position = new Vector2(6.0f, 17.0f),
            Size = new Vector2(12.0f, 12.0f),
            IsVisible = true,
            WrapMode = 2,
            ImageNodeFlags = 0,
        };

        CollapseArrowNode.AttachNode(this);

        LabelNode = new TextNode {
            Position = new Vector2(20.0f, 0.0f),
            Size = new Vector2(218.0f, 21.0f),
            FontType = FontType.Axis,
            FontSize = 12,
            AlignmentType = AlignmentType.Left,
            TextColor = ColorHelper.GetColor(1),
            TextOutlineColor = ColorHelper.GetColor(2),
            IsVisible = true,
            Text = "Demo",
            TextFlags = TextFlags.AutoAdjustNodeSize | TextFlags.Emboss,
        };

        LabelNode.AttachNode(this);

        OptionListNode = new T {
            Position = new Vector2(4.0f, 21.0f), Size = new Vector2(242.0f, 243.0f), NodeFlags = NodeFlags.EmitsEvents,
        };

        OptionListNode.AttachNode(this);

        DropDownFocusCollisionNode = new CollisionNode {
            IsVisible = true, EventFlagsSet = true,
        };
        DropDownFocusCollisionNode.AttachNode(OptionListNode.CollisionNode, NodePosition.AfterTarget);
        DropDownFocusCollisionNode.AddEvent(AddonEventType.MouseDown, _ => Toggle());
        DropDownFocusCollisionNode.AddEvent(AddonEventType.MouseWheel, _ => Toggle());

        BuildTimelines();

        Timeline?.PlayAnimation(4);

        CollisionNode.SetEventFlags();
        CollisionNode.AddEvent(AddonEventType.MouseOver, _ => Timeline?.PlayAnimation(IsCollapsed ? 2 : 9));
        CollisionNode.AddEvent(AddonEventType.MouseOut, _ => Timeline?.PlayAnimation(IsCollapsed ? 4 : 11));
        CollisionNode.AddEvent(AddonEventType.MouseClick, _ => Toggle());
    }

    public bool IsCollapsed { get; set; } = true;

    public int MaxListOptions {
        get => OptionListNode.MaxButtons;
        set => OptionListNode.MaxButtons = value;
    }

    public TU? SelectedOption {
        get => OptionListNode.SelectedOption;
        set {
            OptionListNode.SelectedOption = value;
            UpdateLabel(value);
        }
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        CollisionNode.Size = Size;
        BackgroundNode.Size = new Vector2(Width, Height - 1.0f);
        LabelNode.Size = new Vector2(Width - 32.0f, Height - 3.0f);

        OptionListNode.Width = Width - 8.0f;
        OptionListNode.Position = new Vector2(4.0f, Height - 3.0f);
    }

    public void Toggle() {
        IsCollapsed = !IsCollapsed;
        Timeline?.PlayAnimation(IsCollapsed ? 4 : 11);
        OptionListNode.Toggle(!IsCollapsed);

        var parentAddon = RaptureAtkUnitManager.Instance()->GetAddonByNode(InternalResNode);
        if (parentAddon is not null) {

            if (!IsCollapsed) {
                OptionListNode.Position = ScreenPosition + Size with {
                    X = 0.0f,
                } - new Vector2(parentAddon->X, parentAddon->Y) - new Vector2(0.0f, 4.0f);

                DropDownFocusCollisionNode.Position = -OptionListNode.Position;
                DropDownFocusCollisionNode.Size = new Vector2(parentAddon->RootNode->Width, parentAddon->RootNode->Height);

                OptionListNode.ReattachNode(parentAddon->RootNode);
            }
            else {
                OptionListNode.ReattachNode(this);
            }
        }
    }

    protected abstract void UpdateLabel(TU? option);

    private void BuildTimelines() {
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

        CollapseArrowNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 9)
            .AddFrame(1, new Vector2(6, 17))
            .AddFrame(1, rotation: 4.712389f)
            .AddFrame(1, alpha: 255)
            .AddFrame(1, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(10, 19)
            .AddFrame(10, new Vector2(6, 17))
            .AddFrame(12, new Vector2(6, 17))
            .AddFrame(10, rotation: 4.712389f)
            .AddFrame(12, rotation: 4.712389f)
            .AddFrame(10, alpha: 255)
            .AddFrame(12, alpha: 255)
            .AddFrame(10, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(12, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(20, 29)
            .AddFrame(20, new Vector2(6, 18))
            .AddFrame(20, rotation: 4.712389f)
            .AddFrame(20, alpha: 255)
            .AddFrame(20, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(30, 39)
            .AddFrame(30, new Vector2(6, 17))
            .AddFrame(30, rotation: 4.712389f)
            .AddFrame(30, alpha: 178)
            .AddFrame(30, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(50, 50, 50))
            .EndFrameSet()
            .BeginFrameSet(40, 49)
            .AddFrame(40, new Vector2(6, 17))
            .AddFrame(40, rotation: 4.712389f)
            .AddFrame(40, alpha: 255)
            .AddFrame(40, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(50, 59)
            .AddFrame(50, new Vector2(6, 17))
            .AddFrame(52, new Vector2(6, 17))
            .AddFrame(50, rotation: 4.712389f)
            .AddFrame(52, rotation: 4.712389f)
            .AddFrame(50, alpha: 255)
            .AddFrame(52, alpha: 255)
            .AddFrame(50, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(52, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(60, 69)
            .AddFrame(60, new Vector2(6, 6))
            .AddFrame(60, rotation: 0)
            .AddFrame(60, alpha: 255)
            .AddFrame(60, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(70, 79)
            .AddFrame(70, new Vector2(6, 6))
            .AddFrame(72, new Vector2(6, 6))
            .AddFrame(70, rotation: 0)
            .AddFrame(72, rotation: 0)
            .AddFrame(70, alpha: 255)
            .AddFrame(72, alpha: 255)
            .AddFrame(70, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(72, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(80, 89)
            .AddFrame(80, new Vector2(6, 7))
            .AddFrame(80, rotation: 0)
            .AddFrame(80, alpha: 255)
            .AddFrame(80, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(90, 99)
            .AddFrame(90, new Vector2(6, 6))
            .AddFrame(90, rotation: 0)
            .AddFrame(90, alpha: 178)
            .AddFrame(90, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(50, 50, 50))
            .EndFrameSet()
            .BeginFrameSet(100, 109)
            .AddFrame(100, new Vector2(6, 6))
            .AddFrame(100, rotation: 0)
            .AddFrame(100, alpha: 255)
            .AddFrame(100, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(110, 120)
            .AddFrame(110, new Vector2(6, 6))
            .AddFrame(112, new Vector2(6, 6))
            .AddFrame(110, rotation: 0)
            .AddFrame(112, rotation: 0)
            .AddFrame(110, alpha: 255)
            .AddFrame(112, alpha: 255)
            .AddFrame(110, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(112, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .Build()
        );

        LabelNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 9)
            .AddFrame(1, new Vector2(20, 0))
            .AddFrame(1, alpha: 255)
            .AddFrame(1, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(10, 19)
            .AddFrame(10, new Vector2(20, 0))
            .AddFrame(10, alpha: 255)
            .AddFrame(10, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(20, 29)
            .AddFrame(20, new Vector2(20, 1))
            .AddFrame(20, alpha: 255)
            .AddFrame(20, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(30, 39)
            .AddFrame(30, new Vector2(20, 0))
            .AddFrame(30, alpha: 153)
            .AddFrame(30, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(80, 80, 80))
            .EndFrameSet()
            .BeginFrameSet(40, 49)
            .AddFrame(40, new Vector2(20, 0))
            .AddFrame(40, alpha: 255)
            .AddFrame(40, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(50, 59)
            .AddFrame(50, new Vector2(20, 0))
            .AddFrame(50, alpha: 255)
            .AddFrame(50, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(60, 69)
            .AddFrame(60, new Vector2(20, 0))
            .AddFrame(60, alpha: 255)
            .AddFrame(60, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(70, 79)
            .AddFrame(70, new Vector2(20, 0))
            .AddFrame(70, alpha: 255)
            .AddFrame(70, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(80, 89)
            .AddFrame(80, new Vector2(20, 1))
            .AddFrame(80, alpha: 255)
            .AddFrame(80, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(90, 99)
            .AddFrame(90, new Vector2(20, 0))
            .AddFrame(90, alpha: 153)
            .AddFrame(90, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(80, 80, 80))
            .EndFrameSet()
            .BeginFrameSet(100, 109)
            .AddFrame(100, new Vector2(20, 0))
            .AddFrame(100, alpha: 255)
            .AddFrame(100, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(110, 120)
            .AddFrame(110, new Vector2(20, 0))
            .AddFrame(110, alpha: 255)
            .AddFrame(110, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .Build()
        );

        BackgroundNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 9)
            .AddFrame(1, new Vector2(0, 0))
            .AddFrame(1, alpha: 255)
            .AddFrame(1, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(10, 19)
            .AddFrame(10, new Vector2(0, 0))
            .AddFrame(12, new Vector2(0, 0))
            .AddFrame(10, alpha: 255)
            .AddFrame(12, alpha: 255)
            .AddFrame(10, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(12, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(20, 29)
            .AddFrame(20, new Vector2(0, 1))
            .AddFrame(20, alpha: 255)
            .AddFrame(20, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(30, 39)
            .AddFrame(30, new Vector2(0, 0))
            .AddFrame(30, alpha: 178)
            .AddFrame(30, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(50, 50, 50))
            .EndFrameSet()
            .BeginFrameSet(40, 49)
            .AddFrame(40, new Vector2(0, 0))
            .AddFrame(40, alpha: 255)
            .AddFrame(40, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(50, 59)
            .AddFrame(50, new Vector2(0, 0))
            .AddFrame(52, new Vector2(0, 0))
            .AddFrame(50, alpha: 255)
            .AddFrame(52, alpha: 255)
            .AddFrame(50, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(52, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(60, 69)
            .AddFrame(60, new Vector2(0, 0))
            .AddFrame(60, alpha: 255)
            .AddFrame(60, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(70, 79)
            .AddFrame(70, new Vector2(0, 0))
            .AddFrame(72, new Vector2(0, 0))
            .AddFrame(70, alpha: 255)
            .AddFrame(72, alpha: 255)
            .AddFrame(70, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(72, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(80, 89)
            .AddFrame(80, new Vector2(0, 1))
            .AddFrame(80, alpha: 255)
            .AddFrame(80, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(90, 99)
            .AddFrame(90, new Vector2(0, 0))
            .AddFrame(90, alpha: 178)
            .AddFrame(90, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(50, 50, 50))
            .EndFrameSet()
            .BeginFrameSet(100, 109)
            .AddFrame(100, new Vector2(0, 0))
            .AddFrame(100, alpha: 255)
            .AddFrame(100, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(110, 120)
            .AddFrame(110, new Vector2(0, 0))
            .AddFrame(112, new Vector2(0, 0))
            .AddFrame(110, alpha: 255)
            .AddFrame(112, alpha: 255)
            .AddFrame(110, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(112, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .Build()
        );
    }
}
