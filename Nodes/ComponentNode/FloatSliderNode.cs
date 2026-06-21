using System;
using System.Globalization;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes.ComponentNode;
using KamiToolKit.Nodes.Simplified;
using KamiToolKit.Timelines;

namespace KamiToolKit.Nodes;

/// <summary>
/// Implementation of the games SliderNode and associated component.
/// </summary>
public unsafe class FloatSliderNode : ComponentNode<AtkComponentSlider, AtkUldComponentDataSlider> {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public NineGridNode ProgressTextureNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public SliderBackgroundButtonNode SliderBackgroundButtonNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public SliderForegroundButtonNode SliderForegroundButtonNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public TextNode FloatValueNode { get; }

    /// <summary>
    /// Gets or sets the action to be invoked when the value is changed. Provides the new value.
    /// </summary>
    public Action<float>? OnValueChanged { get; set; }

    /// <summary>
    /// Gets or sets the min value allowed.
    /// </summary>
    public float Min {
        get => Component->MinValue / 100.0f;
        set => Component->SetMinValue((int) (value * 100.0f));
    }

    /// <summary>
    /// Gets or sets the max value allowed.
    /// </summary>
    public float Max {
        get => Component->MaxValue / 100.0f;
        set => Component->SetMaxValue((int) (value * 100.0f));
    }

    /// <summary>
    /// Gets or sets the step value used when interacting with a controller.
    /// </summary>
    public float Step {
        get => Component->Steps / 100.0f;
        set => Component->Steps = (int) (value * 100.0f);
    }

    /// <summary>
    /// Gets or sets the current value. When setting triggers <see cref="OnValueChanged"/>
    /// </summary>
    public float Value {
        get => Component->Value / 100.0f;
        set => Component->SetValue((int) (value * 100.0f));
    }

    /// <summary>
    /// Constructs a new <see cref="FloatSliderNode"/>.
    /// </summary>
    public FloatSliderNode() {
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

        FloatValueNode = new TextNode {
            Size = new Vector2(24.0f, 16.0f),
            IsVisible = false,
            FontType = FontType.Axis,
            FontSize = 12,
            AlignmentType = AlignmentType.TopLeft,
            TextFlags = TextFlags.AutoAdjustNodeSize,
        };
        FloatValueNode.AttachNode(this);

        Data->Step = 5;
        Data->Min = 0;
        Data->Max = 10000;
        Data->OfffsetL = 4;
        Data->OffsetR = 50;

        Data->Nodes[0] = ProgressTextureNode.NodeId;
        Data->Nodes[1] = SliderForegroundButtonNode.NodeId;
        Data->Nodes[2] = 0;
        Data->Nodes[3] = SliderBackgroundButtonNode.NodeId;

        BuildTimelines();

        InitializeComponentEvents();

        Component->SliderSize = 220;
        Component->OffsetR = 50;
        Component->OffsetL = 4;

        AddEvent(AtkEventType.SliderValueUpdate, ValueChangedHandler);

        FocusNode = SliderForegroundButtonNode;
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        SliderBackgroundButtonNode.Size = new Vector2(Width - 18.0f - 25.0f, Height / 2.0f);
        SliderBackgroundButtonNode.Position = new Vector2(0.0f, 4.0f);

        ProgressTextureNode.Size = new Vector2(0.0f, Height / 2.0f - 1.0f);
        ProgressTextureNode.Position = new Vector2(0.0f, 4.0f);

        SliderForegroundButtonNode.Size = new Vector2(Height - 4.0f, Height - 4.0f);
        SliderForegroundButtonNode.Position = new Vector2(0.0f, 0.0f);

        FloatValueNode.Size = new Vector2(0.0f, Height);
        FloatValueNode.Position = new Vector2(Width - 18.0f - 20.0f, 0.0f);

        Component->SliderSize = (short)Width;
    }

    private void ValueChangedHandler() {
        OnValueChanged?.Invoke(Value);
        UpdateFormattedText();
    }

    private void UpdateFormattedText() {
        FloatValueNode.String = string.Format(DecimalFormatInfo, "{0:F}", Value);
    }

    /// <inheritdoc />
    protected override void OnReceiveEvent(AtkComponentBase* component, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        base.OnReceiveEvent(component, eventType, eventParam, atkEvent, atkEventData);

        FloatValueNode.String = string.Format(DecimalFormatInfo, "{0:F}", Value);
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

        FloatValueNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 20)
            .AddFrame(1, alpha: 255)
            .EndFrameSet()
            .BeginFrameSet(21, 30)
            .AddFrame(21, alpha: 153)
            .EndFrameSet()
            .Build()
        );
    }

    private static readonly NumberFormatInfo DecimalFormatInfo = new() {
        NumberDecimalDigits = 2,
    };
}
