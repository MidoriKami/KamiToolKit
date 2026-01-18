using System;
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

    private static void EmplaceBefore(AtkResNode* intruder, AtkResNode* prevSiblingNode) {
        var nextSiblingNode =
            HasSpecialNextSibling(prevSiblingNode)
                ? GetLogicalNextSibling(prevSiblingNode)
                : prevSiblingNode->NextSiblingNode;

        // Self

        intruder->ParentNode = prevSiblingNode->ParentNode;
        intruder->PrevSiblingNode = prevSiblingNode;
        if (!HasSpecialNextSibling(intruder))
            intruder->NextSiblingNode = nextSiblingNode;

        // Next sibling

        if (nextSiblingNode is not null)
            nextSiblingNode->PrevSiblingNode = intruder;

        // Prev sibling

        if (!HasSpecialNextSibling(prevSiblingNode))
            prevSiblingNode->NextSiblingNode = intruder;

        // Parent

        if (prevSiblingNode->ParentNode is not null) {
            var parentNode = prevSiblingNode->ParentNode;

            // We might be the new first child
            if (parentNode->ChildNode == prevSiblingNode) {
                parentNode->ChildNode = intruder;
            }

            if (parentNode->GetNodeType() is not NodeType.Component) {
                parentNode->ChildCount++;
            }
        }
    }

    private static void EmplaceAfter(AtkResNode* intruder, AtkResNode* nextSiblingNode) {
        var prevSiblingNode = nextSiblingNode->PrevSiblingNode;

        // Self

        intruder->ParentNode = nextSiblingNode->ParentNode;
        intruder->PrevSiblingNode = prevSiblingNode;
        if (!HasSpecialNextSibling(intruder))
            intruder->NextSiblingNode = nextSiblingNode;

        // Next sibling

        nextSiblingNode->PrevSiblingNode = intruder;

        // Prev sibling

        if (prevSiblingNode is not null && !HasSpecialNextSibling(prevSiblingNode))
            prevSiblingNode->NextSiblingNode = intruder;

        // Parent

        if (nextSiblingNode->ParentNode is not null) {
            var parentNode = nextSiblingNode->ParentNode;

            // We might be the new last child
            if (parentNode->NextSiblingNode == nextSiblingNode) {
                parentNode->NextSiblingNode = intruder;
            }

            if (parentNode->GetNodeType() is not NodeType.Component) {
                parentNode->ChildCount++;
            }
        }
    }

    private static void EmplaceBeforeSiblings(AtkResNode* intruder, AtkResNode* siblingNode) {
        EmplaceBefore(intruder, GetFirstChild(siblingNode->ParentNode));
    }

    private static void EmplaceAfterSiblings(AtkResNode* intruder, AtkResNode* siblingNode) {
        EmplaceAfter(intruder, GetLastChild(siblingNode->ParentNode));
    }

    private static void EmplaceAsLastChild(AtkResNode* intruder, AtkResNode* parentNode) {
        var lastChild = GetLastChild(parentNode);
        if (lastChild is not null) {
            EmplaceAfter(intruder, lastChild);
        }
        else {
            EmplaceAsOnlyChild(intruder, parentNode);
        }
    }

    private static void EmplaceAsFirstChild(AtkResNode* intruder, AtkResNode* parentNode) {
        var firstChild = GetFirstChild(parentNode);
        if (firstChild is not null) {
            EmplaceBefore(intruder, firstChild);
        }
        else {
            EmplaceAsOnlyChild(intruder, parentNode);
        }
    }

    private static void EmplaceAsOnlyChild(AtkResNode* intruder, AtkResNode* parentNode) {
        intruder->ParentNode = parentNode;

        if (parentNode->GetNodeType() is NodeType.Component) {
            intruder->GetAsAtkComponentNode()->Component->UldManager.RootNode = intruder;
        }
        else {
            parentNode->ChildNode = intruder;
            parentNode->ChildCount++;
            if (parentNode->NodeId is not 1)
                parentNode->NextSiblingNode = intruder;
        }
    }

    public static void DetachNode(AtkResNode* node) {
        if (node is null) return;
        if (node->ParentNode is null) return;

        // Parent

        if (node->ParentNode->ChildNode == node)
            node->ParentNode->ChildNode = node->PrevSiblingNode;

        if (node->ParentNode->NextSiblingNode == node)
            node->ParentNode->NextSiblingNode = GetLogicalNextSibling(node);

        if (node->ParentNode->GetNodeType() is not NodeType.Component) {
            node->ParentNode->ChildCount--;
        }

        // Next Sibling

        if (node->NextSiblingNode != null) {
            if (HasSpecialNextSibling(node)) {
                var nextSiblingNode = GetLogicalNextSibling(node);
                if (nextSiblingNode != null)
                    nextSiblingNode->PrevSiblingNode = node->PrevSiblingNode;
            }
            else {
                node->NextSiblingNode->PrevSiblingNode = node->PrevSiblingNode;
            }
        }

        // Prev Sibling

        if (node->PrevSiblingNode != null) {
            if (HasSpecialNextSibling(node->PrevSiblingNode)) {
                // Do nothing
            }
            else {
                node->PrevSiblingNode->NextSiblingNode = node->NextSiblingNode;
            }
        }
    }

    // Helpers

    private static AtkResNode* GetFirstChild(AtkResNode* node) {
        if (node->GetNodeType() is NodeType.Component) {
            var componentNode = node->GetAsAtkComponentNode();
            return componentNode->Component->UldManager.RootNode;
        }

        return node->ChildNode;
    }

    private static AtkResNode* GetLastChild(AtkResNode* node) {
        var currentNode = GetFirstChild(node);
        if (currentNode is null)
            return null;

        while (currentNode->PrevSiblingNode is not null) {
            currentNode = currentNode->PrevSiblingNode;
        }

        return currentNode;
    }

    private static AtkResNode* GetLogicalNextSibling(AtkResNode* node) {
        if (node->ParentNode is null)
            return null;

        AtkResNode* candidateNext = null;
        var currentNode = node->ParentNode->ChildNode;

        while (true) {
            if (currentNode is null)
                return null;
            if (currentNode == node)
                return candidateNext;
            candidateNext = currentNode;
            currentNode = currentNode->PrevSiblingNode;
        }
    }

    private static bool HasSpecialNextSibling(AtkResNode* node) {
        if (node->NodeId is 1)
            return false;

        if (node->ChildNode is not null)
            return true;

        return false;
    }
}
