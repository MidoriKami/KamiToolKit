﻿using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.System;

public abstract unsafe partial class NodeBase : IDisposable {
    protected static readonly List<NodeBase> CreatedNodes = [];

    private bool isDisposed;

    internal abstract AtkResNode* InternalResNode { get; }

    /// <summary>
    /// Warning, this is only to ensure there are no memory leaks.
    /// Ensure you have detached nodes safely from native ui before disposing.
    /// </summary>
    internal static void DetachAndDispose() {
        foreach (var node in CreatedNodes.ToArray()) {
            node.Dispose();
        }
    }

    ~NodeBase() => Dispose(false);

    protected abstract void Dispose(bool disposing);

    public void Dispose() {
        if (!isDisposed) {
            Log.Debug($"[KamiToolKit] Disposing node {GetType()}");

            TryForceDetach();

            Dispose(true);
            GC.SuppressFinalize(this);

            CreatedNodes.Remove(this);
        }
        
        isDisposed = true;
    }
}

public abstract unsafe class NodeBase<T> : NodeBase where T : unmanaged, ICreatable {
    protected T* InternalNode { get; private set; }

    internal override sealed AtkResNode* InternalResNode => (AtkResNode*) InternalNode;

    protected NodeBase(NodeType nodeType) {
        Log.Debug($"[KamiToolKit] Creating new node {GetType()}");
        InternalNode = NativeMemoryHelper.Create<T>();
        InternalResNode->Type = nodeType;

        if (InternalNode is null) {
            throw new Exception($"Unable to allocate memory for {typeof(T)}");
        }
        
        CreatedNodes.Add(this);
    }
    
    protected override void Dispose(bool disposing) {
        if (disposing) {
            InternalResNode->Destroy(false);
            NativeMemoryHelper.UiFree(InternalNode);
            InternalNode = null;
        }
    }
}

