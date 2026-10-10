using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.BaseTypes.ComponentNode;
using KamiToolKit.Classes;
using KamiToolKit.Internal.Classes;

namespace KamiToolKit.Controllers;

/// <inheritdoc/>
public class NativeListController : NativeListController<AtkUnitBase, ListItemData>;

/// <inheritdoc/>
public class NativeListController<T> : NativeListController<T, ListItemData> where T : unmanaged;

/// <summary>
/// Controller for modifying native AtkListComponents and their various parts and properties.
/// </summary>
public class NativeListController<T, TU> : IDisposable, IAsyncDisposable where T : unmanaged where TU : ListItemData, new() {

    /// <summary>
    /// Addon name to bind to.
    /// </summary>
    public required string AddonName { get; init; }

    /// <summary>
    /// Delegate that is called when the controller is trying to determine if an element should be modified.
    /// </summary>
    public unsafe delegate bool ShouldModifyElementHandler(T* unitBase, TU listItem);

    /// <summary>
    /// Delegate that is called when the list controller is setting up and trying to hook the games node populator.
    /// </summary>
    public unsafe delegate AtkComponentListItemRenderer* GetPopulatorNodeHandler(T* addon);

    /// <summary>
    /// Delegate that is called to apply a change to a list entry.
    /// </summary>
    public unsafe delegate void UpdateElementHandler(T* unitBase, TU listItem);

    /// <summary>
    /// Delegate that is called to undo a change from a list entry.
    /// </summary>
    public unsafe delegate void ResetElementHandler(T* unitBase, TU listItem);

    /// <summary>
    /// Define a function that will return true if the provided list item should be modified by this controller.
    /// </summary>
    /// <remarks>
    /// If no function is defined it will be assumed that each line should be edited.
    /// </remarks>
    public ShouldModifyElementHandler? ShouldModifyElement { get; init; }

    /// <summary>
    /// Define how specifically you want the list item to be modified.
    /// </summary>
    public UpdateElementHandler? UpdateElement { get; init; }

    /// <summary>
    /// Define how specifically you want the list item to be reset.
    /// </summary>
    public ResetElementHandler? ResetElement { get; init; }

    /// <summary>
    /// Function that gets the root ComponentItemRenderer to extract the populator functions from.
    /// </summary>
    public required GetPopulatorNodeHandler GetPopulatorNode { get; init; }

    /// <summary>
    /// List of modified node indexes.
    /// </summary>
    public List<uint> ModifiedIndexes { get; } = [];

    /// <summary>
    /// Adds an attached node to the list item's collision list. Added nodes are owned by the controller.
    /// </summary>
    /// <param name="listItem">List item containing the node.</param>
    /// <param name="node">Node to add.</param>
    /// <remarks>
    /// Configure the node's events before adding it. This must be invoked from the main game thread.
    /// </remarks>
    public unsafe void AddNode(TU listItem, NodeBase node) {
        ThreadSafety.AssertMainThread();
        if (attachments.ContainsKey(node)) return;

        var renderer = listItem.ItemRenderer;
        if (renderer is null && listItem.ItemInfo is not null) {
            renderer = listItem.ItemInfo->ListItem->Renderer;
        }

        if (renderer is null || renderer->UldManager.Objects is null) {
            throw new ArgumentException("List item has no initialized renderer.", nameof(listItem));
        }

        if (node.ResNode is null) {
            throw new ObjectDisposedException(nameof(node));
        }

        if (node.ParentUldManager != &renderer->UldManager || node.ParentAddon is null || node.ParentAddon->NameString != AddonName) {
            throw new ArgumentException("Node must be attached to a renderer owned by this controller.", nameof(node));
        }

        if (node is ComponentNode || NodeBase.GetLocalChildren(node).Any(child => child is ComponentNode || child.NodeFlags.HasFlag(NodeFlags.RespondToMouse))) {
            throw new ArgumentException("Component nodes and interactive children are not supported.", nameof(node));
        }

        var attachment = new NativeListAttachment(node, renderer, attachments);
        try {
            attachment.Attach();
        }
        catch {
            attachment.Dispose();
            throw;
        }
    }

    /// <summary>
    /// Removes and disposes a node owned by this controller.
    /// </summary>
    /// <param name="node">Node to remove.</param>
    /// <remarks>
    /// This must be invoked from the main game thread.
    /// </remarks>
    public void RemoveNode(NodeBase node) {
        ThreadSafety.AssertMainThread();
        if (attachments.TryGetValue(node, out var attachment)) {
            attachment.Dispose();
        }
    }

    /// <summary>
    /// Enables this native list controller.
    /// </summary>
    /// <remarks>
    /// Warning, it can't properly track modified state if the list is already opened when the controller is enabled.
    /// This must be invoked from the main game thread.
    /// </remarks>
    public unsafe void Enable() {
        ThreadSafety.AssertMainThread();

        IAddonLifecycle.Get().RegisterListener(AddonEvent.PostSetup, AddonName, OnAddonSetup);
        IAddonLifecycle.Get().RegisterListener(AddonEvent.PreFinalize, AddonName, OnAddonFinalize);

        var addon = (T*)RaptureAtkUnitManager.Instance()->GetAddonByName(AddonName);
        if (addon is not null) {
            IPluginLog.Get().Warning("Caution: ListController was loaded after list was initialized, data may be stale.");
            LoadPopulators(addon);
        }
    }

    /// <summary>
    /// Enables this native list controller.
    /// </summary>
    public async Task EnableAsync() {
        IAddonLifecycle.Get().RegisterListener(AddonEvent.PostSetup, AddonName, OnAddonSetup);
        IAddonLifecycle.Get().RegisterListener(AddonEvent.PreFinalize, AddonName, OnAddonFinalize);

        await IFramework.Get().Run(() => {
            unsafe {
                var addon = (T*)RaptureAtkUnitManager.Instance()->GetAddonByName(AddonName);
                if (addon is not null) {
                    IPluginLog.Get().Warning("Caution: ListController was loaded after list was initialized, data may be stale.");
                    LoadPopulators(addon);
                }
            }
        });
    }

    /// <summary>
    /// Disables this native list controller.
    /// </summary>
    /// <remarks>
    /// This must be invoked from the main game thread.
    /// </remarks>
    public void Disable() {
        ThreadSafety.AssertMainThread();

        IAddonLifecycle.Get().UnregisterListener(OnAddonSetup, OnAddonFinalize);

        onListPopulate?.Disable();
        onRendererPopulate?.Disable();
        RemoveNodes();

        onListPopulate?.Dispose();
        onListPopulate = null;

        onRendererPopulate?.Dispose();
        onRendererPopulate = null;
    }

    /// <summary>
    /// Disables this native list controller.
    /// </summary>
    public async Task DisableAsync() {
        await IFramework.Get().Run(() => {
            IAddonLifecycle.Get().UnregisterListener(OnAddonSetup, OnAddonFinalize);
            onListPopulate?.Disable();
            onRendererPopulate?.Disable();
            RemoveNodes();
        });

        await onListPopulate.DisposeAsync();
        onListPopulate = null;

        await onRendererPopulate.DisposeAsync();
        onRendererPopulate = null;
    }

    /// <inheritdoc />
    public void Dispose()
        => Disable();

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
        => await DisableAsync();

    private unsafe void OnAddonSetup(AddonEvent type, AddonArgs args)
        => LoadPopulators((T*)args.Addon.Address);

    private void OnAddonFinalize(AddonEvent type, AddonArgs args) {
        onListPopulate?.Disable();
        onRendererPopulate?.Disable();

        RemoveNodes();

        ModifiedIndexes.Clear();
    }

    private unsafe void LoadPopulators(T* addon) {
        var populateMethod = GetPopulatorNode(addon)->Populator;

        if (populateMethod.Populate is not null) {
            onListPopulate ??= IGameInteropProvider.Get().HookFromAddress<AtkComponentListItemPopulator.PopulateDelegate>(populateMethod.Populate, OnPopulateDetour);
            onListPopulate?.Enable();
        }

        if (populateMethod.PopulateWithRenderer is not null) {
            onRendererPopulate ??= IGameInteropProvider.Get().HookFromAddress<AtkComponentListItemPopulator.PopulateWithRendererDelegate>(populateMethod.PopulateWithRenderer, OnRendererPopulateDetour);
            onRendererPopulate?.Enable();
        }
    }

    private unsafe void OnPopulateDetour(AtkEventListener* unitBase, AtkComponentListItemPopulator.ListItemInfo* itemInfo, AtkResNode** nodeList) {
        try {
            var listItemNode = itemInfo->ListItem->Renderer->OwnerNode;

            var parentAddon = RaptureAtkUnitManager.Instance()->GetAddonByNode((AtkResNode*)listItemNode);
            if (parentAddon is null || parentAddon->NameString != AddonName) {
                onListPopulate!.Original(unitBase, itemInfo, nodeList);
                return;
            }

            var listItemData = new TU {
                ItemInfo = itemInfo,
                NodeList = nodeList,
                ItemIndex = itemInfo->ListItemIndex,
                NodeId = itemInfo->ListItem->Renderer->OwnerNode->NodeId,
            };

            var shouldModifyElement = ShouldModifyElement?.Invoke((T*)unitBase, listItemData) ?? true;

            if (!shouldModifyElement) {
                if (ModifiedIndexes.Contains(itemInfo->ListItem->Renderer->OwnerNode->NodeId)) {
                    ResetElement?.Invoke((T*)unitBase, listItemData);
                    ModifiedIndexes.Remove(itemInfo->ListItem->Renderer->OwnerNode->NodeId);
                }
            }

            onListPopulate!.Original(unitBase, itemInfo, nodeList);

            if (shouldModifyElement) {
                UpdateElement?.Invoke((T*)unitBase, listItemData);
                ModifiedIndexes.Add(itemInfo->ListItem->Renderer->OwnerNode->NodeId);
            }
        }
        catch (Exception e) {
            IPluginLog.Get().Exception(e);
        }
    }

    private unsafe void OnRendererPopulateDetour(AtkEventListener* unitBase, int listItemIndex, AtkResNode** nodeList, AtkComponentListItemRenderer* listItemRenderer) {
        try {
            var listItemNode = listItemRenderer->OwnerNode;

            var parentAddon = RaptureAtkUnitManager.Instance()->GetAddonByNode((AtkResNode*)listItemNode);
            if (parentAddon is null || parentAddon->NameString != AddonName) {
                onRendererPopulate!.Original(unitBase, listItemIndex, nodeList, listItemRenderer);
                return;
            }

            var listItemData = new TU {
                ItemRenderer = listItemRenderer,
                NodeList = nodeList,
                ItemIndex = listItemIndex,
                NodeId = listItemRenderer->OwnerNode->NodeId,
            };

            var shouldModifyElement = ShouldModifyElement?.Invoke((T*)unitBase, listItemData) ?? true;

            if (!shouldModifyElement) {
                if (ModifiedIndexes.Contains(listItemRenderer->OwnerNode->NodeId)) {
                    ResetElement?.Invoke((T*)unitBase, listItemData);
                    ModifiedIndexes.Remove(listItemRenderer->OwnerNode->NodeId);
                }
            }

            onRendererPopulate!.Original(unitBase, listItemIndex, nodeList, listItemRenderer);

            if (shouldModifyElement) {
                UpdateElement?.Invoke((T*)unitBase, listItemData);
                ModifiedIndexes.Add(listItemRenderer->OwnerNode->NodeId);
            }
        }
        catch (Exception e) {
            IPluginLog.Get().Exception(e);
        }
    }

    private Hook<AtkComponentListItemPopulator.PopulateDelegate>? onListPopulate;
    private Hook<AtkComponentListItemPopulator.PopulateWithRendererDelegate>? onRendererPopulate;
    private readonly Dictionary<NodeBase, NativeListAttachment> attachments = [];

    private void RemoveNodes() {
        foreach (var attachment in attachments.Values.ToArray()) {
            attachment.Dispose();
        }
    }
}
