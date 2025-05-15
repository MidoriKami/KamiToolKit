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
		NodeBase.DisposeAllNodes();
		
		Experimental.Instance.DisposeHooks();
	}

	public void AttachToAddon(NodeBase customNode, void* addon, AtkResNode* target, NodePosition position)
		=> Framework.RunOnFrameworkThread(() => InternalAttachToAddon(customNode, (AtkUnitBase*) addon, target, position));

	/// <summary>
	/// Warning! Known to be volatile, use at your own risk.
	/// </summary>
	public void AttachToComponent(NodeBase customNode, AtkUnitBase* addon, AtkComponentBase* component, AtkResNode* target, NodePosition position)
		=> Framework.RunOnFrameworkThread(() => InternalAttachToComponent(customNode, addon, component, target, position));

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

	/// <summary>
	/// Warning! Known to be volatile, use at your own risk.
	/// </summary>
	public void DetachFromComponent(NodeBase? customNode, AtkUnitBase* addon, AtkComponentBase* component, Action? disposeAction = null)
		=> Framework.RunOnFrameworkThread(() => {
			if (customNode is not null) {
				InternalDetachFromComponent(customNode, component);
			}
			
			disposeAction?.Invoke();
		});

	public void DetachNode(NodeBase? customNode, Action? disposeAction = null)
		=> Framework.RunOnFrameworkThread(() => {
			customNode?.DetachNode();
			disposeAction?.Invoke();
		});

	public void UpdateEvents(NodeBase node, AtkUnitBase* addon)
		=> Framework.RunOnFrameworkThread(() => UpdateEventsTask(node, addon));

	private void InternalAttachToAddon(NodeBase customNode, AtkUnitBase* addon, AtkResNode* target, NodePosition position) {
		NodeLinker.AttachNode(customNode.InternalResNode, target, position);
		customNode.EnableEvents(AddonEventManager, addon);
		addon->UldManager.UpdateDrawNodeList();
		addon->UpdateCollisionNodeList(false);
	}

	private void InternalAttachToComponent(NodeBase customNode, AtkUnitBase* addon, AtkComponentBase* component, AtkResNode* target, NodePosition position) {
		NodeLinker.AttachNode(customNode.InternalResNode, target, position);
		customNode.EnableEvents(AddonEventManager, addon);
		component->UldManager.UpdateDrawNodeList();
	}
	
	private void InternalAttachToNode(NodeBase customNode, NodeBase other, NodePosition position)
		=> customNode.AttachNode(other, position);

	private void InternalDetachFromAddon(NodeBase customNode, AtkUnitBase* addon) {
		customNode.DisableEvents(AddonEventManager);
		customNode.DetachNode();
			
		addon->UldManager.UpdateDrawNodeList();
		addon->UpdateCollisionNodeList(false);
	}

	private void InternalDetachFromComponent(NodeBase customNode, AtkComponentBase* component) {
		customNode.DisableEvents(AddonEventManager);
		customNode.DetachNode();

		component->UldManager.UpdateDrawNodeList();
	}

	private void UpdateEventsTask(NodeBase node, AtkUnitBase* addon) {
		node.EnableEvents(AddonEventManager, addon);
		addon->UpdateCollisionNodeList(false);
	}
}