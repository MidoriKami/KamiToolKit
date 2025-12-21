using System.Numerics;
using Dalamud.Game.Addon.Events;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

internal unsafe class NodeEditOverlayNode : SimpleComponentNode {

    private readonly NineGridNode backgroundNode;
    private readonly ResizeNineGridNode bottomEditNode;
    private readonly ResizeButtonNode leftCornerEditNode;
    private readonly ResizeNineGridNode leftEditNode;
    private readonly ResizeButtonNode rightCornerEditNode;
    private readonly ResizeNineGridNode rightEditNode;
    private readonly ResizeNineGridNode topEditNode;

    public NodeEditOverlayNode() {
        backgroundNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/HUDLayout.tex",
            TextureSize = new Vector2(44.0f, 32.0f),
            TextureCoordinates = new Vector2(0.0f, 0.0f),
            TopOffset = 20,
            BottomOffset = 8,
            LeftOffset = 21,
            RightOffset = 21,
            Alpha = 0.75f,
        };
        backgroundNode.AttachNode(this);
        
        rightEditNode = new ResizeNineGridNode();
        rightEditNode.AttachNode(this);

        bottomEditNode = new ResizeNineGridNode();
        bottomEditNode.AttachNode(this);

        leftEditNode = new ResizeNineGridNode();
        leftEditNode.AttachNode(this);

        topEditNode = new ResizeNineGridNode();
        topEditNode.AttachNode(this);

        rightCornerEditNode = new ResizeButtonNode(ResizeDirection.BottomRight);
        rightCornerEditNode.AttachNode(this);

        leftCornerEditNode = new ResizeButtonNode(ResizeDirection.BottomLeft);
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

        backgroundNode.Size = Size - new Vector2(24.0f, 24.0f);
        backgroundNode.Position = new Vector2(12.0f, 12.0f);
        
        const float lineThickness = 4.0f;

        leftEditNode.Size = new Vector2(Height - 32.0f, lineThickness);
        leftEditNode.Position = new Vector2(16.0f + leftEditNode.Height / 2.0f, 16.0f);
        leftEditNode.RotationDegrees = 90.0f;

        rightEditNode.Size = new Vector2(Height - 32.0f, lineThickness);
        rightEditNode.Position = new Vector2(Width - 16.0f + rightEditNode.Height / 2.0f, 16.0f);
        rightEditNode.RotationDegrees = 90.0f;

        topEditNode.Size = new Vector2(Width - 32.0f, lineThickness);
        topEditNode.Position = new Vector2(16.0f, 16.0f - lineThickness / 2.0f);

        bottomEditNode.Size = new Vector2(Width - 32.0f, lineThickness);
        bottomEditNode.Position = new Vector2(16.0f, Height - 16.0f - lineThickness / 2.0f);

        leftCornerEditNode.Size = new Vector2(24.0f, 24.0f);
        leftCornerEditNode.Position = new Vector2(16.0f - lineThickness / 4.0f, Height - 16.0f - leftCornerEditNode.Height);

        rightCornerEditNode.Size = new Vector2(24.0f, 24.0f);
        rightCornerEditNode.Position = new Vector2(Width - 16.0f - rightCornerEditNode.Width + lineThickness / 4.0f, Height - 16.0f - rightCornerEditNode.Height);
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
    
    private static void SetCursor(AddonCursorType cursor)
        => DalamudInterface.Instance.AddonEventManager.SetCursor(cursor);
}
