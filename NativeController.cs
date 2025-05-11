using System;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
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
		pluginInterface.Inject(Experimental.Instance);
		GameInteropProvider.InitializeFromAttributes(Experimental.Instance);
	}

	public void Dispose()
		=> NodeBase.DisposeAllNodes();

	public void AttachToAddon(NodeBase customNode, AtkUnitBase* addon, AtkResNode* target, NodePosition position) {
		if (ThreadSafety.IsMainThread) {
			InternalAttachToAddon(customNode, addon, target, position);
		}
		else {
			Framework.RunOnFrameworkThread(() => InternalAttachToAddon(customNode, addon, target, position));
		}
	}

	/// <summary>
	/// Warning! Known to be volatile, use at your own risk.
	/// </summary>
	public void AttachToComponent(NodeBase customNode, AtkUnitBase* addon, AtkComponentBase* component, AtkResNode* target, NodePosition position) {
		if (ThreadSafety.IsMainThread) {
			InternalAttachToComponent(customNode, addon, component, target, position);
		}
		else {
			Framework.RunOnFrameworkThread(() => InternalAttachToComponent(customNode, addon, component, target, position));
		}
	}

	public void AttachToNode(NodeBase customNode, NodeBase other, NodePosition position) {
		if (ThreadSafety.IsMainThread) {
			InternalAttachToNode(customNode, other, position);
		}
		else {
			Framework.RunOnFrameworkThread(() => InternalAttachToNode(customNode, other, position));
		}
	}

	public void DetachFromAddon(NodeBase customNode, AtkUnitBase* addon) {
		if (ThreadSafety.IsMainThread) {
			InternalDetachFromAddon(customNode, addon);
		}
		else {
			Framework.RunOnFrameworkThread(() => InternalDetachFromAddon(customNode, addon));
		}
	}

	/// <summary>
	/// Warning! Known to be volatile, use at your own risk.
	/// </summary>
	public void DetachFromComponent(NodeBase customNode, AtkUnitBase* addon, AtkComponentBase* component) {
		if (ThreadSafety.IsMainThread) {
			InternalDetachFromComponent(customNode, component);
		}
		else {
			Framework.RunOnFrameworkThread(() => InternalDetachFromComponent(customNode, component));
			
		}
	}

	public void DetachFromNode(NodeBase customNode) {
		if (ThreadSafety.IsMainThread) {
			customNode.DetachNode();
		}
		else {
			Framework.RunOnFrameworkThread(customNode.DetachNode);
		}
	}

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