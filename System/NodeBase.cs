using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.System;

public abstract unsafe partial class NodeBase : IDisposable {

    internal const uint NodeIdBase = 100_000_000;
    protected static readonly List<NodeBase> CreatedNodes = [];

    internal static uint CurrentOffset;

    private bool isDisposed;

    internal abstract AtkResNode* InternalResNode { get; }

    private AtkResNode.Delegates.Destroy destructorFunction = null!;
    
    private AtkResNode.AtkResNodeVirtualTable* virtualTable;

    public void Dispose() {
        // If the node was invalidated before dispose, we want to skip trying to free it.
        if (!IsNodeValid()) {
            isDisposed = true;
            GC.SuppressFinalize(this);
            CreatedNodes.Remove(this);
            return;
        }

        if (!isDisposed) {
            Log.Verbose($"Disposing node {GetType()}");

            ClearFocus();

            // Automatically dispose any fields/properties that are managed nodes.
            VisitChildren(node => {
                ClearFocus();
                node?.Dispose();
            });

            TryForceDetach(false);

            Timeline?.Dispose();
            InternalResNode->Timeline = null;

            DisableEditMode(NodeEditMode.Move | NodeEditMode.Resize);

            Dispose(true);
            GC.SuppressFinalize(this);

            CreatedNodes.Remove(this);
        }

        isDisposed = true;
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
                Marshal.WriteInt64((nint)inputManager, 0x1880, 0);
                AtkStage.Instance()->AtkCollisionManager->IntersectingCollisionNode = null;
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

    ~NodeBase() {
        Dispose(false);
    }

    protected abstract void Dispose(bool disposing);

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
        Experimental.Instance.StaticAtkResNodeDestroyFunction?.Invoke(thisPtr, free);
        DisposeManagedResources();

        // Free our custom virtual table, the game doesn't know this exists and won't clear it on its own.
        NativeMemoryHelper.Free(virtualTable, 0x8 * 4);
    }

    /// <summary>
    /// Use this dispose method to free any non-native resources, KTK will automatically free any native resources on dispose.
    /// </summary>
    protected virtual void DisposeManagedResources() { }
}

public abstract unsafe class NodeBase<T> : NodeBase where T : unmanaged, ICreatable {

    protected NodeBase(NodeType nodeType) {
        ThreadSafety.AssertMainThread("Attempted to allocate a node while not on main thread. This is not supported.");
        
        Log.Verbose($"Creating new node {GetType()}");
        Node = NativeMemoryHelper.Create<T>();

        BuildVirtualTable();

        InternalResNode->Type = nodeType;
        InternalResNode->NodeId = NodeIdBase + CurrentOffset++;

        if (Node is null) {
            throw new Exception($"Unable to allocate memory for {typeof(T)}");
        }

        CreatedNodes.Add(this);
    }

    public T* Node { get; private set; }

    internal sealed override AtkResNode* InternalResNode => (AtkResNode*)Node;

    protected override void Dispose(bool disposing) {
        if (disposing) {
            InternalResNode->Destroy(true);
            Node = null;
        }
    }
}
