using System;
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
			Log.Verbose($"[NativeController] Attaching [{customNode.GetType()}] to another Custom Node [{targetNode.GetType()}]");
			var addon = GetAddonForNode(targetNode.InternalResNode);

			switch (targetNode) {

				// Don't attach directly to ComponentNode, attach to its managed RootNode
				case ComponentNode componentNode:
					customNode.AttachNode(componentNode, position);
					customNode.EnableEvents(addon);
					return;

				default:
					customNode.AttachNode(targetNode, position);
					customNode.EnableEvents(addon);
					return;
			}
		});

	public void AttachNode(NodeBase customNode, AtkResNode* targetNode, NodePosition position = NodePosition.AsLastChild)
		=> Framework.RunOnFrameworkThread(() => {
			Log.Verbose($"[NativeController] Attaching [{customNode.GetType()}:{(nint) customNode.InternalResNode:X}] to a native AtkResNode");
			var addon = GetAddonForNode(targetNode);

			customNode.RegisterAutoDetach(addon);
			customNode.AttachNode(targetNode, position);
			customNode.EnableEvents(addon);
		});

	public void AttachNode(NodeBase customNode, AtkComponentNode* targetNode, NodePosition position = NodePosition.AfterAllSiblings) {
		Framework.RunOnFrameworkThread(() => {
			if (targetNode->GetNodeType() is not NodeType.Component) {
				Log.Error("TargetNode type was expected to be Component but was not. Aborting attach.");
				return;
			}
			
			Log.Verbose($"[NativeController] Attaching [{customNode.GetType()}:{(nint) customNode.InternalResNode:X}] to a native AtkComponentNode");

			var addon = GetAddonForNode((AtkResNode*) targetNode);
			if (addon is not null) {
				Log.Verbose($"[NativeController] Tried to get Addon from native AtkComponentNode, found: {addon->NameString}");

				customNode.RegisterAutoDetach(addon);
				customNode.AttachNode(targetNode, position);
				customNode.EnableEvents(addon);
			}
			else {
				Log.Error($"[NativeController] Attempted to attach [{customNode.GetType()}:{(nint) customNode.InternalResNode:X}] to a native AtkComponentNode, but could not find parent addon. Aborting.");
			}
		});
	}

	public void AttachNode(NodeBase customNode, NativeAddon targetAddon, NodePosition position = NodePosition.AsLastChild) {
		Framework.RunOnFrameworkThread(() => {
			Log.Verbose($"[NativeController] Attaching [{customNode.GetType()}:{(nint) customNode.InternalResNode:X}] to a Custom Addon [{targetAddon.GetType()}]");

			customNode.AttachNode(targetAddon, position);
			customNode.EnableEvents(targetAddon.InternalAddon);
		});
	}

	public void DetachNode(NodeBase? customNode, Action? disposeAction = null)
		=> Framework.RunOnFrameworkThread(() => {
			if (customNode is not null) {
				Log.Verbose($"[NativeController] Detaching [{customNode.GetType()}:{(nint) customNode.InternalResNode:X}] from all sources.");
			}

			customNode?.UnregisterAutoDetach();
			customNode?.DisableEvents();
			customNode?.DetachNode();
			disposeAction?.Invoke();
		});

	private AtkUnitBase* GetAddonForNode(AtkResNode* node)
		=> RaptureAtkUnitManager.Instance()->GetAddonByNode(node);
}