using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.System;

public abstract unsafe partial class NodeBase : IDisposable {

    internal const uint NodeIdBase = 100_000_000;
    protected static readonly List<NodeBase> CreatedNodes = [];

    internal static uint CurrentOffset;

    private bool isDisposed;
    private bool isManagedDispose;

    internal abstract AtkResNode* InternalResNode { get; }

    private AtkResNode.Delegates.Destroy destructorFunction = null!;
    private AtkResNode.AtkResNodeVirtualTable* virtualTable;

    public void Dispose() {
        if (MainThreadSafety.TryAssertMainThread()) return;

        if (isDisposed) return;
        isDisposed = true;

        // If the node was invalidated before dispose, we want to skip trying to free it.
        if (!IsNodeValid()) {
            if (!isManagedDispose) {
                Log.Verbose($"Native has disposed node {GetType()}");
                
                Dispose(true, true);

                GC.SuppressFinalize(this);
                CreatedNodes.Remove(this);
            }
            return;
        }

        Log.Verbose($"Disposing node {GetType()}");
        isManagedDispose = true;

        ClearFocus();

        // Automatically dispose any fields/properties that are managed nodes.
        VisitChildren(node => {
            node?.Dispose();
        });

        TryForceDetach(false);

        Timeline?.Dispose();
        InternalResNode->Timeline = null;

        DisableEditMode(NodeEditMode.Move | NodeEditMode.Resize);

        Dispose(true, false);

        GC.SuppressFinalize(this);
        CreatedNodes.Remove(this);
    }

    /// <summary>
    ///     If our node is focused via AtkInputManager, change the focus to be the windows root node instead.
    /// </summary>
    private void ClearFocus() {
        if (InternalResNode is null) return;

        var inputManager = AtkStage.Instance()->AtkInputManager;
        if (inputManager is null) return;

        foreach (ref var focusEntry in inputManager->FocusList) {
            if (focusEntry.AtkEventListener is null) continue;

            // If this focus entry has our custom node focused, unfocus the node.
            if (focusEntry.AtkEventTarget == InternalResNode) {
                Log.Debug($"Custom Node was focused during dispose, Addon: {((AtkUnitBase*)focusEntry.AtkEventListener)->NameString}, unfocusing node.");
                focusEntry.AtkEventTarget = null;
                focusEntry.Unk10 = 0;
                inputManager->FocusedNode = null;
                AtkStage.Instance()->AtkCollisionManager->IntersectingCollisionNode = null;

                var addon = (AtkUnitBase*) focusEntry.AtkEventListener;
                foreach (ref var node in addon->AdditionalFocusableNodes) {
                    if (node.Value == InternalResNode) {
                        Log.Debug("Custom Node was an AdditionalFocusableNode, clearing.");
                        node = null;
                    }
                }
            }
        }
    }

    /// <summary>
    ///     Warning, this is only to ensure there are no memory leaks.
    ///     Ensure you have detached nodes safely from native ui before disposing.
    /// </summary>
    internal static void DisposeNodes() {
        foreach (var node in CreatedNodes.ToArray()) {
            if (!node.IsAttached) continue;
            Log.Debug($"AutoDisposing node {node.GetType()}");

            node.TryForceDetach(true);
            node.Dispose();
        }
    }

    ~NodeBase() => Dispose(false, false);

    /// <summary>
    /// Dispose associated resources. If a resource modifies native state directly guard it with isNativeDestructor
    /// </summary>
    /// <param name="disposing">
    /// Indicates if this specific call should dispose resources or not. This protects against double dispose,
    /// or incorrectly manipulating native state too many times.
    /// </param>
    /// <param name="isNativeDestructor">
    /// Indicates if the dispose call should try to completely clean up all resources,
    /// or if it should only clean up managed resources. When false, be sure to only dispose
    /// resources that exist in managed spaces, as the game has already cleaned up everything else.
    /// </param>
    protected abstract void Dispose(bool disposing, bool isNativeDestructor);

    private bool IsNodeValid() {
        if (InternalResNode is null) return false;
        if (InternalResNode->VirtualTable is null) return false;
        if (InternalResNode->VirtualTable == AtkEventTarget.StaticVirtualTablePointer) return false;

        return true;
    }

    private bool IsDragDropComponent() {
        if (!IsNodeValid()) return false;
        if (NodeType != NodeType.Component) return false;
        var componentNode = (AtkComponentNode*)InternalResNode;
        if (componentNode->Component is null) return false;
        if (componentNode->Component->GetComponentType() is not ComponentType.DragDrop) return false;

        return true;
    }

    public static implicit operator AtkResNode*(NodeBase node) => node.InternalResNode;

    protected void BuildVirtualTable() {
        // Overwrite virtual table with a custom copy,
        // Note: Currently there are only 2 vfuncs, but there's no harm in copying more for if they ever add more vfuncs to the game.
        virtualTable = (AtkResNode.AtkResNodeVirtualTable*)NativeMemoryHelper.Malloc(0x8 * 4);
        NativeMemory.Copy(InternalResNode->VirtualTable, virtualTable, 0x8 * 4);
        InternalResNode->VirtualTable = virtualTable;

        // Pin managed function to virtual table entry
        destructorFunction = NativeDestroyAtkResNode;

        // Replace native destructor with 
        virtualTable->Destroy = (delegate* unmanaged<AtkResNode*, bool, void>) Marshal.GetFunctionPointerForDelegate(destructorFunction);
    }

    private void NativeDestroyAtkResNode(AtkResNode* thisPtr, bool free) {
        AtkResNode.StaticVirtualTablePointer->Destroy(thisPtr, free);
        Dispose();

        // Free our custom virtual table, the game doesn't know this exists and won't clear it on its own.
        NativeMemoryHelper.Free(virtualTable, 0x8 * 4);
    }
}

public abstract unsafe class NodeBase<T> : NodeBase where T : unmanaged, ICreatable {

    protected NodeBase(NodeType nodeType) {
        if (MainThreadSafety.TryAssertMainThread()) return;
        
        Log.Verbose($"Creating new node {GetType()}");
        Node = NativeMemoryHelper.Create<T>();

        BuildVirtualTable();

        InternalResNode->Type = nodeType;
        InternalResNode->NodeId = NodeIdBase + CurrentOffset++;

        if (InternalResNode is null) {
            throw new Exception($"Unable to allocate memory for {typeof(T)}");
        }

        CreatedNodes.Add(this);
    }

    public T* Node { get; private set; }

    internal sealed override AtkResNode* InternalResNode => (AtkResNode*)Node;

    protected override void Dispose(bool disposing, bool isNativeDestructor) {
        if (disposing) {
            if (!isNativeDestructor) {
                InternalResNode->Destroy(true);
            }

            Node = null;
        }
    }
}
