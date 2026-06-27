using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.BaseTypes.ComponentNode;

namespace KamiToolKit.Nodes;

/// <summary>
/// Implementation of the games ScrollBarNode and associated component.
/// </summary>
public unsafe class ScrollBarNode : ComponentNode<AtkComponentScrollBar, AtkUldComponentDataScrollBar> {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ScrollBarBackgroundButtonNode BackgroundButtonNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ScrollBarForegroundButtonNode ForegroundButtonNode { get; }

    /// <summary>
    /// Event that is called when the scroll bar's scroll position is changed.
    /// </summary>
    public Action<int>? OnValueChanged { get; set; }

    /// <summary>
    /// Gets the maximum valid scroll position for the current content.
    /// </summary>
    public int ScrollMaxPosition
        => Component->ScrollMaxPosition;

    /// <summary>
    /// Gets or sets the current scroll position, triggering the component to update.
    /// </summary>
    public float ScrollPosition {
        get => Component->ScrollPosition;
        set => Component->SetScrollPosition((int) value);
    }

    /// <summary>
    /// Gets or sets the scroll speed, default is 24px per scroll.
    /// </summary>
    public int ScrollSpeed {
        get => Component->MouseWheelSpeed;
        set => Component->MouseWheelSpeed = (short)value;
    }

    /// <summary>
    /// Hides this node entirely, if the scrollbar is disabled due to content area being bigger than the scrollbar.
    /// </summary>
    public bool HideWhenDisabled { get; set; }

    /// <summary>
    /// Gets or sets whether this scrollbar is accepting mouse events.
    /// </summary>
    public bool IsAcceptingMouseWheelEvents {
        get => Component->IsAcceptingMouseWheelEvents;
        set => Component->IsAcceptingMouseWheelEvents = value;
    }

    /// <summary>
    /// Set the scrolls content and content collision node for enabling interactability.
    /// </summary>
    public void SetContentNodes(NodeBase contentNode, CollisionNode collisionNode) {
        Component->SetContentNode(contentNode, collisionNode);
        UpdateScrollParams();
    }

    /// <summary>
    /// Updates from attached Content and Collision nodes
    /// </summary>
    public void UpdateScrollParams() {
        if (Component->ContentNode is null) return;
        if (Component->ContentCollisionNode is null) return;

        UpdateScrollParams(
            Component->ContentCollisionNode->Height,
            Component->ContentNode->Height
        );
    }

    /// <summary>
    /// <inheritdoc cref="UpdateScrollParams(int, int)"/>
    /// </summary>
    public void UpdateScrollParams(float barHeight, float offscreenHeight)
        => UpdateScrollParams((int) barHeight, (int) offscreenHeight);

    /// <summary>
    /// Update the scroll bars size and positioning based on manually input values.
    /// It's recommend to use <see cref="UpdateScrollParams()"/> instead, if the content node is sized correctly.
    /// </summary>
    /// <param name="barHeight">The actual displayed height of the scrollbar</param>
    /// <param name="offScreenHeight">The actual size of the content area, this should be larger than the scrollbar.</param>
    public void UpdateScrollParams(int barHeight, int offScreenHeight) {
        var distance = offScreenHeight - barHeight;

        Component->ScrollbarLength = (short)barHeight;
        Component->ScrollMaxPosition = Math.Max(distance, 0);
        Component->ContentNodeOffScreenLength = Math.Max((short)distance, (short)0);
        Component->EmptyLength = Math.Max(barHeight - (int)((float)barHeight / offScreenHeight * barHeight), 0);
        ForegroundButtonNode.Height = barHeight - Component->EmptyLength;

        if (Component->ScrollPosition > Component->ScrollMaxPosition) {
            Component->SetScrollPosition(Component->ScrollMaxPosition);
        }

        if (Component->EmptyLength is 0) {
            ForegroundButtonNode.Y = 0.0f;

            if (Component->ContentNode is not null) {
                Component->ContentNode->Y = 0;
            }
        }

        var enabledState = Component->EmptyLength is not 0;

        Component->SetEnabledState(enabledState);

        if (HideWhenDisabled) {
            BackgroundButtonNode.IsVisible = enabledState;
            ForegroundButtonNode.IsVisible = enabledState;
        }
    }

    /// <summary>
    /// Constructs a new <see cref="ScrollBarNode"/>.
    /// </summary>
    public ScrollBarNode() {
        SetInternalComponentType(ComponentType.ScrollBar);

        BackgroundButtonNode = new ScrollBarBackgroundButtonNode {
            Size = new Vector2(8.0f, 306.0f),
        };
        BackgroundButtonNode.AttachNode(this);

        ForegroundButtonNode = new ScrollBarForegroundButtonNode {
            Size = new Vector2(8.0f, 306.0f),
        };
        ForegroundButtonNode.AttachNode(this);

        Data->Nodes[0] = ForegroundButtonNode.NodeId;
        Data->Nodes[1] = 0; // Arrow Up Button
        Data->Nodes[2] = 0; // Arrow Down Button
        Data->Nodes[3] = BackgroundButtonNode.NodeId;

        Data->Vertical = 1;
        Data->Margin = 0;

        InitializeComponentEvents();

        Component->MouseDownScreenPos = 0;
        Component->MouseWheelSpeed = 24;

        AddEvent(AtkEventType.ValueUpdate, UpdateHandler);
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        BackgroundButtonNode.Size = Size;
        ForegroundButtonNode.Size = Size;
    }

    private void UpdateHandler()
        => OnValueChanged?.Invoke(Component->PendingScrollPosition);
}
