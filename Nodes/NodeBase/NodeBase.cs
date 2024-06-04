using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit;

public abstract class NodeBase {
    // Keeps a reference to all created nodes, so you can simply foreach/Dispose all nodes.
    protected static readonly List<IDisposable> CreatedNodes = [];

    public static void DisposeAllNodes() {
        foreach (var node in CreatedNodes.ToArray()) {
            node.Dispose();
        }
    }
}

[SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "This class is a library utility, it is meant to provide the functionality to other assemblies")]
public abstract unsafe partial class NodeBase<T> : NodeBase, IDisposable where T : unmanaged, ICreatable {
    protected T* InternalNode { get; }
    
    private AtkResNode* InternalResNode => (AtkResNode*) InternalNode;

    private bool isDisposed;

    protected NodeBase(NodeType nodeType) {
        InternalNode = NativeMemoryHelper.Create<T>();
        InternalResNode->Type = nodeType;

        if (InternalNode is null) {
            throw new Exception($"Unable to allocate memory for {typeof(T)}");
        }
        
        CreatedNodes.Add(this);
    }

    ~NodeBase() => Dispose(false);

    protected virtual void Dispose(bool disposing) {
        UnAttachNode();
        RemoveTooltipEvents();
        RemoveOnClickEvents();
        
        InternalResNode->Destroy(false);
        NativeMemoryHelper.UiFree(InternalNode);
        
        CreatedNodes.Remove(this);
    }

    public void Dispose() {
        if (!isDisposed) {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        isDisposed = true;
    }
}

