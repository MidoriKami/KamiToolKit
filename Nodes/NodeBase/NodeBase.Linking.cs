using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public abstract unsafe partial class NodeBase {
    internal void AttachNode(NodeBase target, NodePosition position) {
        NodeLinker.AttachNode(InternalResNode, target.InternalResNode, position);
    }

    internal void DetachNode() {
        NodeLinker.DetachNode(InternalResNode);
    }
}