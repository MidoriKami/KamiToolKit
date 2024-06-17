using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public abstract unsafe partial class NodeBase {
    public void AttachNode(AtkResNode* target, NodePosition position)
        => NodeLinker.AttachNode(InternalResNode, target, position);

    public void AttachNode(NodeBase target, NodePosition position)
        => AttachNode(target.InternalResNode, position);

    public void DetachNode()
        => NodeLinker.DetachNode(InternalResNode);
}