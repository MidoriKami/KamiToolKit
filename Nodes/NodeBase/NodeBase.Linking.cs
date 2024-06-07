using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit;


public abstract unsafe partial class NodeBase {
    public void AttachNode(void* addon, AtkResNode* target, NodePosition position) 
        => NodeLinker.AttachNode((AtkUnitBase*)addon, InternalResNode, target, position);

    public void AttachNode(AtkResNode* target, NodePosition position)
        => NodeLinker.AttachNode(InternalResNode, target, position);

    public void AttachNode(NodeBase target, NodePosition position)
        => AttachNode(target.InternalResNode, position);

    /// <summary>
    /// You're too lazy to pass in the addon that owns the target node, so you use this, but it'll cost up to 1ms of time to calculate.
    /// So it's you being lazy, not the code.
    /// </summary>
    /// <param name="target">Node to attach to.</param>
    /// <param name="position">Position relative to target.</param>
    private void AttachNodeLazy(AtkResNode* target, NodePosition position)
        => NodeLinker.AttachNode(AddonLocator.GetAddonForNode(target), InternalResNode, target, position);

    public void DetachNode() 
        => NodeLinker.DetachNode(InternalResNode);
}