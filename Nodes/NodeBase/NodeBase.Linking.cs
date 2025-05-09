using FFXIVClientStructs.FFXIV.Client.System.Memory;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public abstract unsafe partial class NodeBase {
    internal void AttachNode(NodeBase target, NodePosition position) {
        NodeLinker.AttachNode(InternalResNode, target.InternalResNode, position);
    }

    internal void AttachNode<T, TU>(ComponentNode<T, TU> target, NodePosition position) where T : unmanaged, ICreatable where TU : unmanaged {
        NodeLinker.AttachNode(InternalResNode, target.InternalResNode, position);
        target.ComponentBase->UldManager.UpdateDrawNodeList();
    }

    internal void DetachNode() {
        NodeLinker.DetachNode(InternalResNode);
    }
}