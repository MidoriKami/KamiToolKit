using System;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace KamiToolKit;

/// <summary>
/// Controller for custom native nodes, this class is required to attach custom nodes to native ui, this service will also keep track of the allocated nodes to prevent memory leaks.
/// </summary>
public unsafe class NativeController : IDisposable {
	[PluginService] private IAddonEventManager AddonEventManager { get; set; } = null!;
	[PluginService] private IFramework Framework { get; set; } = null!;
	[PluginService] private IGameInteropProvider GameInteropProvider { get; set; } = null!;
	
	public NativeController(IDalamudPluginInterface pluginInterface) {
		pluginInterface.Inject(this);
		
		// Inject Experimental Properties
		pluginInterface.Inject(Experimental.Instance);
		GameInteropProvider.InitializeFromAttributes(Experimental.Instance);
		
		Experimental.Instance.EnableHooks();
		
		// Inject non-Experimental Properties
		pluginInterface.Inject(DalamudInterface.Instance);
		GameInteropProvider.InitializeFromAttributes(DalamudInterface.Instance);
	}

	public void Dispose() {
		NodeBase.DetachAndDispose();
		
		Experimental.Instance.DisposeHooks();
	}

	public void AttachToAddon(NodeBase customNode, void* addon, AtkResNode* target, NodePosition position)
		=> Framework.RunOnFrameworkThread(() => InternalAttachToAddon(customNode, (AtkUnitBase*) addon, target, position));

	public void AttachToNode(NodeBase customNode, NodeBase other, NodePosition position)
		=> Framework.RunOnFrameworkThread(() => InternalAttachToNode(customNode, other, position));

	public void DetachFromAddon(NodeBase? customNode, nint addon, Action? disposeAction = null)
		=> DetachFromAddon(customNode, (AtkUnitBase*) addon, disposeAction);
	
	public void DetachFromAddon(NodeBase? customNode, void* addon, Action? disposeAction = null)
		=> DetachFromAddon(customNode, (AtkUnitBase*) addon, disposeAction);
	
	public void DetachFromAddon(NodeBase? customNode, AtkUnitBase* addon, Action? disposeAction = null)
		=> Framework.RunOnFrameworkThread(() => {
			if (customNode is not null) {
				InternalDetachFromAddon(customNode, addon);
			}

			disposeAction?.Invoke();
		});

	public void DetachNode(NodeBase? customNode, Action? disposeAction = null)
		=> Framework.RunOnFrameworkThread(() => {
			customNode?.DetachNode();
			disposeAction?.Invoke();
		});

	private void InternalAttachToAddon(NodeBase customNode, AtkUnitBase* addon, AtkResNode* target, NodePosition position) {
		customNode.TryRegisterAutoDetach(this, addon);

		NodeLinker.AttachNode(customNode.InternalResNode, target, position);
		customNode.EnableEvents(AddonEventManager, addon);
		addon->UldManager.UpdateDrawNodeList();
		addon->UpdateCollisionNodeList(false);
	}
	
	private static void InternalAttachToNode(NodeBase customNode, NodeBase other, NodePosition position)
		=> customNode.AttachNode(other, position);

	private void InternalDetachFromAddon(NodeBase customNode, AtkUnitBase* addon) {
		customNode.TryUnregisterAutoDetach();
		
		customNode.DisableEvents(AddonEventManager);
		customNode.DetachNode();
			
		addon->UldManager.UpdateDrawNodeList();
		addon->UpdateCollisionNodeList(false);
	}
}