using System;
using System.Collections.Generic;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Hooking;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Dalamud;

namespace KamiToolKit.Controllers;

/// <inheritdoc/>
public class NativeListController : NativeListController<AtkUnitBase, ListItemData>;

/// <inheritdoc/>
public class NativeListController<T> : NativeListController<T, ListItemData> where T : unmanaged;

/// <summary>
/// Controller for modifying native AtkListComponents and their various parts and properties.
/// </summary>
public unsafe class NativeListController<T, TU> : IDisposable where T : unmanaged where TU : ListItemData, new() {

    /// <summary>
    /// Addon name to bind to.
    /// </summary>
    public required string AddonName { get; init; }

    /// <summary>
    /// Delegate that is called when the controller is trying to determine if an element should be modified.
    /// </summary>
    public delegate bool ShouldModifyElementHandler(T* unitBase, TU listItem);

    /// <summary>
    /// Delegate that is called when the list controller is setting up and trying to hook the games node populator.
    /// </summary>
    public delegate AtkComponentListItemRenderer* GetPopulatorNodeHandler(T* addon);

    /// <summary>
    /// Delegate that is called to apply a change to a list entry.
    /// </summary>
    public delegate void UpdateElementHandler(T* unitBase, TU listItem);

    /// <summary>
    /// Delegate that is called to undo a change from a list entry.
    /// </summary>
    public delegate void ResetElementHandler(T* unitBase, TU listItem);

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
    protected readonly List<uint> ModifiedIndexes = [];

    /// <summary>
    /// Enables this native list controller.
    /// </summary>
    /// <remarks>
    /// Warning, it can't properly track modified state if the list is already opened when the controller is enabled.
    /// This must be invoked from the main game thread.
    /// </remarks>
    public void Enable() {
        ThreadSafety.AssertMainThread();

        Services.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, AddonName, OnAddonSetup);
        Services.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, AddonName, OnAddonFinalize);

        var addon = (T*)RaptureAtkUnitManager.Instance()->GetAddonByName(AddonName);
        if (addon is not null) {
            Services.Log.Warning("Caution: ListController was loaded after list was initialized, data may be stale.");
            LoadPopulators(addon);
        }
    }

    /// <summary>
    /// Disables this native list controller.
    /// </summary>
    /// <remarks>
    /// This must be invoked from the main game thread.
    /// </remarks>
    public void Disable() {
        ThreadSafety.AssertMainThread();

        Services.AddonLifecycle.UnregisterListener(OnAddonSetup, OnAddonFinalize);

        onListPopulate?.Dispose();
        onListPopulate = null;

        onRendererPopulate?.Dispose();
        onRendererPopulate = null;
    }

    public void Dispose()
        => Disable();

    private void OnAddonSetup(AddonEvent type, AddonArgs args)
        => LoadPopulators((T*)args.Addon.Address);

    private void OnAddonFinalize(AddonEvent type, AddonArgs args) {
        onListPopulate?.Disable();
        onRendererPopulate?.Disable();

        ModifiedIndexes.Clear();
    }

    private void LoadPopulators(T* addon) {
        var populateMethod = GetPopulatorNode(addon)->Populator;

        if (populateMethod.Populate is not null) {
            onListPopulate ??= Services.GameInteropProvider.HookFromAddress<AtkComponentListItemPopulator.PopulateDelegate>(populateMethod.Populate, OnPopulateDetour);
            onListPopulate?.Enable();
        }

        if (populateMethod.PopulateWithRenderer is not null) {
            onRendererPopulate ??= Services.GameInteropProvider.HookFromAddress<AtkComponentListItemPopulator.PopulateWithRendererDelegate>(populateMethod.PopulateWithRenderer, OnRendererPopulateDetour);
            onRendererPopulate?.Enable();
        }
    }

    private void OnPopulateDetour(AtkEventListener* unitBase, AtkComponentListItemPopulator.ListItemInfo* itemInfo, AtkResNode** nodeList) {
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
            Services.Log.Exception(e);
        }
    }

    private void OnRendererPopulateDetour(AtkEventListener* unitBase, int listItemIndex, AtkResNode** nodeList, AtkComponentListItemRenderer* listItemRenderer) {
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
            Services.Log.Exception(e);
        }
    }

    private Hook<AtkComponentListItemPopulator.PopulateDelegate>? onListPopulate;
    private Hook<AtkComponentListItemPopulator.PopulateWithRendererDelegate>? onRendererPopulate;
}
