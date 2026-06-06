using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes.ComponentNode;

namespace KamiToolKit.Nodes;

/// <summary>
/// Specialized implementation for use with <see cref="ScrollBarNode"/>. Not intended for external use.
/// </summary>
public unsafe class ScrollBarBackgroundButtonNode : ComponentNode<AtkComponentButton, AtkUldComponentDataButton> {
    public ScrollBarBackgroundButtonNode() {
        SetInternalComponentType(ComponentType.Button);

        Component->ButtonBGNode = CollisionNode;

        Data->Nodes[0] = 0;
        Data->Nodes[1] = CollisionNode.NodeId;

        InitializeComponentEvents();
    }
}
