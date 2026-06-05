using System;
using System.Linq;
using System.Runtime.CompilerServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using KamiToolKit.BaseTypes;
using KamiToolKit.Classes.Internal;

namespace KamiToolKit.Extensions;

/// <summary>
/// Extension methods for AtkUldManager's.
/// </summary>
public static unsafe class AtkUldManagerExtensions {
    extension(ref AtkUldManager manager) {

        /// <summary>
        /// Adds node and all children nodes to this UldManager's Object List
        /// </summary>
        [OverloadResolutionPriority(1)]
        public void AddNodeToObjectList(NodeBase node) {
            if (!manager.ResourceFlags.HasFlag(AtkUldManagerResourceFlag.Initialized)) return;

            manager.AddNodeToObjectList(node.ResNode);

            foreach (var child in NodeBase.GetLocalChildren(node)) {
                manager.AddNodeToObjectList(child.ResNode);
            }

            manager.UpdateDrawNodeList();
        }

        /// <summary>
        /// Adds just this node to the UldManagers Object List.
        /// </summary>
        [OverloadResolutionPriority(0)]
        public void AddNodeToObjectList(AtkResNode* newNode) {
            if (!manager.ResourceFlags.HasFlag(AtkUldManagerResourceFlag.Initialized)) return;
            if (newNode is null) return;

            // If the node is already in the object list, skip.
            if (manager.IsNodeInObjectList(newNode)) return;

            var oldSize = manager.Objects->NodeCount;
            var newSize = oldSize + 1;

            var newBuffer = (AtkResNode**)NativeMemoryHelper.Realloc(manager.Objects->NodeList, (ulong)(newSize * 8));
            newBuffer[newSize - 1] = newNode;

            manager.Objects->NodeList = newBuffer;
            manager.Objects->NodeCount = newSize;
        }

        /// <summary>
        /// Removes node and all children nodes from this UldManager's Object List
        /// </summary>
        public void RemoveNodeFromObjectList(NodeBase node) {
            if (!manager.ResourceFlags.HasFlag(AtkUldManagerResourceFlag.Initialized)) return;
            manager.RemoveNodeFromObjectList(node.ResNode);

            foreach (var child in NodeBase.GetLocalChildren(node)) {
                manager.RemoveNodeFromObjectList(child.ResNode);
            }

            manager.UpdateDrawNodeList();
        }

        /// <summary>
        /// Removes just this node from the UldManagers Object List.
        /// </summary>
        public void RemoveNodeFromObjectList(AtkResNode* node) {
            if (!manager.ResourceFlags.HasFlag(AtkUldManagerResourceFlag.Initialized)) return;
            if (node is null) return;

            // If the node isn't in the object list, skip.
            if (!manager.IsNodeInObjectList(node)) return;

            var oldSize = manager.Objects->NodeCount;
            var newSize = oldSize - 1;
            var newBuffer = (AtkResNode**)NativeMemoryHelper.Malloc((ulong)(newSize * 8));

            var newIndex = 0;
            foreach (var index in Enumerable.Range(0, oldSize)) {
                if (manager.Objects->NodeList[index] != node) {
                    newBuffer[newIndex] = manager.Objects->NodeList[index];
                    newIndex++;
                }
            }

            NativeMemoryHelper.Free(manager.Objects->NodeList, (ulong)(oldSize * 8));
            manager.Objects->NodeList = newBuffer;
            manager.Objects->NodeCount = newSize;
        }

        /// <summary>
        /// Debug helper for printing a UldManagers entire object list.
        /// </summary>
        public void PrintObjectList() {
            Services.Log.Debug("Beginning NodeList");

            foreach (var index in Enumerable.Range(0, manager.Objects->NodeCount)) {
                var nodePointer = manager.Objects->NodeList[index];
                Services.Log.Debug($"[{index}]: {(nint)nodePointer:X}");
            }
        }

        /// <summary>
        /// Helper to search for a node by id, helpful for AtkLists as the GetNodeById doesn't return duplicated nodes.
        /// </summary>
        public T* SearchNodeById<T>(uint nodeId) where T : unmanaged {
            foreach (var node in manager.Nodes) {
                if (node.Value is not null) {
                    if (node.Value->NodeId == nodeId)
                        return (T*)node.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Default typed SearchNodeById helper.
        /// </summary>
        public AtkResNode* SearchNodeById(uint nodeId)
            => manager.SearchNodeById<AtkResNode>(nodeId);

        private bool IsNodeInObjectList(AtkResNode* node) {
            foreach (var objectNode in manager.ObjectNodeSpan) {
                if (objectNode.Value == node) return true;
            }

            return false;
        }

        private Span<Pointer<AtkResNode>> ObjectNodeSpan
            => new(manager.Objects->NodeList, manager.Objects->NodeCount);
    }
}
