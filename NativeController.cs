using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Addon;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.System;
using Serilog.Events;

namespace KamiToolKit;

/// <summary>
///     Controller for custom native nodes, this class is required to attach custom nodes to native ui, this service will
///     also keep track of the allocated nodes to prevent memory leaks.
/// </summary>
public unsafe class NativeController : IDisposable {
    
    public NativeController(IDalamudPluginInterface pluginInterface) {
        pluginInterface.Inject(this);

        // Inject non-Experimental Properties
        pluginInterface.Inject(DalamudInterface.Instance);
        DalamudInterface.Instance.GameInteropProvider.InitializeFromAttributes(DalamudInterface.Instance);

        // Inject Experimental Properties
        pluginInterface.Inject(Experimental.Instance);
        DalamudInterface.Instance.GameInteropProvider.InitializeFromAttributes(Experimental.Instance);

        Experimental.Instance.EnableHooks();

        // Force enable Verbose so that users are able to get advanced logging information on request.
        DalamudInterface.Instance.Log.MinimumLogLevel = LogEventLevel.Verbose;
    }

    public void Dispose() {
        if (MainThreadSafety.TryAssertMainThread()) return;

        NodeBase.DisposeNodes();
        NativeAddon.DisposeAddons();

        Experimental.Instance.DisposeHooks();
    }

    [OverloadResolutionPriority(2)]  
    public void AttachNode(NodeBase customNode, NodeBase targetNode, NodePosition? position = null) {
        if (MainThreadSafety.TryAssertMainThread()) return;
        
        Log.Verbose($"Attaching [{customNode.GetType()}] to another Custom Node [{targetNode.GetType()}]");

        switch (targetNode) {

            // Don't attach directly to ComponentNode, attach to its managed RootNode
            case ComponentNode componentNode:
                customNode.AttachNode(componentNode, position ?? NodePosition.AfterAllSiblings);
                return;

            default:
                customNode.AttachNode(targetNode, position ?? NodePosition.AsLastChild);
                return;
        }
    }

    [OverloadResolutionPriority(1)]  
    public void AttachNode(NodeBase customNode, AtkResNode* targetNode, NodePosition? position = null) {
        if (MainThreadSafety.TryAssertMainThread()) return;

        Log.Verbose($"Attaching [{customNode.GetType()}:{(nint)customNode.ResNode:X}] to a native AtkResNode");
        customNode.AttachNode(targetNode, position ?? NodePosition.AsLastChild);
    }

    public void AttachNode(NodeBase customNode, AtkComponentNode* targetNode, NodePosition position = NodePosition.AfterAllSiblings) {
        if (MainThreadSafety.TryAssertMainThread()) return;
        
        if (targetNode->GetNodeType() is not NodeType.Component) {
            Log.Error("TargetNode type was expected to be Component but was not. Aborting attach.");
            return;
        }

        Log.Verbose($"[NativeController] Attaching [{customNode.GetType()}:{(nint)customNode.ResNode:X}] to a native AtkComponentNode");
        customNode.AttachNode(targetNode, position);
    }

    public void AttachNode(NodeBase customNode, NativeAddon targetAddon, NodePosition? position = null) {
        if (MainThreadSafety.TryAssertMainThread()) return;

        Log.Verbose($"Attaching [{customNode.GetType()}:{(nint)customNode.ResNode:X}] to a Custom Addon [{targetAddon.GetType()}]");

        customNode.AttachNode(targetAddon, position ?? NodePosition.AsLastChild);
    }

    public void DetachNode(NodeBase? customNode) {
        if (MainThreadSafety.TryAssertMainThread()) return;
        
        if (customNode is not null) {
            Log.Verbose($"Detaching [{customNode.GetType()}:{(nint)customNode.ResNode:X}] from all sources.");
        }

        customNode?.DisableEditMode(NodeEditMode.Move | NodeEditMode.Resize);
        customNode?.DetachNode();
    }

    public void DisposeNode<T>(ref T? customNode) where T : NodeBase {
        if (MainThreadSafety.TryAssertMainThread()) return;

        var node = Interlocked.Exchange(ref customNode, null);
        
        if (customNode is not null) {
            Log.Verbose($"Disposing [{customNode.GetType()}:{(nint)customNode.ResNode:X}] from all sources.");
        }

        node?.DisableEditMode(NodeEditMode.Move | NodeEditMode.Resize);
        node?.DetachNode();
        node?.Dispose();
    }
}
