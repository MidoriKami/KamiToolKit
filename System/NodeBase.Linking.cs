using System;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
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

        AddToUldObjectsList(&target.ComponentBase->UldManager, InternalResNode);
        UpdateLinkedAddon();
        UpdateUldManager(target.InternalResNode);
    }
    
    public void AttachNode(AtkComponentNode* targetNode, NodePosition position) {
        var uldManager = &targetNode->Component->UldManager;
        var target = uldManager->RootNode;
        
        NodeLinker.AttachNode(InternalResNode, target, position);

        AddToUldObjectsList(uldManager, InternalResNode);
        UpdateLinkedAddon();
        UpdateUldManager(target);
    }
    
    internal void AttachNode(NativeAddon addon, NodePosition position) {
        NodeLinker.AttachNode(InternalResNode, addon.InternalAddon->RootNode, position);

        AddToUldObjectsList(&addon.InternalAddon->UldManager, InternalResNode);
        UpdateLinkedAddon();
        UpdateUldManager(addon.InternalAddon->RootNode);
    }

    internal void DetachNode() {
        var nodesUldManager = GetUldManagerForNode(InternalResNode);
        
        RemoveFromUldObjectList(nodesUldManager, InternalResNode);

        NodeLinker.DetachNode(InternalResNode);

        if (IsAddonPointerValid(linkedParentAddon, linkedParentName)) {
            if (nodesUldManager is not null) {
                nodesUldManager->UpdateDrawNodeList();
            }
            
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
    
    private void VisitChildren(AtkResNode* parent, Action<Pointer<AtkResNode>> visitAction) {
        visitAction(parent);
        
        var child = parent->ChildNode;

        while (child is not null) {
            visitAction(child);

            // Be sure to not accidentally visit a components children, they manage their own children
            if (child->ChildNode is not null && child->ChildNode->GetNodeType() is not NodeType.Component) {
                VisitChildren(child->ChildNode, visitAction);
            }

            child = child->PrevSiblingNode;
        }
    }

    private void AddToUldObjectsList(AtkUldManager* uldManager, AtkResNode* parent)
        => VisitChildren(parent, node => {
            NodeLinker.AddNodeToUldObjectList(uldManager, node);
        });

    private void RemoveFromUldObjectList(AtkUldManager* uldManager, AtkResNode* parent) {
        if (uldManager is null) return;
        
        VisitChildren(parent, node => {
            NodeLinker.RemoveNodeFromUldObjectList(uldManager, node);
        });
    }
}