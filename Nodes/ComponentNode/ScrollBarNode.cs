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
    /// Gets or sets whether this scrollbar scrolls horizontally.
    /// </summary>
    public bool IsHorizontalMode {
        get => !Component->IsVertical;
        set {
            Data->Vertical = (byte)(value ? 0 : 1);

            Component->IsVertical = !value;
            Component->IsInputVertical = !value;
            // TODO: Swap when merged into Dalamud main (was UnkIsVertical)
            // This controls whether auto-resizing changes thumb height vs width.
            //Component->IsThumbVertical = !value;
            ((byte*)Component)[0x136] = (byte)(value ? 0 : 1);

            ForegroundButtonNode.IsHorizontalMode = value;
            UpdateScrollParams();
        }
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
            IsHorizontalMode ? Component->ContentCollisionNode->Width : Component->ContentCollisionNode->Height,
            IsHorizontalMode ? Component->ContentNode->Width : Component->ContentNode->Height
        );
    }

    /// <summary>
    /// <inheritdoc cref="UpdateScrollParams(int, int)"/>
    /// </summary>
    public void UpdateScrollParams(float barLength, float offScreenLength)
        => UpdateScrollParams((int) barLength, (int) offScreenLength);

    /// <summary>
    /// Update the scroll bars size and positioning based on manually input values.
    /// It's recommend to use <see cref="UpdateScrollParams()"/> instead, if the content node is sized correctly.
    /// </summary>
    /// <param name="barLength">The actual displayed length of the scrollbar</param>
    /// <param name="offScreenLength">The actual size of the content area, this should be larger than the scrollbar.</param>
    public void UpdateScrollParams(int barLength, int offScreenLength) {
        if (barLength <= 0 || offScreenLength <= 0) {
            Component->ScrollbarLength = (short)Math.Max(barLength, 0);
            Component->ScrollMaxPosition = 0;
            Component->ContentNodeOffScreenLength = 0;
            Component->EmptyLength = 0;
            ForegroundButtonNode.Position = Vector2.Zero;
            UpdateChildVisibility(false);
            return;
        }

        var distance = offScreenLength - barLength;

        Component->ScrollbarLength = (short)barLength;
        Component->ScrollMaxPosition = Math.Max(distance, 0);
        Component->ContentNodeOffScreenLength = Math.Max((short)distance, (short)0);
        Component->EmptyLength = Math.Max(barLength - (int)((float)barLength / offScreenLength * barLength), 0);

        if (IsHorizontalMode) {
            ForegroundButtonNode.Width = barLength - Component->EmptyLength;
        }
        else {
            ForegroundButtonNode.Height = barLength - Component->EmptyLength;
        }

        if (Component->ScrollPosition > Component->ScrollMaxPosition) {
            Component->SetScrollPosition(Component->ScrollMaxPosition);
        }

        if (Component->EmptyLength is 0) {
            if (IsHorizontalMode) {
                ForegroundButtonNode.X = 0.0f;

                if (Component->ContentNode is not null) {
                    Component->ContentNode->X = 0;
                }
            }
            else {
                ForegroundButtonNode.Y = 0.0f;

                if (Component->ContentNode is not null) {
                    Component->ContentNode->Y = 0;
                }
            }
        }

        var enabledState = Component->EmptyLength is not 0;
        if (IsEnabled != enabledState) {
            Component->SetEnabledState(enabledState);
        }

        UpdateChildVisibility(enabledState);
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
        ForegroundButtonNode.IsHorizontalMode = IsHorizontalMode;
        ForegroundButtonNode.Size = Size;
    }

    private void UpdateHandler()
        => OnValueChanged?.Invoke(Component->PendingScrollPosition);

    private void UpdateChildVisibility(bool enabledState) {
        var isVisible = !HideWhenDisabled || enabledState;

        IsVisible = isVisible;
        BackgroundButtonNode.IsVisible = isVisible;
        ForegroundButtonNode.IsVisible = isVisible;
        ForegroundButtonNode.ButtonTexture.IsVisible = isVisible;
    }
}
