using System;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using KamiToolKit.Addon;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace KamiToolKit.System;

public abstract unsafe partial class NodeBase {
    internal void AttachNode(NodeBase target, NodePosition position) {
        NodeLinker.AttachNode(InternalResNode, target.InternalResNode, position);
    }

    internal void AttachNode(ComponentNode target, NodePosition position) {
        NodeLinker.AttachNode(InternalResNode, target.ComponentBase->UldManager.RootNode, position);
        NodeLinker.AddNodeToUldObjectList(&target.ComponentBase->UldManager, InternalResNode);

        VisitChildren(InternalResNode, node => {
            NodeLinker.AddNodeToUldObjectList(&target.ComponentBase->UldManager, node);
        });

        target.ComponentBase->UldManager.UpdateDrawNodeList();
    }

    internal void AttachNode(NativeAddon addon, NodePosition position) {
        NodeLinker.AttachNode(InternalResNode, addon.InternalAddon->RootNode, position);
        NodeLinker.AddNodeToUldObjectList(&addon.InternalAddon->UldManager, InternalResNode);
        
        var thisChildren = InternalResNode->ChildNode;
        while (thisChildren is not null) {
            NodeLinker.AddNodeToUldObjectList(&addon.InternalAddon->UldManager, thisChildren);
            thisChildren = thisChildren->PrevSiblingNode;
        }
        
        addon.InternalAddon->UldManager.UpdateDrawNodeList();
    }

    internal void DetachNode() {
        NodeLinker.DetachNode(InternalResNode);
    }

    internal void VisitChildren(AtkResNode* parent, Action<Pointer<AtkResNode>> visitAction) {
        var child = parent->ChildNode;

        while (child is not null) {
            visitAction(child);

            // Be sure to not accidentally visit a components children, they manage their own children
            if (child->ChildNode is not null && child->ChildNode->Type < (NodeType) 1000) {
                VisitChildren(child->ChildNode, visitAction);
            }

            child = child->PrevSiblingNode;
        }
    }
}