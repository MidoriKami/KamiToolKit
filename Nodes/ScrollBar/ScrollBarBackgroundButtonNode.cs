using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Nodes;

public unsafe class ScrollBarBackgroundButtonNode : ComponentNode<AtkComponentButton, AtkUldComponentDataButton> {
    public ScrollBarBackgroundButtonNode() {
        SetInternalComponentType(ComponentType.Button);

        Component->ButtonBGNode = CollisionNode.InternalResNode;

        Data->Nodes[0] = 0;
        Data->Nodes[1] = CollisionNode.NodeId;

        InitializeComponentEvents();
    }
}
