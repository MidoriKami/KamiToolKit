using System;
using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Classes;

public enum NodePosition {
    BeforeTarget,
    AfterTarget,
    BeforeAllSiblings,
    AfterAllSiblings,
    AsLastChild,
    AsFirstChild,
}

internal static unsafe class NodeLinker {
    internal static void AddNodeToUldObjectList(AtkUldManager* uldManager, AtkResNode* newNode) {
        if (uldManager is null) return;
        if (NodeListContainsNode(uldManager, newNode)) return;

        var oldSize = uldManager->Objects->NodeCount;
        var newSize = oldSize + 1;
        var newBuffer = (AtkResNode**) NativeMemoryHelper.Malloc((ulong)(newSize * 8));

        if (oldSize > 0) {
            foreach (var index in Enumerable.Range(0, oldSize)) {
                newBuffer[index] = uldManager->Objects->NodeList[index];
            }
        
            NativeMemoryHelper.Free(uldManager->Objects->NodeList, (ulong)(oldSize * 8));
        }
        
        newBuffer[newSize - 1] = newNode;

        uldManager->Objects->NodeList = newBuffer;
        uldManager->Objects->NodeCount = newSize;
    }
    
    internal static void RemoveNodeFromUldObjectList(AtkUldManager* uldManager, AtkResNode* nodeToRemove) {
        if (uldManager is null) return;
        if (!NodeListContainsNode(uldManager, nodeToRemove)) return;
        
        var oldSize = uldManager->Objects->NodeCount;
        var newSize = oldSize - 1;
        var newBuffer = (AtkResNode**) NativeMemoryHelper.Malloc((ulong)(newSize * 8));

        var newIndex = 0;
        foreach (var index in Enumerable.Range(0, oldSize)) {
            if (uldManager->Objects->NodeList[index] != nodeToRemove) {
                newBuffer[newIndex] = uldManager->Objects->NodeList[index];
            }
            
            newIndex++;
        }

        NativeMemoryHelper.Free(uldManager->Objects->NodeList, (ulong)(oldSize * 8));
        uldManager->Objects->NodeList = newBuffer;
        uldManager->Objects->NodeCount = newSize;
    }

    private static bool NodeListContainsNode(AtkUldManager* uldManager, AtkResNode* node) {
        if (uldManager->Objects is null) return false;
        if (uldManager->Objects->NodeList is null) return false;
        
        foreach (var index in Enumerable.Range(0, uldManager->Objects->NodeCount)) {
            if (uldManager->Objects->NodeList[index] == node) return true;
        }

        return false;
    }
    
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

        if (attachTargetNode->ParentNode->GetNodeType() is not NodeType.Component) {
            attachTargetNode->ParentNode->ChildCount++;
        }
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

        if (attachTargetNode->ParentNode->GetNodeType() is not NodeType.Component) {
            attachTargetNode->ParentNode->ChildCount++;
        }
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

        if (attachTargetNode->ParentNode->GetNodeType() is not NodeType.Component) {
            attachTargetNode->ParentNode->ChildCount++;
        }
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

        if (attachTargetNode->ParentNode->GetNodeType() is not NodeType.Component) {
            attachTargetNode->ParentNode->ChildCount++;
        }
    }

    private static void EmplaceAsLastChild(AtkResNode* node, AtkResNode* attachTargetNode) {
        // If the child list is empty
        if (attachTargetNode->ChildNode is null && attachTargetNode->GetNodeType() is not NodeType.Component)
        {
            if (attachTargetNode->GetNodeType() is not NodeType.Component) {
                attachTargetNode->ChildNode = node;
                node->ParentNode = attachTargetNode;
                attachTargetNode->ChildCount++;
            }
            else {
                node->ParentNode = attachTargetNode;
            }
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
            if (attachTargetNode->GetNodeType() is not NodeType.Component) {
                attachTargetNode->ChildCount++;
            }
        }
    }
    
    private static void EmplaceAsFirstChild(AtkResNode* node, AtkResNode* attachTargetNode) {
        // If the child list is empty
        if (attachTargetNode->ChildNode is null && attachTargetNode->ChildCount is 0)
        {
            if (attachTargetNode->GetNodeType() is not NodeType.Component) {
                attachTargetNode->ChildNode = node;
                node->ParentNode = attachTargetNode;
                attachTargetNode->ChildCount++;
            }
            else {
                node->ParentNode = attachTargetNode;
            }
        }
        // Else Add to the List as the First Child
        else {
            if (attachTargetNode->GetNodeType() is not NodeType.Component) {
                attachTargetNode->ChildNode->NextSiblingNode = node;
                node->PrevSiblingNode = attachTargetNode->ChildNode;
                attachTargetNode->ChildNode = node;
                node->ParentNode = attachTargetNode;
                attachTargetNode->ChildCount++;
            }
            else {
                node->PrevSiblingNode = attachTargetNode->ChildNode;
                node->ParentNode = attachTargetNode;
            }
        }
    }

    public static void DetachNode(AtkResNode* node) {
        if (node is null) return;
        if (node->ParentNode is null) return;

        if (node->ParentNode->ChildNode == node)
            node->ParentNode->ChildNode = node->PrevSiblingNode;

        if (node->PrevSiblingNode != null)
            node->PrevSiblingNode->NextSiblingNode = node->NextSiblingNode;

        if (node->NextSiblingNode != null)
            node->NextSiblingNode->PrevSiblingNode = node->PrevSiblingNode;
        
        if (node->ParentNode->GetNodeType() is not NodeType.Component) {
            node->ParentNode->ChildCount--;
        }
    }
}