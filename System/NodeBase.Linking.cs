using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Addon;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace KamiToolKit.System;

public abstract unsafe partial class NodeBase {
    internal void AttachNode(NodeBase target, NodePosition position) {
        NodeLinker.AttachNode(InternalResNode, target.InternalResNode, position);
    }

    /// <summary>
    /// When attaching to a custom ComponentNode, we want to attach to the ULDManager, which should already have a collision node allocated.
    /// As this node is intended to be self contained, it will update the draw list upon any additions.
    /// </summary>
    internal void AttachNode(ComponentNode target, NodePosition position) {
        NodeLinker.AttachNode(InternalResNode, target.ComponentBase->UldManager.RootNode, position);
        AddNodeToUldObjectList(&target.ComponentBase->UldManager, InternalResNode);

        var thisChildren = InternalResNode->ChildNode;
        while (thisChildren is not null) {
            AddNodeToUldObjectList(&target.ComponentBase->UldManager, thisChildren);
            thisChildren = thisChildren->PrevSiblingNode;
        }
        
        target.ComponentBase->UldManager.UpdateDrawNodeList();
    }

    internal void AttachNode(NativeAddon addon, NodePosition position) {
        NodeLinker.AttachNode(InternalResNode, addon.InternalAddon->RootNode, position);
        AddNodeToUldObjectList(&addon.InternalAddon->UldManager, InternalResNode);
        
        var thisChildren = InternalResNode->ChildNode;
        while (thisChildren is not null) {
            AddNodeToUldObjectList(&addon.InternalAddon->UldManager, thisChildren);
            thisChildren = thisChildren->PrevSiblingNode;
        }
        
        addon.InternalAddon->UldManager.UpdateDrawNodeList();
    }

    internal void DetachNode() {
        NodeLinker.DetachNode(InternalResNode);

        if (this is ComponentNode node) {
            RemoveNodeFromUldObjectList(&node.ComponentBase->UldManager, InternalResNode);
            node.ComponentBase->UldManager.UpdateDrawNodeList();
        }
    }

    internal static void AddNodeToUldObjectList(AtkUldManager* uldManager, AtkResNode* newNode) {
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
}