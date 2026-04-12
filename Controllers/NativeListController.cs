using System;
using System.Collections.Generic;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Dalamud;

namespace KamiToolKit.Controllers;

public class NativeListController : NativeListController<AtkUnitBase, ListItemData>;
public class NativeListController<T> : NativeListController<T, ListItemData> where T : unmanaged;

/// <summary>
/// Controller for modifying native lists.
/// </summary>
/// <typeparam name="T">The concrete addon type.</typeparam>
/// <typeparam name="TU">A data model for this addons list items.</typeparam>
public unsafe class NativeListController<T, TU> : IDisposable where T : unmanaged where TU : ListItemData, new() {
    public required string AddonName { get; init; }

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

    private Hook<AtkComponentListItemPopulator.PopulateDelegate>? onListPopulate;
    private Hook<AtkComponentListItemPopulator.PopulateWithRendererDelegate>? onRendererPopulate;

    public readonly List<uint> ModifiedIndexes = [];

    public void Enable() {
        Services.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, AddonName, OnAddonSetup);
        Services.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, AddonName, OnAddonFinalize);

        Services.Framework.RunOnFrameworkThread(() => {
            var addon = (T*) RaptureAtkUnitManager.Instance()->GetAddonByName(AddonName);
            if (addon is not null) {
                Services.Log.Warning("Caution: ListController was loaded after list was initialized, data may be stale.");
                LoadPopulators(addon);
            }
        });
    }

    public void Disable() {
        Services.AddonLifecycle.UnregisterListener(OnAddonSetup, OnAddonFinalize);

        onListPopulate?.Dispose();
        onListPopulate = null;

        onRendererPopulate?.Dispose();
        onRendererPopulate = null;
    }

    public void Dispose() {
        onListPopulate?.Dispose();
        onListPopulate = null;
        
        onRendererPopulate?.Dispose();
        onRendererPopulate = null;
        
        Disable();
    }

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

    private void OnPopulateDetour(AtkUnitBase* unitBase, AtkComponentListItemPopulator.ListItemInfo* itemInfo, AtkResNode** nodeList) {
        try {
            var listItemData = new TU {
                ItemInfo = itemInfo,
                NodeList = nodeList,
                ItemIndex = itemInfo->ListItemIndex,
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
    
    private void OnRendererPopulateDetour(AtkUnitBase* unitBase, int listItemIndex, AtkResNode** nodeList, AtkComponentListItemRenderer* listItemRenderer) {
        try {
            var listItemData = new TU {
                ItemRenderer = listItemRenderer,
                NodeList = nodeList,
                ItemIndex = listItemIndex,
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

    public delegate bool ShouldModifyElementHandler(T* unitBase, TU listItem);
    public delegate AtkComponentListItemRenderer* GetPopulatorNodeHandler(T* addon);
    public delegate void UpdateElementHandler(T* unitBase, TU listItem);
    public delegate void ResetElementHandler(T* unitBase, TU listItem);
}
