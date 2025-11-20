using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.Timelines;
using KamiToolKit.Premade.Nodes;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

public unsafe class ColorOptionTextButtonNode : ButtonBase {

    public readonly NineGridNode BackgroundNode;
    public readonly TextNode LabelNode;
    public readonly ColorPreviewNode ColorNode;

    public ColorOptionTextButtonNode() {
        BackgroundNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/ButtonA.tex",
            TextureSize = new Vector2(100.0f, 28.0f),
            LeftOffset = 16.0f,
            RightOffset = 16.0f,
        };
        BackgroundNode.AttachNode(this);

        ColorNode = new ColorPreviewNode {
            DisableCollisionNode = true,
        };
        ColorNode.AttachNode(this);
        
        LabelNode = new TextNode {
            AlignmentType = AlignmentType.Center, 
            Position = new Vector2(16.0f, 3.0f), 
        };

        LabelNode.AttachNode(this);

        LoadTimelines();

        Data->Nodes[0] = LabelNode.NodeId;
        Data->Nodes[1] = BackgroundNode.NodeId;

        InitializeComponentEvents();
    }

    public ReadOnlySeString SeString {
        get => LabelNode.SeString;
        set => LabelNode.SeString = value;
    }

    public string String {
        get => LabelNode.String;
        set => LabelNode.String = value;
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        LabelNode.Size = new Vector2(Width - 32.0f, Height - 8.0f);
        BackgroundNode.Size = Size;
        ColorNode.Size = new Vector2(17.0f, 17.0f);
    }

    public ColorHelpers.HsvaColor? DefaultHsvaColor {
        get => ColorNode.HsvaColor;
        set => ColorNode.HsvaColor = value ?? default;
    }

    public Vector4? DefaultColor {
        get => ColorNode.Color;
        set => ColorNode.Color = value ?? default;
    }

    private void LoadTimelines() {
        var foregroundPositionOffset = new Vector2(24.0f, 3.0f);
        var colorElementPositionOffset = new Vector2(16.0f, 2.0f);
        
        AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 53)
            .AddLabelPair(1, 10, 1)
            .AddLabelPair(11, 17, 2)
            .AddLabelPair(18, 26, 3)
            .AddLabelPair(27, 36, 7)
            .AddLabelPair(37, 46, 6)
            .AddLabelPair(47, 53, 4)
            .EndFrameSet()
            .Build());

        BackgroundNode.AddTimeline(new TimelineBuilder()
            .AddFrameSetWithFrame(1, 10, 1, Vector2.Zero, 255, multiplyColor: new Vector3(100.0f))
            .BeginFrameSet(11, 17)
            .AddFrame(11, Vector2.Zero, 255, multiplyColor: new Vector3(100.0f))
            .AddFrame(13, Vector2.Zero, 255, multiplyColor: new Vector3(100.0f), addColor: new Vector3(16.0f))
            .EndFrameSet()
            .AddFrameSetWithFrame(18, 26, 18, new Vector2(0.0f, 1.0f), 255, new Vector3(16.0f))
            .AddFrameSetWithFrame(27, 36, 27, Vector2.Zero, 178, multiplyColor: new Vector3(50.0f))
            .AddFrameSetWithFrame(37, 46, 37, Vector2.Zero, 255, multiplyColor: new Vector3(100.0f), addColor: new Vector3(16.0f))
            .BeginFrameSet(47, 53)
            .AddFrame(47, Vector2.Zero, 255, multiplyColor: new Vector3(100.0f), addColor: new Vector3(16.0f))
            .AddFrame(53, Vector2.Zero, 255, multiplyColor: new Vector3(100.0f))
            .EndFrameSet()
            .Build());

        ColorNode.AddTimeline(new TimelineBuilder()
            .AddFrameSetWithFrame(1, 10, 1, colorElementPositionOffset, 255, multiplyColor: new Vector3(100.0f))
            .AddFrameSetWithFrame(11, 17, 11, colorElementPositionOffset, 255, multiplyColor: new Vector3(100.0f))
            .AddFrameSetWithFrame(18, 26, 18, colorElementPositionOffset + new Vector2(0.0f, 1.0f), 255, multiplyColor: new Vector3(100.0f))
            .AddFrameSetWithFrame(27, 36, 27, colorElementPositionOffset, 153, multiplyColor: new Vector3(80.0f))
            .AddFrameSetWithFrame(37, 46, 37, colorElementPositionOffset, 255, multiplyColor: new Vector3(100.0f))
            .AddFrameSetWithFrame(47, 53, 47, colorElementPositionOffset, 255, multiplyColor: new Vector3(100.0f))
            .Build());
        
        LabelNode.AddTimeline(new TimelineBuilder()
            .AddFrameSetWithFrame(1, 10, 1, foregroundPositionOffset, 255, multiplyColor: new Vector3(100.0f))
            .AddFrameSetWithFrame(11, 17, 11, foregroundPositionOffset, 255, multiplyColor: new Vector3(100.0f))
            .AddFrameSetWithFrame(18, 26, 18, foregroundPositionOffset + new Vector2(0.0f, 1.0f), 255, multiplyColor: new Vector3(100.0f))
            .AddFrameSetWithFrame(27, 36, 27, foregroundPositionOffset, 153, multiplyColor: new Vector3(80.0f))
            .AddFrameSetWithFrame(37, 46, 37, foregroundPositionOffset, 255, multiplyColor: new Vector3(100.0f))
            .AddFrameSetWithFrame(47, 53, 47, foregroundPositionOffset, 255, multiplyColor: new Vector3(100.0f))
            .Build());
    }
}
