using System;
using System.Globalization;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Timelines;

namespace KamiToolKit.Nodes;

public unsafe class SliderNode : ComponentNode<AtkComponentSlider, AtkUldComponentDataSlider> {

    public readonly NineGridNode ProgressTextureNode;
    public readonly SliderBackgroundButtonNode SliderBackgroundButtonNode;
    public readonly SliderForegroundButtonNode SliderForegroundButtonNode;
    public readonly TextNode ValueNode;
    public readonly TextNode FloatValueNode;

    public SliderNode() {
        SetInternalComponentType(ComponentType.Slider);

        SliderBackgroundButtonNode = new SliderBackgroundButtonNode();
        SliderBackgroundButtonNode.AttachNode(this);

        ProgressTextureNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/SliderGaugeHorizontalA.tex",
            TextureCoordinates = new Vector2(16.0f, 8.0f),
            TextureSize = new Vector2(40.0f, 7.0f),
            Height = 7.0f,
            Y = 4.0f,
            LeftOffset = 8,
            RightOffset = 8,
        };
        ProgressTextureNode.AttachNode(this);

        SliderForegroundButtonNode = new SliderForegroundButtonNode {
            Size = new Vector2(16.0f, 16.0f), 
        };
        SliderForegroundButtonNode.AttachNode(this);

        ValueNode = new TextNode {
            Size = new Vector2(24.0f, 16.0f),
            FontType = FontType.Axis,
            FontSize = 12,
            AlignmentType = AlignmentType.TopLeft,
            TextFlags = TextFlags.AutoAdjustNodeSize,
        };
        ValueNode.AttachNode(this);

        FloatValueNode = new TextNode {
            Size = new Vector2(24.0f, 16.0f),
            IsVisible = false,
            FontType = FontType.Axis,
            FontSize = 12,
            AlignmentType = AlignmentType.TopLeft,
            TextFlags = TextFlags.AutoAdjustNodeSize,
        };
        FloatValueNode.AttachNode(this);

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

        AddEvent(AtkEventType.SliderValueUpdate, ValueChangedHandler);
    }

    public Action<int>? OnValueChanged { get; set; }

    public required Range Range {
        get => Data->Min .. Data->Max;
        set {
            Component->SetMaxValue(value.End.Value);
            Component->SetMinValue(value.Start.Value);

            Value = Math.Clamp(Value, value.Start.Value, value.End.Value);
        }
    }

    public int Step {
        get => Component->Steps;
        set => Component->Steps = value;
    }

    public int Value {
        get => Component->Value;
        set {
            Component->SetValue(value);
            UpdateFormattedText();
        }
    }

    public int DecimalPlaces {
        get;
        set {
            field = value;
            UpdateFormattedText();
        }
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        SliderBackgroundButtonNode.Size = new Vector2(Width - 18.0f, Height / 2.0f);
        SliderBackgroundButtonNode.Position = new Vector2(0.0f, 4.0f);

        ProgressTextureNode.Size = new Vector2(0.0f, Height / 2.0f - 1.0f);
        ProgressTextureNode.Position = new Vector2(0.0f, 4.0f);

        SliderForegroundButtonNode.Size = new Vector2(Height - 4.0f, Height - 4.0f);
        SliderForegroundButtonNode.Position = new Vector2(0.0f, 0.0f);

        ValueNode.Size = new Vector2(0.0f, Height);
        ValueNode.Position = new Vector2(Width - 18.0f, 0.0f);
        
        FloatValueNode.Size = new Vector2(0.0f, Height);
        FloatValueNode.Position = new Vector2(Width - 18.0f, 0.0f);

        Component->SliderSize = (short)Width;
    }

    private void ValueChangedHandler() {
        OnValueChanged?.Invoke(Value);
        UpdateFormattedText();
    }

    private void UpdateFormattedText() {
        if (DecimalPlaces is not 0) {
            var formatInfo = new NumberFormatInfo {
                NumberDecimalDigits = DecimalPlaces,
            };
            
            FloatValueNode.IsVisible = true;
            FloatValueNode.String = string.Format(formatInfo, "{0:F}", Value / MathF.Pow(10, DecimalPlaces));
            ValueNode.FontSize = 0;
        }
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
