using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Nodes;

public unsafe class ScrollBarNode : ComponentNode<AtkComponentScrollBar, AtkUldComponentDataScrollBar> {

    public readonly ScrollBarBackgroundButtonNode BackgroundButtonNode;
    public readonly ScrollBarForegroundButtonNode ForegroundButtonNode;

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

    public Action<int>? OnValueChanged { get; set; }

    public NodeBase? ContentNode {
        get;
        set {
            field = value;

            if (value is not null) {
                Component->ContentNode = value;
                UpdateScrollParams();
            }
        }
    }

    public CollisionNode? ContentCollisionNode {
        get;
        set {
            field = value;
            Component->ContentCollisionNode = value is null ? null : value.Node;
            UpdateScrollParams();
        }
    }

    public int ScrollPosition {
        get => Component->ScrollPosition;
        set => Component->SetScrollPosition(value);
    }

    public int ScrollSpeed {
        get => Component->MouseWheelSpeed;
        set => Component->MouseWheelSpeed = (short)value;
    }
    
    public bool HideWhenDisabled { get; set; }

    private void UpdateHandler() {
        OnValueChanged?.Invoke(Component->PendingScrollPosition);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        BackgroundButtonNode.Size = Size;
        ForegroundButtonNode.Size = Size;
    }

    /// <summary>
    ///     Updates from attached Content and Collision nodes
    /// </summary>
    public void UpdateScrollParams() {
        if (Component->ContentNode is null) return;
        if (Component->ContentCollisionNode is null) return;

        var content = Component->ContentNode;
        var collision = Component->ContentCollisionNode;

        UpdateScrollParams(collision->Height, content->Height);
    }

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

            ContentNode?.Y = 0;
        }

        var enabledState = Component->EmptyLength is not 0;
        
        Component->SetEnabledState(enabledState);

        if (HideWhenDisabled) {
            BackgroundButtonNode.IsVisible = enabledState;
            ForegroundButtonNode.IsVisible = enabledState;
        }
    }
}
