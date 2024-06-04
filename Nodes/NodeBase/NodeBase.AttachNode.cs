using System;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit;

public enum NodePosition {
    BeforeTarget,
    AfterTarget,
    BeforeAllSiblings,
    AfterAllSiblings,
    AsLastChild,
    AsFirstChild
}

public abstract unsafe partial class NodeBase<T> where T : unmanaged, ICreatable {
    private IAddonLifecycle? addonLifecycleService;
    
    // Attaches a self-removing node to an addon, when the addon finalizes, or the plugin unloads, attached nodes will automatically be removed.
    public void AttachNode(IAddonLifecycle addonLifecycle, AtkResNode* targetNode, NodePosition position) {
        var atkUnitBase = GetAddonForNode(targetNode);
        if (atkUnitBase is null) return;

        addonLifecycleService ??= addonLifecycle;
        
        addonLifecycle.RegisterListener(AddonEvent.PreFinalize, atkUnitBase->NameString, AutomaticNodeCleanup);
        AttachNode(atkUnitBase, targetNode, position);
    }

    private void AutomaticNodeCleanup(AddonEvent type, AddonArgs args) {
        addonLifecycleService!.UnregisterListener(AutomaticNodeCleanup);
        Dispose();
    }
    
    /// <summary>
    /// Searches all loaded addons and all nodes contained within to find the addon that owns the targed node.
    /// </summary>
    /// <param name="node">The node to search for</param>
    /// <returns>AtkUnitBase* to the addon </returns>
    private static AtkUnitBase* GetAddonForNode(AtkResNode* node) {
        foreach (ref readonly var unit in AtkStage.Instance()->RaptureAtkUnitManager->AllLoadedUnitsList.Entries) {
            if (unit.Value is null) continue;
            
            if (AddonContainsNode(ref unit.Value->UldManager, node)) {
                return unit.Value;
            }
        }

        return null;
    }

    private static bool AddonContainsNode(ref AtkUldManager uldManager, AtkResNode* targetNode) {
        var nodeListCount = uldManager.NodeListCount;
        
        for (var index = 0; index < nodeListCount; ++index) {
            ref var currentNode = ref uldManager.NodeList[index];
            if (targetNode == currentNode) return true;

            if ((uint) currentNode->Type > 1000u) {
                var componentNode = ((AtkComponentNode*) currentNode)->Component;
                if (componentNode is not null) {
                    if (AddonContainsNode(ref componentNode->UldManager, targetNode)) {
                        return true;
                    }
                }
            }
        }

        return false;
    }
    
    private void AttachNode(AtkUnitBase* parent, AtkResNode* targetNode, NodePosition position) {
        switch (position) {
            case NodePosition.BeforeTarget:
                EmplaceBefore(targetNode);
                break;

            case NodePosition.AfterTarget:
                EmplaceAfter(targetNode);
                break;

            case NodePosition.BeforeAllSiblings:
                EmplaceBeforeSiblings(targetNode);
                break;

            case NodePosition.AfterAllSiblings:
                EmplaceAfterSiblings(targetNode);
                break;
            
            case NodePosition.AsLastChild:
                EmplaceAsLastChild(targetNode);
                break;
            
            case NodePosition.AsFirstChild:
                EmplaceAsFirstChild(targetNode);
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(position), position, null);
        }
        
        parent->UpdateCollisionNodeList(false);
        parent->UldManager.UpdateDrawNodeList();
    }
    
     private void EmplaceBefore(AtkResNode* targetNode) {
        InternalResNode->ParentNode = targetNode->ParentNode;

        // Target node is the head of the nodelist, we will be the new head.
        if (targetNode->NextSiblingNode is null) {
            targetNode->ParentNode->ChildNode = InternalResNode;
        }

        // We have a node that will be before us
        if (targetNode->NextSiblingNode is not null) {
            targetNode->NextSiblingNode->PrevSiblingNode = InternalResNode;
            InternalResNode->NextSiblingNode = targetNode->NextSiblingNode;
        }

        targetNode->NextSiblingNode = InternalResNode;
        InternalResNode->PrevSiblingNode = targetNode;
        
        targetNode->ParentNode->ChildCount++;
    }

    private void EmplaceAfter(AtkResNode* targetNode) {
        InternalResNode->ParentNode = targetNode->ParentNode;

        // We have a node that will be after us
        if (targetNode->PrevSiblingNode is not null) {
            targetNode->PrevSiblingNode->NextSiblingNode = InternalResNode;
            InternalResNode->PrevSiblingNode = targetNode->PrevSiblingNode;
        }

        targetNode->PrevSiblingNode = InternalResNode;
        InternalResNode->NextSiblingNode = targetNode;
        
        targetNode->ParentNode->ChildCount++;
    }

    private void EmplaceBeforeSiblings(AtkResNode* targetNode) {
        var current = targetNode;
        var previous = current;

        while (current is not null) {
            previous = current;
            current = current->NextSiblingNode;
        }

        if (previous is not null) {
            EmplaceBefore(previous);
        }
        
        targetNode->ParentNode->ChildCount++;
    }

    private void EmplaceAfterSiblings(AtkResNode* targetNode) {
        var current = targetNode;
        var previous = current;

        while (current is not null) {
            previous = current;
            current = current->PrevSiblingNode;
        }

        if (previous is not null) {
            EmplaceAfter(previous);
        }

        targetNode->ParentNode->ChildCount++;
    }

    private void EmplaceAsLastChild(AtkResNode* targetNode) {
        // If the child list is empty
        if (targetNode->ChildNode is null)
        {
            targetNode->ChildNode = InternalResNode;
            InternalResNode->ParentNode = targetNode;
            targetNode->ChildCount++;
        }
        // Else Add to the List
        else
        {
            var currentNode = targetNode->ChildNode;
            while (currentNode is not null && currentNode->PrevSiblingNode != null)
            {
                currentNode = currentNode->PrevSiblingNode;
            }
        
            InternalResNode->ParentNode = targetNode;
            InternalResNode->NextSiblingNode = currentNode;
            currentNode->PrevSiblingNode = InternalResNode;
            targetNode->ChildCount++;
        }
    }
    
    private void EmplaceAsFirstChild(AtkResNode* targetNode) {
        // If the child list is empty
        if (targetNode->ChildNode is null && targetNode->ChildCount is 0)
        {
            targetNode->ChildNode = InternalResNode;
            InternalResNode->ParentNode = targetNode;
            targetNode->ChildCount++;
        }
        // Else Add to the List as the First Child
        else {
            targetNode->ChildNode->NextSiblingNode = InternalResNode;
            InternalResNode->PrevSiblingNode = targetNode->ChildNode;
            targetNode->ChildNode = InternalResNode;
            InternalResNode->ParentNode = targetNode;
            targetNode->ChildCount++;
        }
    }

    public void UnAttachNode() {
        if (InternalResNode is null) return;
        if (InternalResNode->ParentNode is null) return;
        
        // If we were the main child of the containing node, assign it to the next element in line.
        if (InternalResNode->ParentNode->ChildNode == InternalResNode) {
            // And we have a node after us, our parents child should be the next node in line.
            if (InternalResNode->PrevSiblingNode != null) {
                InternalResNode->ParentNode->ChildNode = InternalResNode->PrevSiblingNode;
            }
            // else our parent is no longer pointing to any children.
            else {
                InternalResNode->ParentNode->ChildNode = null;
            }
        }
        
        // If we have a node before us
        if (InternalResNode->NextSiblingNode != null) {
            // and a node after us, link the one before to the one after
            if (InternalResNode->PrevSiblingNode != null) {
                InternalResNode->NextSiblingNode->PrevSiblingNode = InternalResNode->PrevSiblingNode;
            }
            // else unlink it from us
            else {
                InternalResNode->NextSiblingNode->PrevSiblingNode = null;
            }
        }
        
        // If we have a node after us
        if (InternalResNode->PrevSiblingNode != null) {
            // and a node before us, link the one after to the one before
            if (InternalResNode->NextSiblingNode != null) {
                InternalResNode->PrevSiblingNode->NextSiblingNode = InternalResNode->NextSiblingNode;
            }
            // else unlink it from us
            else {
                InternalResNode->PrevSiblingNode->NextSiblingNode = null;
            }
        }
    }
}