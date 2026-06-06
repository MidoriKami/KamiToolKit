using System;
using System.Collections.Generic;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Internal.Classes;

namespace KamiToolKit.BaseTypes;

/// <summary>
/// Generic Base class for all nodes used in KamiToolKit.
/// </summary>
public abstract unsafe class NodeBase<T> : NodeBase where T : unmanaged, ICreatable<T> {

    /// <summary>
    /// Gets the typed inner node pointer for this instance.
    /// </summary>
    public T* Node { get; private set; }

    /// <summary>
    /// Implicit operator to seamlessly cast this instance with the contained node type pointer.
    /// </summary>
    public static implicit operator T*(NodeBase<T> node) => node.Node;

    protected NodeBase(NodeType nodeType) {
        ThreadSafety.AssertMainThread();

        Services.Log.Verbose($"Creating new node {GetType()}");
        Node = NativeMemoryHelper.Create<T>();

        if (ResNode is null) {
            throw new Exception($"Unable to allocate memory for {typeof(T)}");
        }

        KamiToolKitLibrary.AllocatedNodes?.TryAdd((nint)Node, GetType());

        BuildVirtualTable();

        ResNode->Type = nodeType;
        ResNode->NodeId = NodeIdBase + CurrentOffset++;
        ResNode->ToggleVisibility(true);

        CreatedNodes.Add(this);
    }

    protected override void Dispose(bool disposing, bool isNativeDestructor) {
        if (disposing) {
            try {
                base.Dispose(disposing, isNativeDestructor);
            }
            catch (Exception e) {
                Services.Log.Exception(e);
            } finally {
                if (!isNativeDestructor) {
                    OriginalDestroy(this, true);
                }

                KamiToolKitLibrary.AllocatedNodes?.Remove((nint)Node, out _);

                Node = null;
            }
        }
    }

    internal sealed override AtkResNode* ResNode => (AtkResNode*)Node;
}
