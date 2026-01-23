using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Enums;

namespace KamiToolKit;

public abstract unsafe partial class NodeBase : IDisposable {

    internal const uint NodeIdBase = 100_000_000;
    protected static readonly List<NodeBase> CreatedNodes = [];
    private static int logIndent = -1;

    internal static uint CurrentOffset;

    private bool isDisposed;

    internal abstract AtkResNode* ResNode { get; }
    internal bool IsAddonRootNode;

    private delegate* unmanaged<AtkResNode*, bool, void> originalDestructorFunction;
    private AtkResNode.Delegates.Destroy destructorFunction = null!;
    private AtkResNode.AtkResNodeVirtualTable* virtualTable;

    public void Dispose() {
        try {
            logIndent++;
            LogIndented($"Beginning Dispose for {GetType()}");
            logIndent++;

            if (MainThreadSafety.TryAssertMainThread()) {
                if (Framework.Instance()->IsDestroying) {
                    LogIndented("Game is shutting down, aborting manual dispose.");
                }
                return;
            }

            if (isDisposed) {
                LogIndented("Node was already disposed, skipping.");
                return;
            }

            isDisposed = true;

            if (!IsNodeValid()) {
                Log.Warning("Invalid node, dispose aborted.");
                return;
            }

            LogIndented("Disposing Children");
            foreach (var child in ChildNodes.ToList()) {
                child.Dispose();
            }
            LogIndented("Children Disposed");
            ChildNodes.Clear();

            LogIndented("Disposing Tooltip Events");
            UnregisterTooltipEvents();

            LogIndented("Clearing Native Focus");
            AtkStage.Instance()->ClearNodeFocus(ResNode);

            LogIndented("Detaching From UI");
            DetachNode();

            LogIndented("Disposing Timeline");
            Timeline?.Dispose();
            ResNode->Timeline = null;
            
            LogIndented("Invoking Native Dispose");
            Dispose(true, false);
            GC.SuppressFinalize(this);
            CreatedNodes.Remove(this);

            logIndent--;
            logIndent--;
        }
        catch (Exception e) {
            Log.Exception(e);
            logIndent = 0;
        } 
    }

    private static void LogIndented(string message)
        => Log.Verbose(new string(' ', logIndent * 2) + message);

    /// <summary>
    ///     Warning, this is only to ensure there are no memory leaks.
    ///     Ensure you have detached nodes safely from native ui before disposing.
    /// </summary>
    internal static void DisposeNodes() {
        var leakedNodeCount = CreatedNodes.Count(node => !node.IsAddonRootNode && node.ResNode is not null && node.ResNode->ParentNode is null);

        if (leakedNodeCount is not 0) {
            Log.Warning($"There were {leakedNodeCount} node(s) that were not disposed safely.");
        }

        foreach (var node in CreatedNodes.ToArray()) {
            if (node.ResNode is null) continue;
            if (node.ResNode->ParentNode is not null) continue;
            if (node.IsAddonRootNode) continue;

            Log.Warning($"Forcing disposal of: {node.GetType()}");
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
    protected virtual void Dispose(bool disposing, bool isNativeDestructor) {
        
        // Dispose of managed resources that must be disposed regardless of how dispose is invoked
        DisposeEvents();
        DisableEditMode(NodeEditMode.Move | NodeEditMode.Resize);
    }

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
        Dispose(true, true);
        InvokeOriginalDestructor(thisPtr, free);

        Log.Verbose($"Native has disposed node {GetType()}");
        GC.SuppressFinalize(this);
        CreatedNodes.Remove(this);

        isDisposed = true;
    }

    protected void InvokeOriginalDestructor(AtkResNode* thisPtr, bool free) {
        if (virtualTable is null) return; // Shouldn't be possible, but just in case.
        
        originalDestructorFunction(thisPtr, free);
        NativeMemoryHelper.Free(virtualTable, 0x8 * 4);
        virtualTable = null;
    }
}
