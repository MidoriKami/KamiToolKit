using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Core.Primitives.ComponentNode;
using KamiToolKit.UI.Nodes.ComponentNode;

namespace KamiToolKit.UI.Nodes.Parts;

/// <summary>
/// Specialized implementation for use with <see cref="ScrollBarNode"/>. Not intended for external use.
/// </summary>
public unsafe class ScrollBarBackgroundButtonNode : ComponentNode<AtkComponentButton, AtkUldComponentDataButton> {

    /// <summary>
    /// Constructs a new <see cref="ScrollBarBackgroundButtonNode"/>
    /// </summary>
    public ScrollBarBackgroundButtonNode() {
        SetInternalComponentType(ComponentType.Button);

        Component->ButtonBGNode = CollisionNode;

        Data->Nodes[0] = 0;
        Data->Nodes[1] = CollisionNode.NodeId;

        InitializeComponentEvents();
    }
}
