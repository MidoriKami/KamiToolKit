using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.TimelineBuilding;

namespace KamiToolKit.Nodes.Slider;

public unsafe class SliderBackgroundButtonNode : ComponentNode<AtkComponentButton, AtkUldComponentDataButton> {

    public readonly NineGridNode BackgroundTexture;

    public SliderBackgroundButtonNode() {
        SetInternalComponentType(ComponentType.Button);

        BackgroundTexture = new SimpleNineGridNode {
            NodeId = 2,
            TexturePath = "ui/uld/SliderGaugeHorizontalA.tex",
            TextureCoordinates = new Vector2(16.0f, 0.0f),
            TextureSize = new Vector2(40.0f, 8.0f),
            LeftOffset = 8,
            RightOffset = 8,
            IsVisible = true,
        };
        BackgroundTexture.AttachNode(this);

        Component->ButtonBGNode = BackgroundTexture.InternalResNode;

        Data->Nodes[0] = 0;
        Data->Nodes[1] = BackgroundTexture.NodeId;

        BuildTimelines();

        InitializeComponentEvents();
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        BackgroundTexture.Size = new Vector2(Width, Height / 2.0f);
        BackgroundTexture.Y = Height / 4.0f;
    }

    private void BuildTimelines() {
        AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 20)
            .AddFrame(1, alpha: 255)
            .EndFrameSet()
            .BeginFrameSet(21, 30)
            .AddFrame(21, alpha: 127)
            .EndFrameSet()
            .Build()
        );

        BackgroundTexture.AddTimeline(new TimelineBuilder()
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
            .AddFrame(30, alpha: 178)
            .AddFrame(30, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(50, 50, 50))
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
