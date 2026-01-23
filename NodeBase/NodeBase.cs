using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit;

public abstract unsafe class NodeBase<T> : NodeBase where T : unmanaged, ICreatable {
    protected NodeBase(NodeType nodeType) {
        if (MainThreadSafety.TryAssertMainThread()) return;

        Log.Verbose($"Creating new node {GetType()}");
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

    public T* Node { get; private set; }

    internal sealed override AtkResNode* ResNode => (AtkResNode*)Node;

    public static implicit operator T*(NodeBase<T> node) => (T*) node.ResNode;

    protected override void Dispose(bool disposing, bool isNativeDestructor) {
        if (disposing) {
            try {
                base.Dispose(disposing, isNativeDestructor);
            }
            catch (Exception e) {
                Log.Exception(e);
            } 
            finally {
                if (!isNativeDestructor) {
                    InvokeOriginalDestructor(ResNode, true);
                }

                KamiToolKitLibrary.AllocatedNodes?.Remove((nint)Node, out _);
            
                Node = null;
            }
        }
    }
}
