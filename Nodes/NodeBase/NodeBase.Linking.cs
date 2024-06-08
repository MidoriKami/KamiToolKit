using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;


public abstract unsafe partial class NodeBase {
    internal string? AttachedAddonName;

    public void AttachNode(void* addon, AtkResNode* target, NodePosition position) {
        AttachedAddonName = ((AtkUnitBase*) addon)->NameString;
        NodeLinker.AttachNode((AtkUnitBase*) addon, InternalResNode, target, position);
    }

    public void AttachNode(AtkResNode* target, NodePosition position)
        => NodeLinker.AttachNode(InternalResNode, target, position);

    public void AttachNode(NodeBase target, NodePosition position)
        => AttachNode(target.InternalResNode, position);

    public void DetachNode() {
        NodeLinker.DetachNode(InternalResNode);
        
        if (AttachedAddonName is not null) {
            var parentAddon = AtkStage.Instance()->RaptureAtkUnitManager->GetAddonByName(AttachedAddonName);
            if (parentAddon is not null && parentAddon->IsReady && parentAddon->UldManager.LoadedState is AtkLoadState.Loaded) {
                parentAddon->UpdateCollisionNodeList(false);
                parentAddon->UldManager.UpdateDrawNodeList();
            }
            AttachedAddonName = null;
        }
    }
}