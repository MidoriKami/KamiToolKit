using System;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Addon;
using KamiToolKit.Classes;
using KamiToolKit.System;

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
		
		// Inject non-Experimental Properties
		pluginInterface.Inject(DalamudInterface.Instance);
		GameInteropProvider.InitializeFromAttributes(DalamudInterface.Instance);
		
		// Inject Experimental Properties
		pluginInterface.Inject(Experimental.Instance);
		GameInteropProvider.InitializeFromAttributes(Experimental.Instance);
		
		Experimental.Instance.EnableHooks();
	}

	public void Dispose() {
		NodeBase.DetachAndDispose();
		
		Experimental.Instance.DisposeHooks();
	}

	public void InjectAddon(NativeAddon addon) 
		=> Framework.RunOnFrameworkThread(() => RaptureAtkUnitManager.Instance()->InitializeAddon(addon.InternalAddon, addon.InternalName));

	public void AttachToAddon(NodeBase customNode, NativeAddon addon)
		=> Framework.RunOnFrameworkThread(() => InternalAttachToAddon(customNode, addon));

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

	public void DetachFromAddon(NodeBase? customNode, NativeAddon addon, Action? disposeAction = null)
		=> Framework.RunOnFrameworkThread(() => {
			if (customNode is not null && addon.InternalAddon is not null) {
				InternalDetachFromAddon(customNode, addon);
			}
			
			disposeAction?.Invoke();
		});

	public void DetachNode(NodeBase? customNode, Action? disposeAction = null)
		=> Framework.RunOnFrameworkThread(() => {
			customNode?.DetachNode();
			disposeAction?.Invoke();
		});

	private void InternalAttachToAddon(NodeBase customNode, NativeAddon addon) {
		NodeLinker.AttachNode(customNode.InternalResNode, addon.InternalAddon->RootNode, NodePosition.AsLastChild);
		customNode.EnableEvents(AddonEventManager, addon.InternalAddon);
		
		addon.InternalAddon->UldManager.UpdateDrawNodeList();
		addon.InternalAddon->UpdateCollisionNodeList(false);
	}
	
	private void InternalAttachToAddon(NodeBase customNode, AtkUnitBase* addon, AtkResNode* target, NodePosition position) {
		customNode.RegisterAutoDetach(this, addon);

		NodeLinker.AttachNode(customNode.InternalResNode, target, position);
		customNode.EnableEvents(AddonEventManager, addon);

		addon->UldManager.UpdateDrawNodeList();
		addon->UpdateCollisionNodeList(false);
	}
	
	private static void InternalAttachToNode(NodeBase customNode, NodeBase other, NodePosition position)
		=> customNode.AttachNode(other, position);

	private void InternalDetachFromAddon(NodeBase customNode, AtkUnitBase* addon) {
		customNode.UnregisterAutoDetach();
		
		customNode.DisableEvents(AddonEventManager);
		customNode.DetachNode();
			
		addon->UldManager.UpdateDrawNodeList();
		addon->UpdateCollisionNodeList(false);
	}

	private void InternalDetachFromAddon(NodeBase customNode, NativeAddon addon) {
		customNode.DisableEvents(AddonEventManager);
		customNode.DetachNode();
		
		addon.InternalAddon->UldManager.UpdateDrawNodeList();
		addon.InternalAddon->UpdateCollisionNodeList(false);
	}
}