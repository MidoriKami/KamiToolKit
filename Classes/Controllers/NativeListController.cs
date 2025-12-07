using System;
using System.Collections.Generic;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Classes.Controllers;

/// <summary>
/// Only one or the other field will be valid, be sure to check for null.
/// </summary>
public unsafe class ListItemData {
    public AtkComponentListItemPopulator.ListItemInfo* ItemInfo { get; set; }
    public AtkComponentListItemRenderer* ItemRenderer { get; set; }
}

public unsafe class NativeListController : IDisposable {

    public required ShouldModifyElementHandler ShouldModifyElement { get; init; }
    public required UpdateElementHandler UpdateElement { get; init; }
    public required ResetElementHandler ResetElement { get; init; }
    public required GetPopulatorNodeHandler GetPopulatorNode { get; init; }

    private Hook<AtkComponentListItemPopulator.PopulateDelegate>? onListPopulate;
    private Hook<AtkComponentListItemPopulator.PopulateWithRendererDelegate>? onRendererDelegate;

    public readonly List<uint> ModifiedIndexes = [];
    
    public event Action? OnClose {
        add => OnInnerClose += value;
        remove => throw new Exception("Do not remove events, on dispose addon state will be managed properly.");
    }
    
    public event Action? OnOpen {
        add => OnInnerOpen += value;
        remove => throw new Exception("Do not remove events, on dispose addon state will be managed properly.");
    }

    public NativeListController(string addonName) {
        DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, addonName, OnAddonSetup);
        DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, addonName, OnAddonFinalize);
    }

    public void Dispose() {
        DalamudInterface.Instance.AddonLifecycle.UnregisterListener(OnAddonSetup, OnAddonFinalize);
        onListPopulate?.Dispose();
    }

    private void OnAddonSetup(AddonEvent type, AddonArgs args) {
        var addon = (AtkUnitBase*)args.Addon.Address;
        var populateMethod = GetPopulatorNode(addon)->Populator;
        
        onListPopulate = DalamudInterface.Instance.GameInteropProvider.HookFromAddress<AtkComponentListItemPopulator.PopulateDelegate>(populateMethod.Populate, OnPopulateDetour);
        onListPopulate?.Enable();
        
        onRendererDelegate = DalamudInterface.Instance.GameInteropProvider.HookFromAddress<AtkComponentListItemPopulator.PopulateWithRendererDelegate>(populateMethod.PopulateWithRenderer, OnRendererPopulateDetour);
        onRendererDelegate?.Enable();
        
        OnInnerOpen?.Invoke();
    }

    private void OnAddonFinalize(AddonEvent type, AddonArgs args) {
        onListPopulate?.Dispose();
        ModifiedIndexes.Clear();
        
        OnInnerClose?.Invoke();
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
            Log.Exception(e);
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
            
            onRendererDelegate!.Original(unitBase, listItemIndex, nodeList, listItemRenderer);

            if (shouldModifyElement) {
                UpdateElement.Invoke(unitBase, listItemData, nodeList);
                ModifiedIndexes.Add(listItemRenderer->OwnerNode->NodeId);
            }
        }
        catch (Exception e) {
            Log.Exception(e);
        }
    }

    public delegate bool ShouldModifyElementHandler(AtkUnitBase* unitBase, ListItemData listItemInfo, AtkResNode** nodeList);
    public delegate AtkComponentListItemRenderer* GetPopulatorNodeHandler(AtkUnitBase* addon);
    public delegate void UpdateElementHandler(AtkUnitBase* unitBase, ListItemData listItemInfo, AtkResNode** nodeList);
    public delegate void ResetElementHandler(AtkUnitBase* unitBase, ListItemData listItemInfo, AtkResNode** nodeList);

    private Action? OnInnerClose { get; set; }
    private Action? OnInnerOpen { get; set; }
}
