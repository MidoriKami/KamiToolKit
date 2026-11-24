using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Core.Interfaces.Nodes;
using KamiToolKit.Core.Primitives.NodeBase;

namespace KamiToolKit.UI.Nodes;

/// <summary>
/// Implementation of the game's CollisionNode.
/// </summary>
public unsafe class CollisionNode() : NodeBase<AtkCollisionNode>(NodeType.Collision), ICollisionNode {

    /// <inheritdoc />
    public CollisionType CollisionType {
        get => Node->CollisionType;
        set => Node->CollisionType = value;
    }

    /// <inheritdoc />
    public uint Uses {
        get => Node->Uses;
        set => Node->Uses = (ushort)value;
    }

    /// <inheritdoc />
    public AtkComponentBase* LinkedComponent {
        get => Node->LinkedComponent;
        set => Node->LinkedComponent = value;
    }
}
