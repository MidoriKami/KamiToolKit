using System;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Addon.Events.EventDataTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.System;

namespace KamiToolKit.Nodes;

public unsafe class ScrollBarNode : ComponentNode<AtkComponentScrollBar, AtkUldComponentDataScrollBar> {

    public readonly ScrollBarBackgroundButtonNode BackgroundButtonNode;
    public readonly ScrollBarForegroundButtonNode ForegroundButtonNode;

    public ScrollBarNode() {
        SetInternalComponentType(ComponentType.ScrollBar);

        BackgroundButtonNode = new ScrollBarBackgroundButtonNode {
            NodeId = 3, Size = new Vector2(8.0f, 306.0f), IsVisible = true,
        };

        BackgroundButtonNode.AttachNode(this);

        ForegroundButtonNode = new ScrollBarForegroundButtonNode {
            NodeId = 2, Size = new Vector2(8.0f, 306.0f), IsVisible = true,
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

        AddEvent(AddonEventType.ValueUpdate, UpdateHandler);
    }

    public Action<int>? OnValueChanged { get; set; }

    public NodeBase? ContentNode {
        get;
        set {
            field = value;
            Component->ContentNode = value is null ? null : value.InternalResNode;
            UpdateScrollParams();
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

    private void UpdateHandler(AddonEventData obj) {
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

            if (ContentNode is not null) {
                ContentNode.Y = 0;
            }
        }

        Component->SetEnabledState(Component->EmptyLength is not 0);
    }
}
