using System;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

internal unsafe class NodeEditOverlayNode : SimpleComponentNode {

    private HorizontalResizeNineGridNode bottomEditNode;
    private ResizeButtonNode leftCornerEditNode;
    private VerticalResizeNineGridNode leftEditNode;
    private ResizeButtonNode rightCornerEditNode;
    private VerticalResizeNineGridNode rightEditNode;
    private HorizontalResizeNineGridNode topEditNode;

    public NodeEditOverlayNode() {
        rightEditNode = new VerticalResizeNineGridNode {
            Position = new Vector2(0.0f, 16.0f), Size = new Vector2(8.0f, 0.0f), Rotation = 1 * MathF.PI / 2.0f, IsVisible = true,
        };
        rightEditNode.AttachNode(this);

        bottomEditNode = new HorizontalResizeNineGridNode {
            Position = new Vector2(14.0f, 0.0f), Size = new Vector2(0.0f, 8.0f), IsVisible = true,
        };
        bottomEditNode.AttachNode(this);

        leftEditNode = new VerticalResizeNineGridNode {
            Position = new Vector2(18.0f, 16.0f), Size = new Vector2(8.0f, 0.0f), Rotation = 1 * MathF.PI / 2.0f, IsVisible = true,
        };
        leftEditNode.AttachNode(this);

        topEditNode = new HorizontalResizeNineGridNode {
            Position = new Vector2(14.0f, 12.0f), Size = new Vector2(0.0f, 8.0f), IsVisible = true,
        };
        topEditNode.AttachNode(this);

        rightCornerEditNode = new ResizeButtonNode(ResizeDirection.BottomRight) {
            Size = new Vector2(28.0f, 28.0f), Origin = new Vector2(14.0f, 14.0f), IsVisible = true, EnableEventFlags = true,
        };
        rightCornerEditNode.AttachNode(this);

        leftCornerEditNode = new ResizeButtonNode(ResizeDirection.BottomLeft) {
            Position = new Vector2(16.0f, 0.0f), Size = new Vector2(28.0f, 28.0f), Origin = new Vector2(14.0f, 14.0f), IsVisible = true,
        };
        leftCornerEditNode.AttachNode(this);
    }

    public bool ShowParts {
        get;
        set {
            field = value;
            rightEditNode.IsVisible = value;
            bottomEditNode.IsVisible = value;
            leftEditNode.IsVisible = value;
            topEditNode.IsVisible = value;
            rightCornerEditNode.IsVisible = value;
            leftCornerEditNode.IsVisible = value;
        }
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        rightEditNode.X = Width - 16.0f;
        bottomEditNode.Width = Width - 28.0f;
        topEditNode.Width = Width - 28.0f;

        rightEditNode.Height = Height - 32.0f;
        bottomEditNode.Y = Height - 22.0f;
        leftEditNode.Height = Height - 32.0f;
        leftCornerEditNode.Y = Height - 44.0f;

        rightCornerEditNode.Position = Size - new Vector2(44.0f, 44.0f);
    }

    public Vector2 GetSizeDelta(Vector2 mouseDelta) {
        if (leftEditNode.IsHovered) return new Vector2(-mouseDelta.X, 0.0f);
        if (rightEditNode.IsHovered) return new Vector2(mouseDelta.X, 0.0f);
        if (topEditNode.IsHovered) return new Vector2(0.0f, -mouseDelta.Y);
        if (bottomEditNode.IsHovered) return new Vector2(0.0f, mouseDelta.Y);
        if (rightCornerEditNode.IsHovered) return mouseDelta;
        if (leftCornerEditNode.IsHovered) return new Vector2(-mouseDelta.X, mouseDelta.Y);

        return Vector2.Zero;
    }

    public Vector2 GetPositionDelta(Vector2 mouseDelta) {
        if (leftEditNode.IsHovered) return new Vector2(mouseDelta.X, 0.0f);
        if (topEditNode.IsHovered) return new Vector2(0.0f, mouseDelta.Y);
        if (leftCornerEditNode.IsHovered) return new Vector2(mouseDelta.X, 0.0f);

        return Vector2.Zero;
    }

    public void UpdateHover(AtkEventData* eventData) {
        rightEditNode.IsHovered = rightEditNode.CheckCollision(eventData);
        bottomEditNode.IsHovered = bottomEditNode.CheckCollision(eventData);
        leftEditNode.IsHovered = leftEditNode.CheckCollision(eventData);
        topEditNode.IsHovered = topEditNode.CheckCollision(eventData);
        rightCornerEditNode.IsHovered = rightCornerEditNode.CheckCollision(eventData);
        leftCornerEditNode.IsHovered = leftCornerEditNode.CheckCollision(eventData);

        if (rightCornerEditNode.IsHovered) {
            bottomEditNode.IsHovered = false;
            rightEditNode.IsHovered = false;
        }

        if (leftCornerEditNode.IsHovered) {
            leftEditNode.IsHovered = false;
            bottomEditNode.IsHovered = false;
        }
    }

    public bool AnyHovered() {
        if (rightEditNode.IsHovered) return true;
        if (bottomEditNode.IsHovered) return true;
        if (leftEditNode.IsHovered) return true;
        if (topEditNode.IsHovered) return true;
        if (rightCornerEditNode.IsHovered) return true;
        if (leftCornerEditNode.IsHovered) return true;

        return false;
    }

    public void SetCursor() {
        if (rightEditNode.IsHovered) SetCursor(AddonCursorType.ResizeWE);
        if (bottomEditNode.IsHovered) SetCursor(AddonCursorType.ResizeNS);
        if (leftEditNode.IsHovered) SetCursor(AddonCursorType.ResizeWE);
        if (topEditNode.IsHovered) SetCursor(AddonCursorType.ResizeNS);
        if (rightCornerEditNode.IsHovered) SetCursor(AddonCursorType.ResizeNWSR);
        if (leftCornerEditNode.IsHovered) SetCursor(AddonCursorType.ResizeNESW);
    }
}
