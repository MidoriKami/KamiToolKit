using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

public class SimpleComponentNode : ComponentNode<AtkComponentBase, AtkUldComponentDataBase> {
    public override ReadOnlySeString? Tooltip {
        get => CollisionNode.Tooltip;
        set => CollisionNode.Tooltip = value;
    }

    public bool DisableCollisionNode {
        get;
        set {
            field = value;
            CollisionNode.NodeFlags = 0;
        }
    }
}
