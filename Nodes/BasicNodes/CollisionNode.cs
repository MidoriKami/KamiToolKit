using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.System;

namespace KamiToolKit.Nodes;

public unsafe class CollisionNode() : NodeBase<AtkCollisionNode>(NodeType.Collision) {
    public CollisionType CollisionType {
        get => (CollisionType)InternalNode->CollisionType;
        set => InternalNode->CollisionType = (ushort) value;
    }

    public uint Uses {
        get => InternalNode->Uses;
        set => InternalNode->Uses = (ushort) value;
    }

    public AtkComponentBase* LinkedComponent {
        get => InternalNode->LinkedComponent;
        set => InternalNode->LinkedComponent = value;
    }

    public bool CheckCollision(Vector2 coordinate, bool inclusive = true)
        => InternalNode->CheckCollisionAtCoords((short)coordinate.X, (short)coordinate.Y, inclusive);
}