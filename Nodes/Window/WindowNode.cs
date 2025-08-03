using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Classes.TimelineBuilding;

namespace KamiToolKit.Nodes;

public unsafe class WindowNode : ComponentNode<AtkComponentWindow, AtkUldComponentDataWindow> {

    public readonly ImageNode BackgroundImageNode;
    public readonly WindowBackgroundNode BackgroundNode;
    public readonly WindowBackgroundNode BorderNode;
    public readonly TextureButtonNode CloseButtonNode;
    public readonly TextureButtonNode ConfigurationButtonNode;
    public readonly SimpleNineGridNode DividingLineNode;
    public readonly CollisionNode HeaderCollisionNode;
    public readonly ResNode HeaderContainerNode;
    public readonly TextureButtonNode InformationButtonNode;
    public readonly TextNode SubtitleNode;
    public readonly TextNode TitleNode;

    public WindowNode() {
        SetInternalComponentType(ComponentType.Window);

        CollisionNode.NodeId = 13;
        Component->ShowFlags = 1;

        HeaderCollisionNode = new CollisionNode {
            Uses = 2,
            NodeId = 12,
            Height = 28.0f,
            Position = new Vector2(8.0f, 8.0f),
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.AnchorRight | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.HasCollision | NodeFlags.RespondToMouse | NodeFlags.EmitsEvents,
        };

        HeaderCollisionNode.AttachNode(this);

        BackgroundNode = new WindowBackgroundNode(false) {
            NodeId = 11,
            Position = Vector2.Zero,
            Offsets = new Vector4(64.0f, 32.0f, 32.0f, 32.0f),
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.Fill | NodeFlags.EmitsEvents,
            PartsRenderType = 19,
        };

        BackgroundNode.AttachNode(this);

        BorderNode = new WindowBackgroundNode(true) {
            NodeId = 10,
            Position = Vector2.Zero,
            Offsets = new Vector4(64.0f, 32.0f, 32.0f, 32.0f),
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Enabled | NodeFlags.Fill | NodeFlags.EmitsEvents,
            PartsRenderType = 7,
        };

        BorderNode.AttachNode(this);

        BackgroundImageNode = new SimpleImageNode {
            NodeId = 9,
            WrapMode = 2,
            ImageNodeFlags = 0,
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.AnchorBottom | NodeFlags.AnchorRight | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            TexturePath = "ui/uld/WindowA_Gradation.tex",
            TextureCoordinates = new Vector2(6.0f, 2.0f),
            TextureSize = new Vector2(24.0f, 24.0f),
        };

        BackgroundImageNode.AttachNode(this);

        HeaderContainerNode = new ResNode {
            Size = new Vector2(477.0f, 38.0f), NodeId = 2, IsVisible = true,
        };
        HeaderContainerNode.AttachNode(this);

        DividingLineNode = new SimpleNineGridNode {
            NodeId = 8,
            TexturePath = "ui/uld/WindowA_Line.tex",
            TextureCoordinates = Vector2.Zero,
            TextureSize = new Vector2(32.0f, 4.0f),
            Size = new Vector2(650.0f, 4.0f),
            IsVisible = true,
            LeftOffset = 12.0f,
            RightOffset = 12.0f,
            Position = new Vector2(10.0f, 33.0f),
        };

        DividingLineNode.AttachNode(HeaderContainerNode);

        CloseButtonNode = new TextureButtonNode {
            NodeId = 7,
            Size = new Vector2(28.0f, 28.0f),
            Position = new Vector2(449.0f, 6.0f),
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorRight | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            TexturePath = "ui/uld/WindowA_Button.tex",
            TextureCoordinates = new Vector2(0.0f, 0.0f),
            TextureSize = new Vector2(28.0f, 28.0f),
        };

        CloseButtonNode.AttachNode(HeaderContainerNode);

        ConfigurationButtonNode = new TextureButtonNode {
            NodeId = 6,
            Size = new Vector2(16.0f, 16.0f),
            Position = new Vector2(435.0f, 8.0f),
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorRight | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            TexturePath = "ui/uld/WindowA_Button.tex",
            TextureCoordinates = new Vector2(44.0f, 0.0f),
            TextureSize = new Vector2(16.0f, 16.0f),
            IsVisible = true,
        };

        ConfigurationButtonNode.AttachNode(HeaderContainerNode);

        InformationButtonNode = new TextureButtonNode {
            NodeId = 5,
            Size = new Vector2(16.0f, 16.0f),
            Position = new Vector2(421.0f, 8.0f),
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorRight | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            TexturePath = "ui/uld/WindowA_Button.tex",
            TextureCoordinates = new Vector2(28.0f, 0.0f),
            TextureSize = new Vector2(16.0f, 16.0f),
            IsVisible = true,
        };

        InformationButtonNode.AttachNode(HeaderContainerNode);

        SubtitleNode = new TextNode {
            NodeId = 4,
            LineSpacing = 12,
            AlignmentType = AlignmentType.Left,
            FontSize = 12,
            FontType = FontType.Axis,
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            TextColor = ColorHelper.GetColor(8),
            TextOutlineColor = ColorHelper.GetColor(7),
            BackgroundColor = Vector4.Zero,
            Size = new Vector2(46.0f, 20.0f),
            Position = new Vector2(83.0f, 17.0f),
        };

        SubtitleNode.AttachNode(HeaderContainerNode);

        TitleNode = new TextNode {
            NodeId = 3,
            LineSpacing = 23,
            AlignmentType = AlignmentType.Left,
            FontSize = 23,
            FontType = FontType.TrumpGothic,
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            TextColor = ColorHelper.GetColor(2),
            TextOutlineColor = ColorHelper.GetColor(7),
            BackgroundColor = Vector4.Zero,
            Size = new Vector2(86.0f, 31.0f),
            Position = new Vector2(12.0f, 7.0f),
        };

        TitleNode.AttachNode(HeaderContainerNode);

        Data->ShowCloseButton = 1;
        Data->ShowConfigButton = 0;
        Data->ShowHelpButton = 0;
        Data->ShowHeader = 1;
        Data->Nodes[0] = TitleNode.NodeId;
        Data->Nodes[1] = SubtitleNode.NodeId;
        Data->Nodes[2] = CloseButtonNode.NodeId;
        Data->Nodes[3] = ConfigurationButtonNode.NodeId;
        Data->Nodes[4] = InformationButtonNode.NodeId;
        Data->Nodes[5] = 0;
        Data->Nodes[6] = HeaderContainerNode.NodeId;
        Data->Nodes[7] = 0;

        NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents;

        LoadTimelines();

        InitializeComponentEvents();
    }

    public AtkUnitBase* OwnerAddon {
        get => Component->OwnerUnitBase;
        set => Component->OwnerUnitBase = value;
    }

    public string Title {
        get => TitleNode.String;
        set {
            TitleNode.String = value;
            TitleNode.IsVisible = true;
        }
    }

    public string Subtitle {
        get => SubtitleNode.String;
        set {
            SubtitleNode.String = value;
            SubtitleNode.IsVisible = true;
            SubtitleNode.X = TitleNode.X + TitleNode.Width + 2.0f;
        }
    }

    public bool ShowCloseButton {
        get => CloseButtonNode.IsVisible;
        set => CloseButtonNode.IsVisible = value;
    }

    public bool ShowConfigButton {
        get => ConfigurationButtonNode.IsVisible;
        set => ConfigurationButtonNode.IsVisible = value;
    }

    public bool ShowHelpButton {
        get => InformationButtonNode.IsVisible;
        set => InformationButtonNode.IsVisible = value;
    }

    public bool ShowHeader {
        get => InformationButtonNode.IsVisible;
        set => InformationButtonNode.IsVisible = value;
    }

    public bool Focused {
        get => BorderNode.IsVisible;
        set => BorderNode.IsVisible = value;
    }

    public float HeaderHeight => HeaderContainerNode.Height;

    public Vector2 ContentSize => new(BackgroundImageNode.Width, BackgroundImageNode.Height - HeaderHeight);

    public Vector2 ContentStartPosition => new(BackgroundImageNode.X, BackgroundImageNode.Y + HeaderHeight);

    public void SetTitle(string title, string? subtitle = null)
        => Component->SetTitle(title, subtitle ?? string.Empty);

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        HeaderContainerNode.Width = Width;
        HeaderCollisionNode.Width = Width - 14.0f;
        BackgroundNode.Size = Size;
        BorderNode.Size = Size;
        BackgroundImageNode.Size = new Vector2(Width - 8.0f, Height - 16.0f);
        BackgroundImageNode.Position = new Vector2(4.0f, 4.0f);

        CloseButtonNode.X = Width - 33.0f;
        ConfigurationButtonNode.X = Width - 47.0f;
        InformationButtonNode.X = Width - 61.0f;
        DividingLineNode.Width = Width - 20.0f;

    }

    private void LoadTimelines() {
        AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 29)
            .AddLabelPair(1, 9, 17)
            .AddLabelPair(10, 19, 18)
            .AddLabelPair(20, 29, 7)
            .EndFrameSet()
            .Build());

        BackgroundNode.AddTimeline(new TimelineBuilder()
            .AddFrameSetWithFrame(1, 9, 1, multiplyColor: new Vector3(100.0f))
            .AddFrameSetWithFrame(10, 19, 10, multiplyColor: new Vector3(100.0f))
            .AddFrameSetWithFrame(20, 29, 20, multiplyColor: new Vector3(50.0f))
            .Build());

        BorderNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(10, 19)
            .AddFrame(10, alpha: 0)
            .AddFrame(12, alpha: 255)
            .EndFrameSet()
            .Build());
    }
}
