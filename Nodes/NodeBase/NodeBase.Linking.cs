using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

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
        
        target.ComponentBase->UldManager.UpdateDrawNodeList();
    }

    internal void DetachNode() {
        NodeLinker.DetachNode(InternalResNode);
    }

    private void AddNodeToUldObjectList(AtkUldManager* uldManager, AtkResNode* newNode) {
        var oldSize = uldManager->Objects->NodeCount;
        var newSize = oldSize + 1;
        var newBuffer = (AtkResNode**) NativeMemoryHelper.Malloc((ulong)(newSize * 8));

        foreach (var index in Enumerable.Range(0, oldSize)) {
            newBuffer[index] = uldManager->Objects->NodeList[index];
        }
        
        newBuffer[newSize - 1] = newNode;
        
        NativeMemoryHelper.Free(uldManager->Objects->NodeList, (ulong)(oldSize * 8));
        uldManager->Objects->NodeList = newBuffer;
        uldManager->Objects->NodeCount = newSize;
    }
}