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

    internal abstract AtkResNode* ResNode { get; }

    private delegate* unmanaged<AtkResNode*, bool, void> originalDestructorFunction;
    private AtkResNode.Delegates.Destroy destructorFunction = null!;
    private AtkResNode.AtkResNodeVirtualTable* virtualTable;

    public void Dispose() {
        if (MainThreadSafety.TryAssertMainThread()) return;

        if (isDisposed) return;
        isDisposed = true;

        if (!IsNodeValid()) {
            Log.Warning("WARNING: Node is not valid, attempted to dispose. Aborted.");
            return;
        }

        Log.Verbose($"Disposing node {GetType()}");

        DisposeEvents();

        AtkStage.Instance()->ClearNodeFocus(ResNode);

        // Automatically dispose any fields/properties that are managed nodes.
        VisitChildren(node => node.Dispose());

        TryForceDetach(false);

        Timeline?.Dispose();
        ResNode->Timeline = null;

        DisableEditMode(NodeEditMode.Move | NodeEditMode.Resize);

        Dispose(true, false);

        GC.SuppressFinalize(this);
        CreatedNodes.Remove(this);
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
        if (ResNode is null) return false;
        if (ResNode->VirtualTable is null) return false;
        if (ResNode->VirtualTable == AtkEventTarget.StaticVirtualTablePointer) return false;

        return true;
    }

    public static implicit operator AtkResNode*(NodeBase node) => node.ResNode;
    public static implicit operator AtkEventTarget*(NodeBase node) => &node.ResNode->AtkEventTarget;

    protected void BuildVirtualTable() {
        // Back up original destructor pointer
        originalDestructorFunction = ResNode->VirtualTable->Destroy;
        
        // Overwrite virtual table with a custom copy,
        // Note: Currently there are only 2 vfuncs, but there's no harm in copying more for if they ever add more vfuncs to the game.
        virtualTable = (AtkResNode.AtkResNodeVirtualTable*)NativeMemoryHelper.Malloc(0x8 * 4);
        NativeMemory.Copy(ResNode->VirtualTable, virtualTable, 0x8 * 4);
        ResNode->VirtualTable = virtualTable;

        // Pin managed function to virtual table entry
        destructorFunction = DestructorDetour;

        // Replace native destructor with
        virtualTable->Destroy = (delegate* unmanaged<AtkResNode*, bool, void>) Marshal.GetFunctionPointerForDelegate(destructorFunction);
    }

    private void DestructorDetour(AtkResNode* thisPtr, bool free) {
        if (!isDisposed) {
            Dispose(true, true);
        }

        originalDestructorFunction(thisPtr, free);

        if (!isDisposed) {
            Log.Verbose($"Native has disposed node {GetType()}");
            GC.SuppressFinalize(this);
            CreatedNodes.Remove(this);
        }
        
        // Free our custom virtual table, the game doesn't know this exists and won't clear it on its own.
        NativeMemoryHelper.Free(virtualTable, 0x8 * 4);
        
        isDisposed = true;
    }
}

public abstract unsafe class NodeBase<T> : NodeBase where T : unmanaged, ICreatable {

    protected NodeBase(NodeType nodeType) {
        if (MainThreadSafety.TryAssertMainThread()) return;
        
        Log.Verbose($"Creating new node {GetType()}");
        Node = NativeMemoryHelper.Create<T>();

        BuildVirtualTable();

        ResNode->Type = nodeType;
        ResNode->NodeId = NodeIdBase + CurrentOffset++;
        IsVisible = true;

        if (ResNode is null) {
            throw new Exception($"Unable to allocate memory for {typeof(T)}");
        }

        CreatedNodes.Add(this);
    }

    public T* Node { get; private set; }

    internal sealed override AtkResNode* ResNode => (AtkResNode*)Node;

    public static implicit operator T*(NodeBase<T> node) => (T*) node.ResNode;

    protected override void Dispose(bool disposing, bool isNativeDestructor) {
        if (disposing) {
            if (!isNativeDestructor) {
                DetachNode();
                ResNode->Destroy(true);
            }

            Node = null;
        }
    }
}
