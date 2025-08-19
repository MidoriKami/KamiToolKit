using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.System;

namespace KamiToolKit.Nodes;

public unsafe class CollisionNode() : NodeBase<AtkCollisionNode>(NodeType.Collision) {
    public virtual CollisionType CollisionType {
        get => Node->CollisionType;
        set => Node->CollisionType = value;
    }

    public virtual uint Uses {
        get => Node->Uses;
        set => Node->Uses = (ushort)value;
    }

    public virtual AtkComponentBase* LinkedComponent {
        get => Node->LinkedComponent;
        set => Node->LinkedComponent = value;
    }
}
