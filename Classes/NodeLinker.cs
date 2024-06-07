using System;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Classes;

public enum NodePosition {
    BeforeTarget,
    AfterTarget,
    BeforeAllSiblings,
    AfterAllSiblings,
    AsLastChild,
    AsFirstChild
}

public static unsafe class NodeLinker {
    internal static void AttachNode(void* addon, AtkResNode* node, AtkResNode* targetNode, NodePosition position) 
        => AttachNode(node, (AtkUnitBase*)addon, targetNode, position);

    /// <summary>
    /// Attaches a node that may or may not be attached to an addon.
    /// This method will not update the parent addon to notify it has a new node, if one exists.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="attachTargetNode"></param>
    /// <param name="position"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal static void AttachNode(AtkResNode* node, AtkResNode* attachTargetNode, NodePosition position) {
        switch (position) {
            case NodePosition.BeforeTarget:
                EmplaceBefore(node, attachTargetNode);
                break;

            case NodePosition.AfterTarget:
                EmplaceAfter(node, attachTargetNode);
                break;

            case NodePosition.BeforeAllSiblings:
                EmplaceBeforeSiblings(node, attachTargetNode);
                break;

            case NodePosition.AfterAllSiblings:
                EmplaceAfterSiblings(node, attachTargetNode);
                break;
            
            case NodePosition.AsLastChild:
                EmplaceAsLastChild(node, attachTargetNode);
                break;
            
            case NodePosition.AsFirstChild:
                EmplaceAsFirstChild(node, attachTargetNode);
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(position), position, null);
        }
    }
    
    private static void AttachNode(AtkResNode* node, AtkUnitBase* parent, AtkResNode* attachTargetNode, NodePosition position) {
        AttachNodeUnsafe(node, attachTargetNode, position);
        
        parent->UpdateCollisionNodeList(false);
        parent->UldManager.UpdateDrawNodeList();
    }
    
     private static void EmplaceBefore(AtkResNode* node, AtkResNode* attachTargetNode) {
        node->ParentNode = attachTargetNode->ParentNode;

        // Target node is the head of the nodelist, we will be the new head.
        if (attachTargetNode->NextSiblingNode is null) {
            attachTargetNode->ParentNode->ChildNode = node;
        }

        // We have a node that will be before us
        if (attachTargetNode->NextSiblingNode is not null) {
            attachTargetNode->NextSiblingNode->PrevSiblingNode = node;
            node->NextSiblingNode = attachTargetNode->NextSiblingNode;
        }

        attachTargetNode->NextSiblingNode = node;
        node->PrevSiblingNode = attachTargetNode;
        
        attachTargetNode->ParentNode->ChildCount++;
    }

    private static void EmplaceAfter(AtkResNode* node, AtkResNode* attachTargetNode) {
        node->ParentNode = attachTargetNode->ParentNode;

        // We have a node that will be after us
        if (attachTargetNode->PrevSiblingNode is not null) {
            attachTargetNode->PrevSiblingNode->NextSiblingNode = node;
            node->PrevSiblingNode = attachTargetNode->PrevSiblingNode;
        }

        attachTargetNode->PrevSiblingNode = node;
        node->NextSiblingNode = attachTargetNode;
        
        attachTargetNode->ParentNode->ChildCount++;
    }

    private static void EmplaceBeforeSiblings(AtkResNode* node, AtkResNode* attachTargetNode) {
        var current = attachTargetNode;
        var previous = current;

        while (current is not null) {
            previous = current;
            current = current->NextSiblingNode;
        }

        if (previous is not null) {
            EmplaceBefore(node, previous);
        }
        
        attachTargetNode->ParentNode->ChildCount++;
    }

    private static void EmplaceAfterSiblings(AtkResNode* node, AtkResNode* attachTargetNode) {
        var current = attachTargetNode;
        var previous = current;

        while (current is not null) {
            previous = current;
            current = current->PrevSiblingNode;
        }

        if (previous is not null) {
            EmplaceAfter(node, previous);
        }

        attachTargetNode->ParentNode->ChildCount++;
    }

    private static void EmplaceAsLastChild(AtkResNode* node, AtkResNode* attachTargetNode) {
        // If the child list is empty
        if (attachTargetNode->ChildNode is null)
        {
            attachTargetNode->ChildNode = node;
            node->ParentNode = attachTargetNode;
            attachTargetNode->ChildCount++;
        }
        // Else Add to the List
        else
        {
            var currentNode = attachTargetNode->ChildNode;
            while (currentNode is not null && currentNode->PrevSiblingNode != null)
            {
                currentNode = currentNode->PrevSiblingNode;
            }
        
            node->ParentNode = attachTargetNode;
            node->NextSiblingNode = currentNode;
            currentNode->PrevSiblingNode = node;
            attachTargetNode->ChildCount++;
        }
    }
    
    private static void EmplaceAsFirstChild(AtkResNode* node, AtkResNode* attachTargetNode) {
        // If the child list is empty
        if (attachTargetNode->ChildNode is null && attachTargetNode->ChildCount is 0)
        {
            attachTargetNode->ChildNode = node;
            node->ParentNode = attachTargetNode;
            attachTargetNode->ChildCount++;
        }
        // Else Add to the List as the First Child
        else {
            attachTargetNode->ChildNode->NextSiblingNode = node;
            node->PrevSiblingNode = attachTargetNode->ChildNode;
            attachTargetNode->ChildNode = node;
            node->ParentNode = attachTargetNode;
            attachTargetNode->ChildCount++;
        }
    }

    public static void DetachNode(AtkResNode* node) {
        if (node is null) return;
        if (node->ParentNode is null) return;
        
        // If we were the main child of the containing node, assign it to the next element in line.
        if (node->ParentNode->ChildNode == node) {
            // And we have a node after us, our parents child should be the next node in line.
            if (node->PrevSiblingNode != null) {
                node->ParentNode->ChildNode = node->PrevSiblingNode;
            }
            // else our parent is no longer pointing to any children.
            else {
                node->ParentNode->ChildNode = null;
            }
        }
        
        // If we have a node before us
        if (node->NextSiblingNode != null) {
            // and a node after us, link the one before to the one after
            if (node->PrevSiblingNode != null) {
                node->NextSiblingNode->PrevSiblingNode = node->PrevSiblingNode;
            }
            // else unlink it from us
            else {
                node->NextSiblingNode->PrevSiblingNode = null;
            }
        }
        
        // If we have a node after us
        if (node->PrevSiblingNode != null) {
            // and a node before us, link the one after to the one before
            if (node->NextSiblingNode != null) {
                node->PrevSiblingNode->NextSiblingNode = node->NextSiblingNode;
            }
            // else unlink it from us
            else {
                node->PrevSiblingNode->NextSiblingNode = null;
            }
        }
    }
}