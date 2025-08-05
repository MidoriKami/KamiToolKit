using System;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Addon.Events.EventDataTypes;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Classes.TimelineBuilding;

namespace KamiToolKit.Nodes;

public unsafe class CheckboxNode : ComponentNode<AtkComponentCheckBox, AtkUldComponentDataCheckBox> {

    public readonly ImageNode BoxBackground;
    public readonly ImageNode BoxForeground;
    public readonly TextNode Label;

    public CheckboxNode() {
        SetInternalComponentType(ComponentType.CheckBox);

        BoxBackground = new SimpleImageNode {
            NodeId = 4,
            TexturePath = "ui/uld/CheckBoxA.tex",
            TextureCoordinates = new Vector2(0.0f, 0.0f),
            TextureSize = new Vector2(16.0f, 16.0f),
            Size = new Vector2(16.0f, 16.0f),
            Position = new Vector2(0.0f, 2.0f),
            NodeFlags = NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            WrapMode = 1,
            ImageNodeFlags = 0,
        };

        BoxBackground.AttachNode(this);

        BoxForeground = new SimpleImageNode {
            NodeId = 3,
            TexturePath = "ui/uld/CheckBoxA.tex",
            TextureCoordinates = new Vector2(16.0f, 0.0f),
            TextureSize = new Vector2(16.0f, 16.0f),
            Size = new Vector2(16.0f, 16.0f),
            Position = new Vector2(0.0f, 2.0f),
            NodeFlags = NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            WrapMode = 1,
            ImageNodeFlags = 0,
            DrawFlags = 0,
        };

        BoxForeground.AttachNode(this);

        Label = new TextNode {
            NodeId = 2,
            Size = new Vector2(0.0f, 20.0f),
            Position = new Vector2(20.0f, 0.0f),
            NodeFlags = NodeFlags.AnchorLeft | NodeFlags.AnchorRight | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            FontType = FontType.Axis,
            AlignmentType = AlignmentType.Left,
            FontSize = 14,
            LineSpacing = 14,
            TextColor = ColorHelper.GetColor(8),
            TextOutlineColor = ColorHelper.GetColor(7),
            TextFlags = TextFlags.Edge | TextFlags.AutoAdjustNodeSize,
        };

        Label.AttachNode(this);

        Component->Flags = 606464;

        Data->Nodes[0] = Label.NodeId;
        Data->Nodes[1] = BoxBackground.NodeId;
        Data->Nodes[2] = 0;

        LoadTimelines();

        AddEvent(AddonEventType.ButtonClick, ClickHandler);

        InitializeComponentEvents();
        Component->Left = 20;
        Component->Right = 20;
        Component->Top = 0;
        Component->Bottom = 0;

        BoxForeground.IsVisible = Component->IsChecked;
        BoxForeground.DrawFlags = 0;

    }

    public Action<bool>? OnClick { get; set; }

    public SeString LabelText {
        get => Label.Text;
        set {
            Label.Text = value;
            CollisionNode.Width = BoxBackground.Width + Label.Width + Label.X - X;
            Width = CollisionNode.Width;
        }
    }

    public bool IsEnabled {
        get => Component->IsEnabled;
        set => Component->SetEnabledState(value);
    }

    public bool IsChecked {
        get => Component->IsChecked;
        set => Component->SetChecked(value);
    }

    private void ClickHandler(AddonEventData data) {
        OnClick?.Invoke(Component->IsChecked);
    }

    private void LoadTimelines() {
        AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 155)
            .AddLabelPair(1, 10, 1)
            .AddLabelPair(11, 20, 2)
            .AddLabelPair(21, 30, 3)
            .AddLabelPair(31, 40, 7)
            .AddLabelPair(41, 50, 6)
            .AddLabelPair(51, 60, 4)
            .AddLabelPair(61, 70, 8)
            .AddLabelPair(71, 80, 9)
            .AddLabelPair(81, 90, 10)
            .AddLabelPair(91, 100, 14)
            .AddLabelPair(101, 110, 13)
            .AddLabelPair(111, 115, 11)
            .AddLabelPair(116, 125, 12)
            .AddLabelPair(126, 135, 5)
            .AddLabelPair(136, 145, 15)
            .AddLabelPair(146, 155, 16)
            .EndFrameSet()
            .Build());

        CollisionNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 155)
            .AddEmptyFrame(1)
            .EndFrameSet()
            .Build());

        BoxBackground.AddTimeline(new TimelineBuilder()
            .AddFrameSetWithFrame(1, 10, 1, new Vector2(0.0f, 2.0f), 255, multiplyColor: new Vector3(100.0f))
            .BeginFrameSet(11, 20)
            .AddFrame(11, new Vector2(0.0f, 2.0f), 255, multiplyColor: new Vector3(100.0f))
            .AddFrame(13, new Vector2(0.0f, 2.0f), 255, new Vector3(16.0f), new Vector3(100.0f))
            .EndFrameSet()
            .AddFrameSetWithFrame(21, 30, 21, new Vector2(0.0f, 2.0f), 255, new Vector3(16.0f), new Vector3(100.0f))
            .AddFrameSetWithFrame(31, 40, 31, new Vector2(0.0f, 2.0f), 102, multiplyColor: new Vector3(80.0f))
            .AddFrameSetWithFrame(41, 50, 41, new Vector2(0.0f, 2.0f), 255, new Vector3(16.0f), new Vector3(100.0f))
            .BeginFrameSet(51, 60)
            .AddFrame(51, new Vector2(0.0f, 2.0f), 255, new Vector3(16.0f), new Vector3(100.0f))
            .AddFrame(60, new Vector2(0.0f, 2.0f), 255, multiplyColor: new Vector3(100.0f))
            .EndFrameSet()
            .AddFrameSetWithFrame(61, 70, 61, new Vector2(0.0f, 2.0f), 255, multiplyColor: new Vector3(100.0f))
            .BeginFrameSet(71, 80)
            .AddFrame(71, new Vector2(0.0f, 2.0f), 255, multiplyColor: new Vector3(100.0f))
            .AddFrame(73, new Vector2(0.0f, 2.0f), 255, new Vector3(16.0f), new Vector3(100.0f))
            .EndFrameSet()
            .AddFrameSetWithFrame(81, 90, 81, new Vector2(0.0f, 2.0f), 255, new Vector3(16.0f), new Vector3(100.0f))
            .AddFrameSetWithFrame(91, 100, 91, new Vector2(0.0f, 2.0f), 102, multiplyColor: new Vector3(80.0f))
            .AddFrameSetWithFrame(101, 110, 101, new Vector2(0.0f, 2.0f), 255, new Vector3(16.0f), new Vector3(100.0f))
            .BeginFrameSet(111, 115)
            .AddFrame(111, new Vector2(0.0f, 2.0f), 255, new Vector3(16.0f), new Vector3(100.0f))
            .AddFrame(115, new Vector2(0.0f, 2.0f), 255, multiplyColor: new Vector3(100.0f))
            .EndFrameSet()
            .AddFrameSetWithFrame(116, 125, 116, new Vector2(0.0f, 2.0f), addColor: new Vector3(16.0f), multiplyColor: new Vector3(100.0f))
            .AddFrameSetWithFrame(126, 135, 126, new Vector2(0.0f, 2.0f), 255, new Vector3(16.0f), new Vector3(100.0f))
            .AddFrameSetWithFrame(136, 145, 126, new Vector2(0.0f, 2.0f), 255, multiplyColor: new Vector3(100.0f))
            .AddFrameSetWithFrame(146, 155, 146, new Vector2(0.0f, 2.0f), 255, multiplyColor: new Vector3(100.0f))
            .Build());

        BoxForeground.AddTimeline(new TimelineBuilder()
            .AddFrameSetWithFrame(61, 70, 61, alpha: 255, multiplyColor: new Vector3(100.0f))
            .BeginFrameSet(71, 80)
            .AddFrame(71, alpha: 255, multiplyColor: new Vector3(100.0f))
            .AddFrame(73, alpha: 255, multiplyColor: new Vector3(100.0f), addColor: new Vector3(16.0f))
            .EndFrameSet()
            .AddFrameSetWithFrame(81, 90, 81, alpha: 255, addColor: new Vector3(16.0f), multiplyColor: new Vector3(100.0f))
            .AddFrameSetWithFrame(91, 100, 91, alpha: 102, multiplyColor: new Vector3(80.0f))
            .AddFrameSetWithFrame(101, 110, 101, alpha: 255, addColor: new Vector3(16.0f), multiplyColor: new Vector3(100.0f))
            .BeginFrameSet(111, 115)
            .AddFrame(111, alpha: 255, addColor: new Vector3(16.0f), multiplyColor: new Vector3(100.0f))
            .AddFrame(115, alpha: 255, multiplyColor: new Vector3(100.0f))
            .EndFrameSet()
            .BeginFrameSet(116, 125)
            .AddFrame(116, alpha: 0, addColor: new Vector3(16.0f), multiplyColor: new Vector3(100.0f))
            .AddFrame(119, alpha: 255, addColor: new Vector3(16.0f), multiplyColor: new Vector3(100.0f))
            .EndFrameSet()
            .BeginFrameSet(126, 135)
            .AddFrame(126, alpha: 255, addColor: new Vector3(16.0f), multiplyColor: new Vector3(100.0f))
            .AddFrame(129, alpha: 0, addColor: new Vector3(16.0f), multiplyColor: new Vector3(100.0f))
            .EndFrameSet()
            .BeginFrameSet(136, 145)
            .AddFrame(136, alpha: 0, multiplyColor: new Vector3(100.0f))
            .AddFrame(140, alpha: 255, multiplyColor: new Vector3(100.0f))
            .EndFrameSet()
            .BeginFrameSet(146, 255)
            .AddFrame(146, alpha: 255, multiplyColor: new Vector3(100.0f))
            .AddFrame(150, alpha: 0, multiplyColor: new Vector3(100.0f))
            .EndFrameSet()
            .Build());

        Label.AddTimeline(new TimelineBuilder()
            .AddFrameSetWithFrame(1, 10, 1, alpha: 255, multiplyColor: new Vector3(100.0f))
            .AddFrameSetWithFrame(11, 20, 11, alpha: 255, multiplyColor: new Vector3(100.0f))
            .AddFrameSetWithFrame(21, 30, 21, alpha: 255, multiplyColor: new Vector3(100.0f))
            .AddFrameSetWithFrame(31, 40, 31, alpha: 102, multiplyColor: new Vector3(80.0f))
            .AddFrameSetWithFrame(41, 50, 41, alpha: 255, multiplyColor: new Vector3(100.0f))
            .AddFrameSetWithFrame(51, 60, 51, alpha: 255, multiplyColor: new Vector3(100.0f))
            .AddFrameSetWithFrame(61, 70, 61, alpha: 255, multiplyColor: new Vector3(100.0f))
            .AddFrameSetWithFrame(71, 80, 71, alpha: 255, multiplyColor: new Vector3(100.0f))
            .AddFrameSetWithFrame(81, 90, 81, alpha: 255, multiplyColor: new Vector3(100.0f))
            .AddFrameSetWithFrame(91, 100, 91, alpha: 102, multiplyColor: new Vector3(80.0f))
            .AddFrameSetWithFrame(101, 110, 101, alpha: 255, multiplyColor: new Vector3(100.0f))
            .AddFrameSetWithFrame(111, 115, 111, alpha: 255, multiplyColor: new Vector3(100.0f))
            .AddFrameSetWithFrame(116, 135, 116, alpha: 255, multiplyColor: new Vector3(100.0f))
            .AddFrameSetWithFrame(126, 135, 126, alpha: 255, multiplyColor: new Vector3(100.0f))
            .AddFrameSetWithFrame(136, 145, 136, alpha: 255, multiplyColor: new Vector3(100.0f))
            .AddFrameSetWithFrame(146, 155, 146, alpha: 255, multiplyColor: new Vector3(100.0f))
            .Build());
    }
}
