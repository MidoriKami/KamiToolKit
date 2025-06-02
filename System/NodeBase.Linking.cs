using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Addon;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace KamiToolKit.System;

public abstract unsafe partial class NodeBase {
    
    private AtkUnitBase* linkedParentAddon;
    private string? linkedParentName;

    internal void AttachNode(AtkResNode* target, NodePosition position) {
        NodeLinker.AttachNode(InternalResNode, target, position);

        UpdateLinkedAddon();
        UpdateUldManager(target);
    }
    
    internal void AttachNode(NodeBase target, NodePosition position) {
        NodeLinker.AttachNode(InternalResNode, target.InternalResNode, position);
        
        UpdateLinkedAddon();
        UpdateUldManager(target.InternalResNode);
    }

    internal void AttachNode(ComponentNode target, NodePosition position) {
        NodeLinker.AttachNode(InternalResNode, target.ComponentBase->UldManager.RootNode, position);
        NodeLinker.AddNodeToUldObjectList(&target.ComponentBase->UldManager, InternalResNode);

        UpdateLinkedAddon();
        UpdateUldManager(target.InternalResNode);
    }
    
    public void AttachNode(AtkComponentNode* targetNode, NodePosition position) {
        var uldManager = &targetNode->Component->UldManager;
        var target = uldManager->RootNode;
        
        NodeLinker.AttachNode(InternalResNode, target, position);
        NodeLinker.AddNodeToUldObjectList(uldManager, InternalResNode);

        UpdateLinkedAddon();
        UpdateUldManager(target);
    }
    
    internal void AttachNode(NativeAddon addon, NodePosition position) {
        NodeLinker.AttachNode(InternalResNode, addon.InternalAddon->RootNode, position);
        NodeLinker.AddNodeToUldObjectList(&addon.InternalAddon->UldManager, InternalResNode);
        
        UpdateLinkedAddon();
        UpdateUldManager(addon.InternalAddon->RootNode);
    }

    internal void DetachNode() {
        // Find this nodes owner uldManager and save it
        var nodesUldManager = GetUldManagerForNode(InternalResNode);
        
        // Remove node from adjacent nodes
        NodeLinker.DetachNode(InternalResNode);
        NodeLinker.RemoveNodeFromUldObjectList(nodesUldManager, InternalResNode);

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

    private bool IsAddonPointerValid(AtkUnitBase* addon, string? addonName) {
        if (addon is null) return false;
        if (addonName is null) return false;

        var nativePointer = (AtkUnitBase*) DalamudInterface.Instance.GameGui.GetAddonByName(addonName);

        if (nativePointer is null) return false;
		
        return addon == nativePointer;
    }

    private void UpdateUldManager(AtkResNode* target) {
        var componentManager = GetUldManagerForNode(target);
        if (componentManager is not null) {
            componentManager->UpdateDrawNodeList();
            return;
        }

        var parentAddon = GetAddonForNode(target);
        if (parentAddon is not null) {
            parentAddon->UldManager.UpdateDrawNodeList();
            parentAddon->UpdateCollisionNodeList(false);
        }
    }

    private AtkUldManager* GetUldManagerForNode(AtkResNode* node) {
        var targetNode = node;

        while (targetNode is not null) {
            if (targetNode->GetNodeType() is NodeType.Component) {
                var componentNode = (AtkComponentNode*) targetNode;
                return &componentNode->Component->UldManager;
            }

            targetNode = targetNode->ParentNode;
        }
        
        var parentAddon = GetAddonForNode(node);
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