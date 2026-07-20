using System;
using System.Collections.Generic;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Internal.Classes;

namespace KamiToolKit.BaseTypes;

/// <summary>
/// Generic Base class for all nodes used in KamiToolKit.
/// This is not intended for external use. If you need base type use <see cref="NodeBase"/>.
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

    /// <summary>
    /// Allocates an initializes a <see cref="NodeBase{T}"/>
    /// </summary>
    protected NodeBase(NodeType nodeType) {
        ThreadSafety.AssertMainThread();

        IPluginLog.Get().Verbose($"Creating new node {GetType()}");
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

    /// <inheritdoc />
    protected override void Dispose(bool isNativeDestructor) {
        if (IsDisposed) return;

        try {
            base.Dispose(isNativeDestructor);
        }
        catch (Exception e) {
            IPluginLog.Get().Exception(e);
        } finally {
            if (!isNativeDestructor) {
                OriginalDestroy(this, true);
            }

            KamiToolKitLibrary.AllocatedNodes?.Remove((nint)Node, out _);

            Node = null;
        }
    }

    internal sealed override AtkResNode* ResNode => (AtkResNode*)Node;
}
