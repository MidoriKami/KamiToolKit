using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.TimelineBuilding;

namespace KamiToolKit.Nodes.Slider;

public class SliderForegroundButtonNode : ComponentNode<AtkComponentButton, AtkUldComponentDataButton> {

    public readonly ImageNode HandleNode;

    public SliderForegroundButtonNode() {
        SetInternalComponentType(ComponentType.Button);

        HandleNode = new SimpleImageNode {
            TexturePath = "ui/uld/SliderGaugeHorizontalA.tex",
            TextureCoordinates = new Vector2(1.0f, 1.0f),
            TextureSize = new Vector2(14.0f, 15.0f),
            Size = new Vector2(14.0f, 15.0f),
            IsVisible = true,
            WrapMode = 1,
            ImageNodeFlags = 0,
        };

        HandleNode.AttachNode(this);

        BuildTimelines();

        InitializeComponentEvents();
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        HandleNode.Size = Size;
    }

    private void BuildTimelines() {
        AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 20)
            .AddFrame(1, alpha: 255)
            .EndFrameSet()
            .BeginFrameSet(21, 30)
            .AddFrame(21, alpha: 178)
            .EndFrameSet()
            .Build()
        );

        HandleNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 9)
            .AddFrame(1, alpha: 255)
            .AddFrame(1, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(10, 19)
            .AddFrame(10, alpha: 255)
            .AddFrame(12, alpha: 255)
            .AddFrame(10, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(12, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(20, 29)
            .AddFrame(20, alpha: 255)
            .AddFrame(20, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(30, 39)
            .AddFrame(30, alpha: 255)
            .AddFrame(30, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(70, 70, 70))
            .EndFrameSet()
            .BeginFrameSet(40, 49)
            .AddFrame(40, alpha: 255)
            .AddFrame(40, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(50, 59)
            .AddFrame(50, alpha: 255)
            .AddFrame(52, alpha: 255)
            .AddFrame(50, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(52, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .Build()
        );
    }
}
