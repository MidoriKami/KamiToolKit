using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Interfaces;

namespace KamiToolKit.Nodes;

/// <summary>
/// Generic class for making any node type scrollable.
/// Access the inner node via ContentNode
/// </summary>
/// <remarks>
/// Property initializers for ContentNode must be set before Size is set, or you'll have to invoke RecalculateSizes().
/// </remarks>
public class ScrollingNode<T> : ResNode where T : NodeBase, new() {

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

    /// <summary>
    /// The contained node for the ScrollingNode.
    /// </summary>
    public T ContentNode { get; }

    /// <summary>
    /// Hides the Scroll Bar Node when its disabled due to the content being larger than the scroll.
    /// </summary>
    public bool AutoHideScrollBar {
        get => ScrollBarNode.HideWhenDisabled;
        set => ScrollBarNode.HideWhenDisabled = value;
    }

    /// <summary>
    /// Set the scroll speed, default 24px per scroll.
    /// </summary>
    public int ScrollSpeed {
        get => ScrollBarNode.ScrollSpeed;
        set => ScrollBarNode.ScrollSpeed = value;
    }

    /// <summary>
    /// Gets or sets whether this scrolling node scrolls horizontally.
    /// </summary>
    public bool IsHorizontalMode {
        get => ScrollBarNode.IsHorizontalMode;
        set {
            if (ScrollBarNode.IsHorizontalMode == value) return;

            ScrollBarNode.IsHorizontalMode = value;
            OnSizeChanged();
        }
    }

    /// <summary>
    /// Sets the scroll position to the start.
    /// </summary>
    public void ScrollToStart() {
        ScrollBarNode.ScrollPosition = 0;
    }

    /// <summary>
    /// Sets the scroll position to the end.
    /// </summary>
    public void ScrollToEnd() {
        ScrollBarNode.ScrollPosition = ScrollBarNode.ScrollMaxPosition;
    }

    /// <summary>
    /// Recalculates sizes to update scroll params correctly.
    /// </summary>
    public void RecalculateSizes() {
        OnSizeChanged();
    }

    /// <summary>
    /// Constructs a new <see cref="ScrollingNode{T}"/>.
    /// </summary>
    public unsafe ScrollingNode() {
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

        ContentNode = new T {
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft |
                        NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
        };
        ContentNode.AttachNode(ClippingContentNode);

        ScrollBarNode = new ScrollBarNode {
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.AnchorBottom |
                        NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.RespondToMouse | NodeFlags.EmitsEvents,
            IsAcceptingMouseWheelEvents = true,
        };
        ScrollBarNode.AttachNode(this);

        ClippingContentNode.ResNode->AtkEventManager.RegisterEvent(
            AtkEventType.MouseWheel,
            5,
            null,
            ScrollingCollisionNode,
            ScrollBarNode,
            false
        );

        ScrollingCollisionNode.ResNode->AtkEventManager.RegisterEvent(
            AtkEventType.MouseWheel,
            5,
            null,
            ScrollingCollisionNode,
            ScrollBarNode,
            false
        );

        ContentNode.ResNode->AtkEventManager.RegisterEvent(
            AtkEventType.MouseWheel,
            5,
            null,
            ScrollingCollisionNode,
            ScrollBarNode,
            false
        );
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        ClippingContentNode.Size = Size;
        ScrollingCollisionNode.Size = Size;

        var oldPosition = ScrollBarNode.ScrollPosition;
        ScrollBarNode.ScrollPosition = 0;
        if (IsHorizontalMode) {
            ScrollBarNode.Size = new Vector2(Width, ScrollBarThickness);
            ScrollBarNode.Position = new Vector2(0.0f, Height - ScrollBarThickness);
        }
        else {
            ScrollBarNode.Size = new Vector2(ScrollBarThickness, Height);
            ScrollBarNode.Position = new Vector2(Width - ScrollBarThickness, 0.0f);
        }

        ScrollBarNode.SetContentNodes(ContentNode, ScrollingCollisionNode);
        ScrollBarNode.ScrollPosition = Math.Clamp(oldPosition, 0, ScrollBarNode.ScrollMaxPosition);

        if (IsHorizontalMode) {
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

        if (ContentNode is ILayoutListNode layoutNode) {
            layoutNode.RecalculateLayout();
        }
    }
}
