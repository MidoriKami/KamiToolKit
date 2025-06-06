﻿using System;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Addon;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.System;

namespace KamiToolKit;

/// <summary>
/// Controller for custom native nodes, this class is required to attach custom nodes to native ui, this service will also keep track of the allocated nodes to prevent memory leaks.
/// </summary>
public unsafe class NativeController : IDisposable {
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

	public void Dispose()
		=> Framework.RunOnFrameworkThread(() => {
			NodeBase.DisposeNodes();
			NativeAddon.DisposeAddons();
		
			Experimental.Instance.DisposeHooks();
		});

	public void AttachNode(NodeBase customNode, NodeBase targetNode, NodePosition position = NodePosition.AsLastChild)
		=> Framework.RunOnFrameworkThread(() => {
			switch (targetNode) {
			
				// Don't attach directly to ComponentNode, attach to its managed RootNode
				case ComponentNode componentNode:
					customNode.AttachNode(componentNode, position);
					return;
			
				default:
					customNode.AttachNode(targetNode, position);
					return;
			}
		});

	public void AttachNode(NodeBase customNode, AtkResNode* targetNode, NodePosition position = NodePosition.AsLastChild)
		=> Framework.RunOnFrameworkThread(() => {
			var addon = GetAddonForNode(targetNode);
			
			customNode.RegisterAutoDetach(addon);
			customNode.AttachNode(targetNode, position);
			customNode.EnableEvents(addon);
		});

	public void AttachNode(NodeBase customNode, AtkComponentNode* targetNode, NodePosition position = NodePosition.AsLastChild) {
		Framework.RunOnFrameworkThread(() => {
			var addon = GetAddonForNode((AtkResNode*) targetNode);
			
			customNode.RegisterAutoDetach(addon);
			customNode.AttachNode(targetNode, position);
			customNode.EnableEvents(addon);
		});
	}

	public void AttachNode(NodeBase customNode, NativeAddon targetAddon, NodePosition position = NodePosition.AsLastChild) {
		Framework.RunOnFrameworkThread(() => {
			customNode.AttachNode(targetAddon, position);
			customNode.EnableEvents(targetAddon.InternalAddon);
		});
	}
	
	public void DetachNode(NodeBase? customNode, Action? disposeAction = null)
		=> Framework.RunOnFrameworkThread(() => {
			customNode?.UnregisterAutoDetach();
			customNode?.DisableEvents();
			customNode?.DetachNode();
			disposeAction?.Invoke();
		});
	
	private AtkUnitBase* GetAddonForNode(AtkResNode* node) {
		if (Experimental.Instance.GetAddonByNode != null) {
			return Experimental.Instance.GetAddonByNode.Invoke(RaptureAtkUnitManager.Instance(), node);
		}

		return null;
	}
}