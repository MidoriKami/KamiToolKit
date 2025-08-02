using System;
using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using KamiToolKit.Classes;
using KamiToolKit.System;

namespace KamiToolKit.Extensions;

public static unsafe class AtkUldManagerExtensions {

    /// <summary>
    ///     Gets a pointer to the specified node id
    ///     Automatically adds base id, so instead of specifying "100,000,021", you only need to provide "21"
    /// </summary>
    /// <param name="manager">AtkUldManager to search</param>
    /// <param name="nodeId">Node ID to search for</param>
    /// <returns>Pointer to desired node, or null</returns>
    public static AtkResNode* GetCustomNodeById(ref this AtkUldManager manager, uint nodeId)
        => nodeId >= NodeBase.NodeIdBase ? manager.SearchNodeById(nodeId) : manager.SearchNodeById(nodeId + NodeBase.NodeIdBase);

    // WARNING, volatile if called on a component that has nodes from other addons. 
    public static void ResyncData(ref this AtkUldManager parentUldManager) {
        parentUldManager.UpdateDrawNodeList();

        // Process ObjectListAdditions
        foreach (var index in Enumerable.Range(0, parentUldManager.NodeListCount)) {
            var nodePointer = parentUldManager.NodeList[index];

            // If the object list doesn't have this node, then it is supposed to, add it.
            if (!parentUldManager.IsNodeInObjectList(nodePointer)) {
                parentUldManager.AddNodeToObjectList(nodePointer);
            }
        }

        // Process ObjectListRemovals
        foreach (var index in Enumerable.Range(0, parentUldManager.Objects->NodeCount)) {
            var nodePointer = parentUldManager.Objects->NodeList[index];

            // If the DrawList doesn't have this node, then we need to remove it from objects list
            if (!parentUldManager.IsNodeInDrawList(nodePointer)) {
                parentUldManager.RemoveNodeFromObjectList(nodePointer);
            }
        }
    }

    private static bool IsNodeInObjectList(ref this AtkUldManager uldManager, AtkResNode* node) {
        foreach (var objectNode in uldManager.GetObjectsNodeSpan()) {
            if (objectNode.Value == node) return true;
        }

        return false;
    }

    private static bool IsNodeInDrawList(ref this AtkUldManager uldManager, AtkResNode* node) {
        foreach (var drawNode in uldManager.Nodes) {
            if (drawNode.Value == node) return true;
        }

        return false;
    }

    public static void AddNodeToObjectList(ref this AtkUldManager uldManager, AtkResNode* newNode) {
        // If the node is already in the object list, skip.
        if (uldManager.IsNodeInObjectList(newNode)) return;

        var oldSize = uldManager.Objects->NodeCount;
        var newSize = oldSize + 1;
        var newBuffer = (AtkResNode**)NativeMemoryHelper.Malloc((ulong)(newSize * 8));

        if (oldSize > 0) {
            foreach (var index in Enumerable.Range(0, oldSize)) {
                newBuffer[index] = uldManager.Objects->NodeList[index];
            }

            NativeMemoryHelper.Free(uldManager.Objects->NodeList, (ulong)(oldSize * 8));
        }

        newBuffer[newSize - 1] = newNode;

        uldManager.Objects->NodeList = newBuffer;
        uldManager.Objects->NodeCount = newSize;
    }

    public static void RemoveNodeFromObjectList(ref this AtkUldManager uldManager, AtkResNode* node) {
        // If the node isn't in the object list, skip.
        if (!uldManager.IsNodeInObjectList(node)) return;

        var oldSize = uldManager.Objects->NodeCount;
        var newSize = oldSize - 1;
        var newBuffer = (AtkResNode**)NativeMemoryHelper.Malloc((ulong)(newSize * 8));

        var newIndex = 0;
        foreach (var index in Enumerable.Range(0, oldSize)) {
            if (uldManager.Objects->NodeList[index] != node) {
                newBuffer[newIndex] = uldManager.Objects->NodeList[index];
                newIndex++;
            }
        }

        NativeMemoryHelper.Free(uldManager.Objects->NodeList, (ulong)(oldSize * 8));
        uldManager.Objects->NodeList = newBuffer;
        uldManager.Objects->NodeCount = newSize;
    }

    public static void PrintObjectList(ref this AtkUldManager uldManager) {
        Log.Debug("Beginning NodeList");

        foreach (var index in Enumerable.Range(0, uldManager.Objects->NodeCount)) {
            var nodePointer = uldManager.Objects->NodeList[index];
            Log.Debug($"[{index}]: {(nint)nodePointer:X}");
        }
    }

    public static Span<Pointer<AtkResNode>> GetObjectsNodeSpan(ref this AtkUldManager uldManager)
        => new(uldManager.Objects->NodeList, uldManager.Objects->NodeCount);
}
