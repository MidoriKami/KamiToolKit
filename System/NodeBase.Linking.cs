using System;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using KamiToolKit.Addon;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace KamiToolKit.System;

public abstract unsafe partial class NodeBase {

    // Note, these fields are not to be confused with those from AutoDetach
    // These are only for use with updating UldManagers
    private AtkUnitBase* linkedParentAddon;
    private string? linkedParentName;
    
    internal void AttachNode(NodeBase target, NodePosition position) {
        NodeLinker.AttachNode(InternalResNode, target.InternalResNode, position);
        
        UpdateLinkedAddon();
        UpdateAttachedUldManger();
    }

    internal void AttachNode(ComponentNode target, NodePosition position) {
        NodeLinker.AttachNode(InternalResNode, target.ComponentBase->UldManager.RootNode, position);
        NodeLinker.AddNodeToUldObjectList(&target.ComponentBase->UldManager, InternalResNode);

        VisitChildren(InternalResNode, node => {
            NodeLinker.AddNodeToUldObjectList(&target.ComponentBase->UldManager, node);
        });

        UpdateLinkedAddon();
        UpdateAttachedUldManger();
    }

    internal void AttachNode(NativeAddon addon, NodePosition position) {
        NodeLinker.AttachNode(InternalResNode, addon.InternalAddon->RootNode, position);
        NodeLinker.AddNodeToUldObjectList(&addon.InternalAddon->UldManager, InternalResNode);
        
        VisitChildren(InternalResNode, thisChildren => {
            NodeLinker.AddNodeToUldObjectList(&addon.InternalAddon->UldManager, thisChildren);
        });
        
        UpdateLinkedAddon();
        UpdateAttachedUldManger();
    }

    internal void DetachNode() {
        // Find this nodes owning uldManager and save it
        var nodesUldManager = GetUldManagerForNode(InternalResNode);
        
        // Remove node from adjacent nodes
        NodeLinker.DetachNode(InternalResNode);

        // If the addon that we initially attached to is still valid
        if (IsAddonPointerValid(linkedParentAddon, linkedParentName)) {
            
            // and the uldManager we looked for earlier is valid
            if (nodesUldManager is not null) {
                
                // Update DrawList that we are changing
                nodesUldManager->UpdateDrawNodeList();
            }
            
            // And also update addons collision node list just in case
            linkedParentAddon->UpdateCollisionNodeList(false);
        }
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

    private void UpdateAttachedUldManger() {
        var componentManager = GetUldManagerForNode(InternalResNode);
        if (componentManager is not null) {
            componentManager->UpdateDrawNodeList();
            return;
        }

        var parentAddon = GetAddonForNode(InternalResNode);
        if (parentAddon is not null) {
            parentAddon->UldManager.UpdateDrawNodeList();
            parentAddon->UpdateCollisionNodeList(false);
        }
    }

    private AtkUldManager* GetUldManagerForNode(AtkResNode* node) {
        var targetNode = InternalResNode;

        while (targetNode is not null) {
            if (targetNode->GetNodeType() is NodeType.Component) {
                var componentNode = (AtkComponentNode*) targetNode;
                return &componentNode->Component->UldManager;
            }

            targetNode = targetNode->ParentNode;
        }
        
        var parentAddon = GetAddonForNode(InternalResNode);
        if (parentAddon is not null) {
            return &parentAddon->UldManager;
        }

        return null;
    }
    
    private void UpdateLinkedAddon() {
        var addon = GetAddonForNode(InternalResNode);

        if (addon is not null) {
            linkedParentAddon = addon;
            linkedParentName = addon->NameString;
        }
    }

    private AtkUnitBase* GetAddonForNode(AtkResNode* node) {
        if (Experimental.Instance.GetAddonByNode != null) {
            return Experimental.Instance.GetAddonByNode.Invoke(RaptureAtkUnitManager.Instance(), node);
        }

        return null;
    }
}