using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.System;

namespace KamiToolKit.Nodes;

public unsafe class ScrollingAreaNode<T> : SimpleComponentNode where T : NodeBase, new() {

    public readonly SimpleComponentNode ContentAreaClipNode;
    public readonly T ContentAreaNode;
    public readonly ScrollBarNode ScrollBarNode;
    public readonly CollisionNode ScrollingCollisionNode;

    public ScrollingAreaNode() {
        ScrollingCollisionNode = new CollisionNode {
            NodeId = 2,
            IsVisible = true, 
            EventFlagsSet = true,
        };
        ScrollingCollisionNode.AttachNode(this);

        ContentAreaClipNode = new SimpleComponentNode {
            NodeId = 3,
            NodeFlags = NodeFlags.Clip | NodeFlags.EmitsEvents, 
            IsVisible = true,
        };
        ContentAreaClipNode.AttachNode(this);

        ContentAreaNode = new T {
            NodeId = 4,
            IsVisible = true,
        };
        ContentAreaNode.AttachNode(ContentAreaClipNode);

        ScrollBarNode = new ScrollBarNode {
            NodeId = 5,
            ContentNode = ContentAreaNode, 
            ContentCollisionNode = ScrollingCollisionNode, 
            IsVisible = true,
        };
        ScrollBarNode.AttachNode(this);

        ContentAreaClipNode.InternalResNode->AtkEventManager.RegisterEvent(
            AtkEventType.MouseWheel,
            5,
            null,
            ScrollingCollisionNode,
            ScrollBarNode,
            false);
        
        ScrollingCollisionNode.InternalResNode->AtkEventManager.RegisterEvent(
            AtkEventType.MouseWheel,
            5,
            null,
            ScrollingCollisionNode,
            ScrollBarNode,
            false);

        ContentAreaNode.InternalResNode->AtkEventManager.RegisterEvent(
            AtkEventType.MouseWheel,
            5,
            null,
            ScrollingCollisionNode,
            ScrollBarNode,
            false);
    }

    public virtual T ContentNode => ContentAreaNode;

    public int ScrollPosition {
        get => ScrollBarNode.ScrollPosition;
        set => ScrollBarNode.ScrollPosition = value;
    }

    public int ScrollSpeed {
        get => ScrollBarNode.ScrollSpeed;
        set => ScrollBarNode.ScrollSpeed = value;
    }

    public required float ContentHeight {
        get => ContentAreaNode.Height;
        set {
            ContentAreaNode.Height = value;
            ScrollBarNode.UpdateScrollParams();
        }
    }

    public bool AutoHideScrollBar {
        get => ScrollBarNode.HideWhenDisabled;
        set => ScrollBarNode.HideWhenDisabled = value;
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        ContentAreaNode.Width = Width - 16.0f;
        ScrollingCollisionNode.Size = new Vector2(Width - 16.0f, Height);
        ContentAreaClipNode.Size = new Vector2(Width - 16.0f, Height);
        ScrollBarNode.Size = new Vector2(8.0f, Height);
        ScrollBarNode.UpdateScrollParams();

        ScrollBarNode.X = Width - 8.0f;
    }
}
