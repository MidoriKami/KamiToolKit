using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes.Simplified;
using KamiToolKit.Timelines;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// Layout node representing a collapsing header that will collapse/hide its contained nodes.
/// </summary>
public class CollapsingHeaderNode : LayoutListNode {

    /// <summary>
    /// Gets or sets the nodes collapsed state.
    /// </summary>
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

    /// <summary>
    /// Gets or sets whether this node should respond to clicks.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets whether contained nodes will be resized to fit this nodes width.
    /// </summary>
    public bool FitWidth { get; set; }

    /// <summary>
    /// Gets or sets the displayed string.
    /// </summary>
    public ReadOnlySeString String {
        get => LabelTextNode.String;
        set => LabelTextNode.String = value;
    }

    /// <summary>
    /// Action that is invoked when the header node is collapsed.
    /// </summary>
    public Action? OnCollapse { get; set; }

    /// <summary>
    /// Action that is invoked when the header node is uncollapsed.
    /// </summary>
    public Action? OnUncollapse { get; set; }

    /// <summary>
    /// Action that is invoked when the header is either collapsed or uncollapsed.
    /// </summary>
    public Action? OnToggle { get; set; }

    /// <summary>
    /// Gets the node used for event collision.
    /// </summary>
    public CollisionNode CollisionNode { get; }

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

    /// <summary>
    /// Constructs a new <see cref="CollapsingHeaderNode"/>
    /// </summary>
    public unsafe CollapsingHeaderNode() {
        CollisionNode = new CollisionNode {
            NodeFlags = NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.HasCollision |
                        NodeFlags.RespondToMouse | NodeFlags.Focusable | NodeFlags.EmitsEvents,
            ShowClickableCursor = true,
        };
        CollisionNode.AttachNode(this);

        ButtonTextureNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/ListItemB.tex",
            TextureSize = new Vector2(48.0f, 28.0f),
            TextureCoordinates = new Vector2(0.0f, 24.0f),
            NodeFlags = NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.Fill | NodeFlags.HasCollision | NodeFlags.RespondToMouse | NodeFlags.Focusable,
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
                Uncollapse();
                break;

            case AtkEventType.MouseDown when !IsCollapsed:
                Timeline?.PlayAnimation(10);
                break;

            case AtkEventType.MouseUp when !IsCollapsed:
                IsCollapsed = true;
                Collapse();
                break;
        }
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

    /// <inheritdoc />
    protected override void OnRecalculateLayout() {
        var yPosition = CollisionNode.Height + FirstItemSpacing;

        foreach (var node in Nodes) {
            node.IsVisible = !IsCollapsed;
            node.Y = yPosition;

            yPosition += node.Height + ItemSpacing;

            if (FitWidth) {
                node.Width = Width;
            }
        }

        Height = yPosition;
    }

    /// <inheritdoc />
    protected override void OnRecalculateNavigation() {
        // Not implemented yet.
    }

    private void Collapse() {
        foreach (var node in Nodes) {
            node.IsVisible = false;
        }

        Height = 28.0f;
        ToggleArrowImageNode.PartId = 1;

        OnCollapse?.Invoke();
        OnToggle?.Invoke();
    }

    private void Uncollapse() {
        foreach (var node in Nodes) {
            node.IsVisible = true;
        }

        ToggleArrowImageNode.PartId = 0;
        RecalculateLayout();

        OnUncollapse?.Invoke();
        OnToggle?.Invoke();
    }
}
