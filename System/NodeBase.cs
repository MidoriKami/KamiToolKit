﻿using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.System;

public abstract unsafe partial class NodeBase : IDisposable {
    protected static readonly List<NodeBase> CreatedNodes = [];

    private bool isDisposed;
    
    internal const uint NodeIdBase = 100_000_000;

    internal static uint CurrentOffset;
    
    internal abstract AtkResNode* InternalResNode { get; }

    /// <summary>
    /// Warning, this is only to ensure there are no memory leaks.
    /// Ensure you have detached nodes safely from native ui before disposing.
    /// </summary>
    internal static void DisposeNodes() {
        foreach (var node in CreatedNodes.ToArray()) {
            if (!node.IsAttached) continue;
            Log.Debug($"AutoDisposing node {node.GetType()}");
            
            node.TryForceDetach(true);
            node.Dispose();
        }
    }

    ~NodeBase() => Dispose(false);

    protected abstract void Dispose(bool disposing);

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
            
            // Automatically dispose any fields/properties that are managed nodes.
            VisitChildren(node => node?.Dispose());

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

    private bool IsNodeValid() {
        if (InternalResNode is null) return false;
        if (InternalResNode->VirtualTable is null) return false;
        if ((nint) InternalResNode->VirtualTable == Experimental.Instance.AtkEventListenerVirtualTable) return false;

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
    
    public static explicit operator AtkResNode*(NodeBase node) => node.InternalResNode;
}

public abstract unsafe class NodeBase<T> : NodeBase where T : unmanaged, ICreatable {
    internal T* InternalNode { get; private set; }

    internal override sealed AtkResNode* InternalResNode => (AtkResNode*) InternalNode;

    public static explicit operator T*(NodeBase<T> node) => (T*)node.InternalResNode;

    protected NodeBase(NodeType nodeType) {
        Log.Verbose($"Creating new node {GetType()}");
        InternalNode = NativeMemoryHelper.Create<T>();
        InternalResNode->Type = nodeType;
        InternalResNode->NodeId = NodeIdBase + CurrentOffset++;

        if (InternalNode is null) {
            throw new Exception($"Unable to allocate memory for {typeof(T)}");
        }
        
        CreatedNodes.Add(this);
    }
    
    protected override void Dispose(bool disposing) {
        if (disposing) {
            InternalResNode->Destroy(true);
            InternalNode = null;
        }
    }
}

