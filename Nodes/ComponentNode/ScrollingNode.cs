using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Enums;
using KamiToolKit.Interfaces;

namespace KamiToolKit.Nodes;

/// <summary>
/// Generic class for making any node type scrollable.
/// Access the inner node via ContentNode
/// </summary>
/// <remarks>
/// Property initializers for ContentNode must be set before Size is set, or you'll have to invoke RecalculateSizes().
/// </remarks>
public class ScrollingNode<T> : ResNode, IScrollingNode<T> where T : NodeBase, new() {

    private const float ScrollBarThickness = 8.0f;

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public CollisionNode ScrollingCollisionNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ResNode ClippingContentNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ScrollBarNode ScrollBarNode { get; }

    /// <inheritdoc />
    public T ContentNode { get; }

    /// <inheritdoc />
    public LayoutFill LayoutFill { get; set; } = LayoutFill.Both;

    /// <inheritdoc />
    public bool AutoHideScrollBar {
        get => ScrollBarNode.HideWhenDisabled;
        set => ScrollBarNode.HideWhenDisabled = value;
    }

    /// <inheritdoc />
    public int ScrollSpeed {
        get => ScrollBarNode.ScrollSpeed;
        set => ScrollBarNode.ScrollSpeed = value;
    }

    /// <inheritdoc />
    public bool IsHorizontal {
        get => ScrollBarNode.IsHorizontal;
        set {
            if (ScrollBarNode.IsHorizontal == value) return;

            ScrollBarNode.IsHorizontal = value;
            OnSizeChanged();
        }
    }

    /// <inheritdoc />
    public void ScrollToStart() {
        ScrollBarNode.ScrollPosition = 0;
    }

    /// <inheritdoc />
    public void ScrollToEnd() {
        ScrollBarNode.ScrollPosition = ScrollBarNode.ScrollMaxPosition;
    }

    /// <inheritdoc />
    public void RecalculateLayout() {
        OnSizeChanged();
    }

    /// <summary>
    /// Constructs a new <see cref="ScrollingNode{T}"/>.
    /// </summary>
    public ScrollingNode() : this(new T
        {
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft |
                        NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
        })
    { }

    /// <summary>
    /// Constructs a new <see cref="ScrollingNode{T}"/>, with provided prebuilt node.
    /// </summary>
    public unsafe ScrollingNode(T prebuiltContentNode) {
        ClippingContentNode = new ResNode {
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft |
                        NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.Clip | NodeFlags.EmitsEvents,
        };
        ClippingContentNode.AttachNode(this);

        ScrollingCollisionNode = new CollisionNode {
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft |
                        NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.HasCollision | NodeFlags.RespondToMouse | NodeFlags.EmitsEvents,
        };
        ScrollingCollisionNode.AttachNode(ClippingContentNode);

        ContentNode = prebuiltContentNode;
        ContentNode.AttachNode(ClippingContentNode);

        ScrollBarNode = new ScrollBarNode {
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.AnchorBottom |
                        NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.RespondToMouse | NodeFlags.EmitsEvents,
            IsAcceptingMouseWheelEvents = true,
        };
        ScrollBarNode.AttachNode(this);

        ClippingContentNode.AddLinkedEvent(ScrollBarNode, 5, AtkEventType.MouseWheel);
        ScrollingCollisionNode.AddLinkedEvent(ScrollBarNode, 5, AtkEventType.MouseWheel);
        ContentNode.AddLinkedEvent(ScrollBarNode, 5, AtkEventType.MouseWheel);
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        ClippingContentNode.Size = Size;
        ScrollingCollisionNode.Size = Size;

        var oldPosition = ScrollBarNode.ScrollPosition;
        ScrollBarNode.ScrollPosition = 0;
        if (IsHorizontal) {
            ScrollBarNode.Size = new Vector2(Width, ScrollBarThickness);
            ScrollBarNode.Position = new Vector2(0.0f, Height - ScrollBarThickness);
        }
        else {
            ScrollBarNode.Size = new Vector2(ScrollBarThickness, Height);
            ScrollBarNode.Position = new Vector2(Width - ScrollBarThickness, 0.0f);
        }

        ScrollBarNode.SetContentNodes(ContentNode, ScrollingCollisionNode);
        ScrollBarNode.ScrollPosition = Math.Clamp(oldPosition, 0, ScrollBarNode.ScrollMaxPosition);

        if (IsHorizontal) {
            ContentNode.Height = Height;
        }
        else {
            if (ScrollBarNode.IsVisible) {
                ContentNode.Width = Width - ScrollBarNode.Width - 2.0f;
            }
            else {
                ContentNode.Width = Width;
            }
        }

        if (ContentNode is ILayoutUpdatable layoutNode) {
            layoutNode.RecalculateLayout();
        }
    }
}
