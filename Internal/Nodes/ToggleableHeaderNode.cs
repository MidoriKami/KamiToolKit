using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Interfaces;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Simplified;
using KamiToolKit.Timelines;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Internal.Nodes;

/// <summary>
/// Node for use with <see cref="CollapsingHeaderNode"/> and <see cref="TreeListNode{T,TU}"/>, to represent a clickable header node.
/// </summary>
internal class ToggleableHeaderNode : SimpleComponentNode, IToggleableHeaderNode {

    /// <summary>
    /// Gets the node used to display the body texture of this node.
    /// </summary>
    public SimpleNineGridNode ButtonTextureNode { get; }

    /// <summary>
    /// Gets the text node used to display this buttons label.
    /// </summary>
    public TextNode LabelTextNode { get; }

    /// <summary>
    /// Gets the image node used to display the collapsed/uncollapsed arrow node.
    /// </summary>
    public ImageNode ToggleArrowImageNode { get; }

    /// <inheritdoc />
    public bool IsCollapsed {
        get;
        set {
            if (field == value) return;

            field = value;
            if (value) {
                Collapse();
            }
            else {
                Uncollapse();
            }
        }
    }

    /// <inheritdoc />
    public ReadOnlySeString String {
        get => LabelTextNode.String;
        set => LabelTextNode.String = value;
    }

    /// <inheritdoc />
    public Vector4 TextColor {
        get => LabelTextNode.TextColor;
        set => LabelTextNode.TextColor = value;
    }

    /// <inheritdoc />
    public Vector4 TextOutlineColor {
        get => LabelTextNode.TextOutlineColor;
        set => LabelTextNode.TextOutlineColor = value;
    }

    /// <inheritdoc />
    public AlignmentType AlignmentType {
        get => LabelTextNode.AlignmentType;
        set => LabelTextNode.AlignmentType = value;
    }

    /// <inheritdoc />
    public FontType FontType {
        get => LabelTextNode.FontType;
        set => LabelTextNode.FontType = value;
    }

    /// <inheritdoc />
    public TextFlags TextFlags {
        get => LabelTextNode.TextFlags;
        set => LabelTextNode.TextFlags = value;
    }

    /// <inheritdoc />
    public uint FontSize {
        get => LabelTextNode.FontSize;
        set => LabelTextNode.FontSize = value;
    }

    /// <inheritdoc />
    public Action? OnCollapsed { get; set; }

    /// <inheritdoc />
    public Action? OnUncollapsed { get; set; }

    /// <inheritdoc />
    public Action<bool>? OnCollapseToggled { get; set; }

    /// <inheritdoc />
    public void Collapse(bool playSoundEffect = true) {
        if (!IsEnabled) return;

        ToggleArrowImageNode.PartId = 1;

        OnCollapsed?.Invoke();
        OnCollapseToggled?.Invoke(false);
    }

    /// <inheritdoc />
    public void Uncollapse(bool playSoundEffect = true) {
        if (!IsEnabled) return;

        ToggleArrowImageNode.PartId = 0;

        OnUncollapsed?.Invoke();
        OnCollapseToggled?.Invoke(true);
    }

    /// <inheritdoc />
    public void Toggle(bool playSoundEffect = true) {
        if (IsCollapsed) {
            Uncollapse(playSoundEffect);
        }
        else {
            Collapse(playSoundEffect);
        }
    }

    public unsafe ToggleableHeaderNode() {
        CollisionNode.ShowClickableCursor = true;

        ButtonTextureNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/ListItemB.tex",
            TextureSize = new Vector2(48.0f, 28.0f),
            TextureCoordinates = new Vector2(0.0f, 24.0f),
            NodeFlags = NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.Fill |
                        NodeFlags.HasCollision | NodeFlags.RespondToMouse | NodeFlags.Focusable,
            TopOffset = 10,
            BottomOffset = 12,
            LeftOffset = 12,
            RightOffset = 12,
        };
        ButtonTextureNode.AttachNode(this);

        LabelTextNode = new TextNode {
            NodeFlags = NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled,
            TextColor = ColorHelper.GetColor(1),
            TextOutlineColor = ColorHelper.GetColor(7),
            FontSize = 14,
            LineSpacing = 14,
            Alpha = 229.0f / 255.0f,
        };
        LabelTextNode.AttachNode(this);

        ToggleArrowImageNode = new ImageNode {
            PartId = 0,
        };
        ToggleArrowImageNode.AddPart([
            new Part { TexturePath = "ui/uld/ListItemB.tex", TextureCoordinates = new Vector2(24.0f, 0.0f), Size = new Vector2(24.0f, 24.0f) },
            new Part { TexturePath = "ui/uld/ListItemB.tex", TextureCoordinates = new Vector2(0.0f, 0.0f), Size = new Vector2(24.0f, 24.0f) },
        ]);
        ToggleArrowImageNode.AttachNode(this);

        CollisionNode.AddEvent(AtkEventType.MouseOver, OnEvent);
        CollisionNode.AddEvent(AtkEventType.MouseOut, OnEvent);
        CollisionNode.AddEvent(AtkEventType.MouseDown, OnEvent);
        CollisionNode.AddEvent(AtkEventType.MouseUp, OnEvent);

        AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 119)
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
            .AddLabel(119, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .EndFrameSet()
            .Build()
        );

        ButtonTextureNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 9)
            .AddFrame(1, alpha: 255)
            .AddFrame(1, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(10, 19)
            .AddFrame(10, alpha: 255, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(12, alpha: 255, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(20, 29)
            .AddFrame(20, alpha: 255, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(30, 39)
            .AddFrame(30, alpha: 178, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(50, 50, 50))
            .EndFrameSet()
            .BeginFrameSet(40, 49)
            .AddFrame(40, alpha: 255, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(50, 59)
            .AddFrame(50, alpha: 255, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(52, alpha: 255, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(60, 69)
            .AddFrame(60, alpha: 255, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(70, 79)
            .AddFrame(70, alpha: 255, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(72, alpha: 255, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(80, 89)
            .AddFrame(80, alpha: 255, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(90, 99)
            .AddFrame(90, alpha: 178, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(50, 50, 50))
            .EndFrameSet()
            .BeginFrameSet(100, 109)
            .AddFrame(100, alpha: 255, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(110, 119)
            .AddFrame(110, alpha: 255, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(112, alpha: 255, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .Build()
        );

        InitializeComponentEvents();
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        CollisionNode.Size = new Vector2(Width, 28.0f);
        CollisionNode.Position = new Vector2(0.0f, 0.0f);

        ButtonTextureNode.Size = new Vector2(Width, 28.0f);
        ButtonTextureNode.Position = new Vector2(0.0f, 0.0f);

        ToggleArrowImageNode.Size = new Vector2(24.0f, 24.0f);
        ToggleArrowImageNode.Position = new Vector2(0.0f, 1.0f);

        LabelTextNode.Size = new Vector2(Width - ToggleArrowImageNode.Width, 28.0f);
        LabelTextNode.Position = new Vector2(ToggleArrowImageNode.Bounds.Right, 0.0f);
    }

    private unsafe void OnEvent(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        switch (eventType) {
            case AtkEventType.MouseOver:
                Timeline?.PlayAnimation(2);
                break;

            case AtkEventType.MouseOut:
                Timeline?.PlayAnimation(1);
                break;

            case AtkEventType.MouseDown when IsCollapsed:
                Timeline?.PlayAnimation(3);
                break;

            case AtkEventType.MouseUp when IsCollapsed:
                IsCollapsed = false;
                break;

            case AtkEventType.MouseDown when !IsCollapsed:
                Timeline?.PlayAnimation(10);
                break;

            case AtkEventType.MouseUp when !IsCollapsed:
                IsCollapsed = true;
                break;
        }
    }
}
