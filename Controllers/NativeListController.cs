using System;
using System.Collections.Generic;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Dalamud;

namespace KamiToolKit.Controllers;

/// <summary>
/// Only one or the other field will be valid, be sure to check for null.
/// </summary>
public unsafe class ListItemData {
    public AtkComponentListItemPopulator.ListItemInfo* ItemInfo { get; set; }
    public AtkComponentListItemRenderer* ItemRenderer { get; set; }
}

public unsafe class NativeListController : IDisposable {
    public required string AddonName { get; init; }

    public required ShouldModifyElementHandler ShouldModifyElement { get; init; }
    public required UpdateElementHandler UpdateElement { get; init; }
    public required ResetElementHandler ResetElement { get; init; }
    public required GetPopulatorNodeHandler GetPopulatorNode { get; init; }

    private Hook<AtkComponentListItemPopulator.PopulateDelegate>? onListPopulate;
    private Hook<AtkComponentListItemPopulator.PopulateWithRendererDelegate>? onRendererPopulate;

    public readonly List<uint> ModifiedIndexes = [];

    public Action? OnClose { get; init; }
    public Action? OnOpen { get; init; }

    public void Enable() {
        Services.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, AddonName, OnAddonSetup);
        Services.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, AddonName, OnAddonFinalize);

        Services.Framework.RunOnFrameworkThread(() => {
            var addon = RaptureAtkUnitManager.Instance()->GetAddonByName(AddonName);
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

    public void Dispose()
        => Disable();

    private void OnAddonSetup(AddonEvent type, AddonArgs args)
        => LoadPopulators((AtkUnitBase*)args.Addon.Address);

    private void OnAddonFinalize(AddonEvent type, AddonArgs args) {
        onListPopulate?.Dispose();
        onListPopulate = null;
        
        onRendererPopulate?.Dispose();
        onRendererPopulate = null;
        
        ModifiedIndexes.Clear();

        OnClose?.Invoke();
    }
    
    private void LoadPopulators(AtkUnitBase* addon) {
        var populateMethod = GetPopulatorNode(addon)->Populator;

        if (populateMethod.Populate is not null) {
            onListPopulate = Services.GameInteropProvider.HookFromAddress<AtkComponentListItemPopulator.PopulateDelegate>(populateMethod.Populate, OnPopulateDetour);
            onListPopulate?.Enable();
        }

        if (populateMethod.PopulateWithRenderer is not null) {
            onRendererPopulate = Services.GameInteropProvider.HookFromAddress<AtkComponentListItemPopulator.PopulateWithRendererDelegate>(populateMethod.PopulateWithRenderer, OnRendererPopulateDetour);
            onRendererPopulate?.Enable();
        }

        OnOpen?.Invoke();
    }

    private void OnPopulateDetour(AtkUnitBase* unitBase, AtkComponentListItemPopulator.ListItemInfo* itemInfo, AtkResNode** nodeList) {
        try {
            var listItemData = new ListItemData {
                ItemInfo = itemInfo,
            };
            
            var shouldModifyElement = ShouldModifyElement(unitBase, listItemData, nodeList);

            if (!shouldModifyElement) {
                if (ModifiedIndexes.Contains(itemInfo->ListItem->Renderer->OwnerNode->NodeId)) {
                    ResetElement.Invoke(unitBase, listItemData, nodeList);
                    ModifiedIndexes.Remove(itemInfo->ListItem->Renderer->OwnerNode->NodeId);
                }
            }
            
            onListPopulate!.Original(unitBase, itemInfo, nodeList);

            if (shouldModifyElement) {
                UpdateElement.Invoke(unitBase, listItemData, nodeList);
                ModifiedIndexes.Add(itemInfo->ListItem->Renderer->OwnerNode->NodeId);
            }
        }
        catch (Exception e) {
            Services.Log.Exception(e);
        }
    }
    
    private void OnRendererPopulateDetour(AtkUnitBase* unitBase, int listItemIndex, AtkResNode** nodeList, AtkComponentListItemRenderer* listItemRenderer) {
        try {
            var listItemData = new ListItemData {
                ItemRenderer = listItemRenderer,
            };
            
            var shouldModifyElement = ShouldModifyElement(unitBase, listItemData, nodeList);

            if (!shouldModifyElement) {
                if (ModifiedIndexes.Contains(listItemRenderer->OwnerNode->NodeId)) {
                    ResetElement.Invoke(unitBase, listItemData, nodeList);
                    ModifiedIndexes.Remove(listItemRenderer->OwnerNode->NodeId);
                }
            }
            
            onRendererPopulate!.Original(unitBase, listItemIndex, nodeList, listItemRenderer);

            if (shouldModifyElement) {
                UpdateElement.Invoke(unitBase, listItemData, nodeList);
                ModifiedIndexes.Add(listItemRenderer->OwnerNode->NodeId);
            }
        }
        catch (Exception e) {
            Services.Log.Exception(e);
        }
    }

    public delegate bool ShouldModifyElementHandler(AtkUnitBase* unitBase, ListItemData listItemInfo, AtkResNode** nodeList);
    public delegate AtkComponentListItemRenderer* GetPopulatorNodeHandler(AtkUnitBase* addon);
    public delegate void UpdateElementHandler(AtkUnitBase* unitBase, ListItemData listItemInfo, AtkResNode** nodeList);
    public delegate void ResetElementHandler(AtkUnitBase* unitBase, ListItemData listItemInfo, AtkResNode** nodeList);
}
