using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.System;

namespace KamiToolKit.Nodes;

public unsafe class CollisionNode() : NodeBase<AtkCollisionNode>(NodeType.Collision) {
    public virtual CollisionType CollisionType {
        get => InternalNode->CollisionType;
        set => InternalNode->CollisionType = value;
    }

    public virtual uint Uses {
        get => InternalNode->Uses;
        set => InternalNode->Uses = (ushort)value;
    }

    public virtual AtkComponentBase* LinkedComponent {
        get => InternalNode->LinkedComponent;
        set => InternalNode->LinkedComponent = value;
    }
}
