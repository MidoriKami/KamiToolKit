using System;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Addon.Events.EventDataTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.TimelineBuilding;

namespace KamiToolKit.Nodes.Slider;

public unsafe class SliderNode : ComponentNode<AtkComponentSlider, AtkUldComponentDataSlider> {

    public readonly NineGridNode ProgressTextureNode;
    public readonly SliderBackgroundButtonNode SliderBackgroundButtonNode;
    public readonly SliderForegroundButtonNode SliderForegroundButtonNode;
    public readonly TextNode ValueNode;

    public SliderNode() {
        SetInternalComponentType(ComponentType.Slider);

        SliderBackgroundButtonNode = new SliderBackgroundButtonNode {
            NodeId = 5, IsVisible = true,
        };

        SliderBackgroundButtonNode.AttachNode(this);

        ProgressTextureNode = new SimpleNineGridNode {
            NodeId = 4,
            TexturePath = "ui/uld/SliderGaugeHorizontalA.tex",
            TextureCoordinates = new Vector2(16.0f, 8.0f),
            TextureSize = new Vector2(40.0f, 7.0f),
            Height = 7.0f,
            Y = 4.0f,
            LeftOffset = 8,
            RightOffset = 8,
            IsVisible = true,
        };

        ProgressTextureNode.AttachNode(this);

        SliderForegroundButtonNode = new SliderForegroundButtonNode {
            NodeId = 3, Size = new Vector2(16.0f, 16.0f), IsVisible = true,
        };

        SliderForegroundButtonNode.AttachNode(this);

        ValueNode = new TextNode {
            NodeId = 2,
            Size = new Vector2(24.0f, 16.0f),
            IsVisible = true,
            FontType = FontType.Axis,
            FontSize = 12,
            AlignmentType = AlignmentType.Left,
        };

        ValueNode.AttachNode(this);

        Data->Step = 1;
        Data->Min = 0;
        Data->Max = 100;
        Data->OfffsetL = 4;
        Data->OffsetR = 28;

        Data->Nodes[0] = ProgressTextureNode.NodeId;
        Data->Nodes[1] = SliderForegroundButtonNode.NodeId;
        Data->Nodes[2] = ValueNode.NodeId;
        Data->Nodes[3] = SliderBackgroundButtonNode.NodeId;

        BuildTimelines();

        InitializeComponentEvents();

        Component->SliderSize = 220;
        Component->OffsetR = 28;
        Component->OffsetL = 4;

        AddEvent(AddonEventType.SliderValueUpdate, ValueChangedHandler);
    }

    public Action<int>? OnValueChanged { get; set; }

    public required int Min {
        get => Component->MinValue;
        set {
            Component->SetMinValue(value);
            Component->SetValue(value);
        }
    }

    public required int Max {
        get => Component->MaxValue;
        set => Component->SetMaxValue(value);
    }

    public int Step {
        get => Component->Steps;
        set => Component->Steps = value;
    }

    public int Value {
        get => Component->Value;
        set => Component->SetValue(value);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        SliderBackgroundButtonNode.Size = new Vector2(Width - 24.0f, Height);
        ProgressTextureNode.Size = new Vector2(Width, Height / 2.0f - 1.0f);
        ProgressTextureNode.Y = Height / 4.0f;
        ValueNode.Position = new Vector2(Width - 24.0f, Height / 4.0f);
        SliderForegroundButtonNode.Size = new Vector2(Height / 2.0f, Height / 2.0f - 1.0f);
        SliderForegroundButtonNode.Y = Height / 4.0f;

        Component->SliderSize = (short)Width;
    }

    private void ValueChangedHandler(AddonEventData obj) {
        OnValueChanged?.Invoke(Value);
    }

    private void BuildTimelines() {
        AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 30)
            .AddLabel(1, 17, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(11, 18, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(21, 7, AtkTimelineJumpBehavior.PlayOnce, 0)
            .EndFrameSet()
            .Build()
        );

        ProgressTextureNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 20)
            .AddFrame(1, alpha: 255)
            .EndFrameSet()
            .BeginFrameSet(21, 30)
            .AddFrame(21, alpha: 127)
            .EndFrameSet()
            .Build()
        );

        ValueNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 20)
            .AddFrame(1, alpha: 255)
            .EndFrameSet()
            .BeginFrameSet(21, 30)
            .AddFrame(21, alpha: 153)
            .EndFrameSet()
            .Build()
        );
    }
}
