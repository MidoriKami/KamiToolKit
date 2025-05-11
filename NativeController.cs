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
		pluginInterface.Inject(ExperimentalMethods.Instance);
		GameInteropProvider.InitializeFromAttributes(ExperimentalMethods.Instance);
	}

	/// <summary>
	/// Dispose this <em>after</em> removing/disposing any attached nodes.
	/// </summary>
	public void Dispose()
		=> NodeBase.DisposeAllNodes();

	public void AttachToAddon(NodeBase customNode, AtkUnitBase* addon, AtkResNode* target, NodePosition position)
		=> Framework.RunOnFrameworkThread(() => AttachToAddonTask(customNode, addon, target, position));

	private void AttachToAddonTask(NodeBase customNode, AtkUnitBase* addon, AtkResNode* target, NodePosition position) {
		NodeLinker.AttachNode(customNode.InternalResNode, target, position);
		customNode.EnableEvents(AddonEventManager, addon);
		addon->UldManager.UpdateDrawNodeList();
		addon->UpdateCollisionNodeList(false);
	}

	/// <summary>
	/// Warning! Known to be volatile, use at your own risk.
	/// </summary>
	public void AttachToComponent(NodeBase customNode, AtkUnitBase* addon, AtkComponentBase* component, AtkResNode* target, NodePosition position)
		=> Framework.RunOnFrameworkThread(() => AttachToComponentTask(customNode, addon, component, target, position));

	private void AttachToComponentTask(NodeBase customNode, AtkUnitBase* addon, AtkComponentBase* component, AtkResNode* target, NodePosition position) {
		NodeLinker.AttachNode(customNode.InternalResNode, target, position);
		customNode.EnableEvents(AddonEventManager, addon);
		component->UldManager.UpdateDrawNodeList();
	}

	public void AttachToNode(NodeBase customNode, NodeBase other, NodePosition position)
		=> Framework.RunOnFrameworkThread(() => AttachToNodeTask(customNode, other, position));

	private void AttachToNodeTask(NodeBase customNode, NodeBase other, NodePosition position)
		=> customNode.AttachNode(other, position);

	public void DetachFromAddon(NodeBase customNode, AtkUnitBase* addon) {
		customNode.DisableEvents(AddonEventManager);
		customNode.DetachNode();
			
		addon->UldManager.UpdateDrawNodeList();
		addon->UpdateCollisionNodeList(false);
	}

	/// <summary>
	/// Warning! Known to be volatile, use at your own risk.
	/// </summary>
	public void DetachFromComponent(NodeBase customNode, AtkUnitBase* addon, AtkComponentBase* component) {
		customNode.DisableEvents(AddonEventManager);
		customNode.DetachNode();

		component->UldManager.UpdateDrawNodeList();
	}

	public void DetachFromNode(NodeBase customNode)
		=> Framework.RunOnFrameworkThread(customNode.DetachNode);

	public void UpdateEvents(NodeBase node, AtkUnitBase* addon)
		=> Framework.RunOnFrameworkThread(() => UpdateEventsTask(node, addon));

	private void UpdateEventsTask(NodeBase node, AtkUnitBase* addon) {
		node.EnableEvents(AddonEventManager, addon);
		addon->UpdateCollisionNodeList(false);
	}
}