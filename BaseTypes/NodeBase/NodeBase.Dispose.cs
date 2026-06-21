using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Enums;
using KamiToolKit.Internal.Classes;

namespace KamiToolKit.BaseTypes;

/// <summary>
/// Abstract base class for all nodes used in KamiToolKit.
/// </summary>
public abstract unsafe partial class NodeBase : IDisposable {

    /// <summary>
    /// Implicit operator to convert this instance to a AtkResNode* for cleaner interop.
    /// </summary>
    public static implicit operator AtkResNode*(NodeBase node) => node.ResNode;

    /// <summary>
    /// Implicit operator to convert this instance to a AtkEventTarget* for cleaner interop.
    /// </summary>
    public static implicit operator AtkEventTarget*(NodeBase node) => &node.ResNode->AtkEventTarget;

    /// <summary>
    /// Gets the list of all allocated nodes for this KamiToolKit instance.
    /// </summary>
    protected static List<NodeBase> CreatedNodes { get; } = [];

    /// <summary>
    /// Disposes this instance. Has double dispose guards.
    /// </summary>
    /// <remarks>
    /// Must be invoked from the main game thread.
    /// </remarks>
    public void Dispose() {
        try {
            logIndent++;
            LogIndented($"Beginning Dispose for {GetType()}");
            logIndent++;

            if (isDisposed) {
                LogIndented("Node was already disposed, skipping.");
                return;
            }

            if (Services.Framework.IsFrameworkUnloading) {
                LogIndented("Game is shutting down, aborting manual dispose.");
                return;
            }

            if (!ThreadSafety.IsMainThread) {
                LogIndented($"{GetType()}'s Dispose must be called from the main thread.");
                return;
            }

            isDisposed = true;

            if (!IsNodeValid()) {
                Services.Log.Warning("Invalid node, dispose aborted.");
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
        }
        catch (Exception e) {
            Services.Log.Exception(e);
        } finally {
            logIndent--;
            LogIndented("Dispose Complete");
            logIndent--;
        }
    }

    internal const uint NodeIdBase = 100_000_000;
    internal static uint CurrentOffset;

    internal abstract AtkResNode* ResNode { get; }
    internal bool IsAddonRootNode;

    private static int logIndent = -1;

    private bool isDisposed;

    private AtkResNode.Delegates.Destroy destroyFunction = null!;

    private AtkResNode.AtkResNodeVirtualTable* originalVirtualTable;
    private AtkResNode.AtkResNodeVirtualTable* modifiedVirtualTable;

    private static void LogIndented(string message)
        => Services.Log.Verbose(new string(' ', logIndent * 2) + message);

    internal static void WarnLeakedNodes() {
        var leakedNodeCount = CreatedNodes.Count(node => !node.IsAddonRootNode && node.ResNode is not null && node.ResNode->ParentNode is null);

        if (leakedNodeCount is not 0) {
            Services.Log.Warning($"There were {leakedNodeCount} node(s) that were not disposed safely.");
        }

        foreach (var node in CreatedNodes.ToArray()) {
            if (node.ResNode is null) continue;
            if (node.ResNode->ParentNode is not null) continue;
            if (node.IsAddonRootNode) continue;

            Services.Log.Warning($"Forcing disposal of: {node.GetType()}");
        }
    }

    /// <summary>
    /// Warning, this is only to ensure there are no memory leaks.
    /// Ensure you have detached nodes safely from native ui before disposing.
    /// </summary>
    internal static void DisposeNodes() {
        foreach (var node in CreatedNodes.ToArray()) {
            if (node.ResNode is null) continue;
            if (node.ResNode->ParentNode is not null) continue;
            if (node.IsAddonRootNode) continue;

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

    /// <summary>
    /// Replaces the nodes entire virtual table to ensure that C#'s managed space gets notified of the games unmanaged node dtor.
    /// </summary>
    protected void BuildVirtualTable() {
        // Back up original destructor pointer
        originalVirtualTable = ResNode->VirtualTable;

        // Overwrite virtual table with a custom copy,
        // Note: Currently there are only 2 vfuncs, but there's no harm in copying more for if they ever add more vfuncs to the game.
        modifiedVirtualTable = (AtkResNode.AtkResNodeVirtualTable*)NativeMemoryHelper.Malloc(0x8 * 4);
        NativeMemory.Copy(ResNode->VirtualTable, modifiedVirtualTable, 0x8 * 4);
        ResNode->VirtualTable = modifiedVirtualTable;

        // Pin managed function to virtual table entry
        destroyFunction = Destroy;

        // Replace native destructor with
        modifiedVirtualTable->Destroy = (delegate* unmanaged<AtkResNode*, bool, void>)Marshal.GetFunctionPointerForDelegate(destroyFunction);
    }

    /// <summary>
    /// Pinned managed function that is used to replace the native virtual tables dtor function pointer.
    /// </summary>
    protected void Destroy(AtkResNode* thisPtr, bool free) {
        Dispose(true, true);

        originalVirtualTable->Destroy(thisPtr, free);

        NativeMemoryHelper.Free(modifiedVirtualTable, 0x8 * 4);
        modifiedVirtualTable = null;

        Services.Log.Verbose($"Native has disposed node {GetType()}");
        GC.SuppressFinalize(this);
        CreatedNodes.Remove(this);

        isDisposed = true;
    }

    // To be invoked from NodeBase.Dispose(bool, bool).
    /// <summary>
    /// Invokes the original games destroy function without calling back to the native disposal method.
    /// </summary>
    /// <remarks>
    /// This is intended to be used from <see cref="NodeBase"/> after the managed disposal functions have been invoked.
    /// </remarks>
    protected void OriginalDestroy(AtkResNode* thisPtr, bool free) {
        originalVirtualTable->Destroy(thisPtr, free);

        NativeMemoryHelper.Free(modifiedVirtualTable, 0x8 * 4);
        modifiedVirtualTable = null;

        GC.SuppressFinalize(this);
        CreatedNodes.Remove(this);

        isDisposed = true;
    }
}
