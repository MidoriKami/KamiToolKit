using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public abstract unsafe partial class NodeBase {
    internal void AttachNode(NodeBase target, NodePosition position) {
        NodeLinker.AttachNode(InternalResNode, target.InternalResNode, position);
    }

    /// <summary>
    /// When attaching to a custom ComponentNode, we want to attach to the ULDManager, which should already have a collision node allocated.
    /// As this node is intended to be self contained, it will update the draw list upon any additions.
    /// </summary>
    internal void AttachNode(ComponentNode target, NodePosition position) {
        NodeLinker.AttachNode(InternalResNode, target.ComponentBase->UldManager.RootNode, position);
        target.ComponentBase->UldManager.UpdateDrawNodeList();
    }

    internal void DetachNode() {
        NodeLinker.DetachNode(InternalResNode);
    }
}