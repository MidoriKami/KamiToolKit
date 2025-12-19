using System;
using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using KamiToolKit.Classes;

namespace KamiToolKit.Extensions;

public static unsafe class AtkUldManagerExtensions {

    /// <param name="manager">AtkUldManager to search</param>
    extension(ref AtkUldManager manager) {
        private bool IsNodeInObjectList(AtkResNode* node) {
            foreach (var objectNode in manager.ObjectNodeSpan) {
                if (objectNode.Value == node) return true;
            }

            return false;
        }

        public bool IsNodeInDrawList(AtkResNode* node) {
            foreach (var drawNode in manager.Nodes) {
                if (drawNode.Value == node) return true;
            }

            return false;
        }

        public void AddNodeToObjectList(AtkResNode* newNode) {
            if (newNode is null) return;

            // If the node is already in the object list, skip.
            if (manager.IsNodeInObjectList(newNode)) return;

            var oldSize = manager.Objects->NodeCount;
            var newSize = oldSize + 1;
            var newBuffer = (AtkResNode**)NativeMemoryHelper.Malloc((ulong)(newSize * 8));

            if (oldSize > 0) {
                foreach (var index in Enumerable.Range(0, oldSize)) {
                    newBuffer[index] = manager.Objects->NodeList[index];
                }

                NativeMemoryHelper.Free(manager.Objects->NodeList, (ulong)(oldSize * 8));
            }

            newBuffer[newSize - 1] = newNode;

            manager.Objects->NodeList = newBuffer;
            manager.Objects->NodeCount = newSize;
        }

        public void RemoveNodeFromObjectList(AtkResNode* node) {
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

        public void PrintObjectList() {
            Log.Debug("Beginning NodeList");

            foreach (var index in Enumerable.Range(0, manager.Objects->NodeCount)) {
                var nodePointer = manager.Objects->NodeList[index];
                Log.Debug($"[{index}]: {(nint)nodePointer:X}");
            }
        }

        public Span<Pointer<AtkResNode>> ObjectNodeSpan => new(manager.Objects->NodeList, manager.Objects->NodeCount);
    }
}
