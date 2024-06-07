using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes.NodeBase;

public abstract unsafe partial class NodeBase : IDisposable {
    protected static readonly List<IDisposable> CreatedNodes = [];
    
    private bool isDisposed;

    internal abstract AtkResNode* InternalResNode { get; }

    public static void DisposeAllNodes() {
        foreach (var node in CreatedNodes.ToArray()) {
            node.Dispose();
        }
    }

    ~NodeBase() => Dispose(false);

    protected virtual void Dispose(bool disposing) {
        if (disposing) {
            RemoveTooltipEvents();
            RemoveOnClickEvents();
        }
    }

    public void Dispose() {
        if (!isDisposed) {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        isDisposed = true;
    }
}

public abstract unsafe partial class NodeBase<T> : NodeBase where T : unmanaged, ICreatable {
    protected T* InternalNode { get; }

    internal override sealed AtkResNode* InternalResNode => (AtkResNode*) InternalNode;

    protected NodeBase(NodeType nodeType) {
        InternalNode = NativeMemoryHelper.Create<T>();
        InternalResNode->Type = nodeType;

        if (InternalNode is null) {
            throw new Exception($"Unable to allocate memory for {typeof(T)}");
        }
        
        CreatedNodes.Add(this);
    }
    
    protected override void Dispose(bool disposing) {
        if (disposing) {
            NodeLinker.DetachNode(InternalResNode);
        
            InternalResNode->Destroy(false);
            NativeMemoryHelper.UiFree(InternalNode);
        
            CreatedNodes.Remove(this);
            
            base.Dispose(disposing);
        }
    }
}

