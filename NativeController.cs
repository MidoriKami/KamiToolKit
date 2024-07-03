using System;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace KamiToolKit;


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public unsafe class NativeController : IDisposable {
	[PluginService] private IAddonLifecycle AddonLifecycle { get; set; } // Might be used later, haven't decided yet.
	[PluginService] private IAddonEventManager AddonEventManager { get; set; }
	[PluginService] private IFramework Framework { get; set; }
	
	public NativeController(IDalamudPluginInterface pluginInterface) {
		pluginInterface.Inject(this);
	}

	/// <summary>
	/// Dispose this <em>after</em> removing/disposing any attached nodes.
	/// </summary>
	public void Dispose() {
		NodeBase.DisposeAllNodes();
	}

	public void AttachToAddon(NodeBase customNode, AtkUnitBase* addon, AtkResNode* target, NodePosition position) {
		Framework.RunOnFrameworkThread(() => {
			NodeLinker.AttachNode(customNode.InternalResNode, target, position);
			customNode.EnableEvents(AddonEventManager, addon);
			addon->UldManager.UpdateDrawNodeList();
			addon->UpdateCollisionNodeList(false);
		});
	}

	public void AttachToComponent(NodeBase customNode, AtkUnitBase* addon, AtkComponentBase* component, AtkResNode* target, NodePosition position) {
		Framework.RunOnFrameworkThread(() => {
			NodeLinker.AttachNode(customNode.InternalResNode, target, position);
			customNode.EnableEvents(AddonEventManager, addon);
			component->UldManager.UpdateDrawNodeList();
			addon->UldManager.UpdateDrawNodeList();
			addon->UpdateCollisionNodeList(false);
		});
	}
	
	public void AttachToNode(NodeBase customNode, NodeBase other, NodePosition position) {
		Framework.RunOnFrameworkThread(() => {
			customNode.AttachNode(other, position);
		});
	}

	public void DetachFromAddon(NodeBase customNode, AtkUnitBase* addon) {
		Framework.RunOnFrameworkThread(() => {
			customNode.DisableEvents(AddonEventManager);
			addon->UpdateCollisionNodeList(false);
			
			customNode.DetachNode();
			
			addon->UldManager.UpdateDrawNodeList();
			addon->UpdateCollisionNodeList(false);
		});
	}

	public void DetachFromComponent(NodeBase customNode, AtkUnitBase* addon, AtkComponentBase* component) {
		Framework.RunOnFrameworkThread(() => {
			customNode.DisableEvents(AddonEventManager);
			addon->UpdateCollisionNodeList(false);
			
			customNode.DetachNode();

			component->UldManager.UpdateDrawNodeList();

			addon->UldManager.UpdateDrawNodeList();
			addon->UpdateCollisionNodeList(false);
		});
	}

	public void DetachFromNode(NodeBase customNode) {
		Framework.RunOnFrameworkThread(customNode.DetachNode);
	}

	public void UpdateEvents(NodeBase node, AtkUnitBase* addon) {
		Framework.RunOnFrameworkThread(() => {
			node.EnableEvents(AddonEventManager, addon);
			addon->UpdateCollisionNodeList(false);
		});
	}
}