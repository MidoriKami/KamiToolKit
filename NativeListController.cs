using System;
using System.Collections.Generic;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using ListItemInfo = FFXIVClientStructs.FFXIV.Component.GUI.AtkComponentListItemPopulator.ListItemInfo;
using PopulateDelegate = FFXIVClientStructs.FFXIV.Component.GUI.AtkComponentListItemPopulator.PopulateDelegate;

namespace KamiToolKit;

public unsafe class NativeListController : IDisposable {

    public required ShouldModifyElementHandler ShouldModifyElement { get; init; }
    public required UpdateElementHandler UpdateElement { get; init; }
    public required ResetElementHandler ResetElement { get; init; }
    public required GetPopulatorNodeHandler GetPopulatorNode { get; init; }

    private Hook<PopulateDelegate>? onListPopulate;
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
        var populateMethod = GetPopulatorNode(addon)->Populator.Populate;
        
        onListPopulate = DalamudInterface.Instance.GameInteropProvider.HookFromAddress<PopulateDelegate>(populateMethod, OnPopulateDetour);
        onListPopulate?.Enable();
        
        OnInnerOpen?.Invoke();
    }

    private void OnAddonFinalize(AddonEvent type, AddonArgs args) {
        onListPopulate?.Dispose();
        ModifiedIndexes.Clear();
        
        OnInnerClose?.Invoke();
    }

    private void OnPopulateDetour(AtkUnitBase* unitBase, ListItemInfo* listItemInfo, AtkResNode** nodeList) {
        try {
            var shouldModifyElement = ShouldModifyElement(unitBase, listItemInfo, nodeList);

            if (!shouldModifyElement) {
                if (ModifiedIndexes.Contains(listItemInfo->ListItem->Renderer->OwnerNode->NodeId)) {
                    ResetElement.Invoke(unitBase, listItemInfo, nodeList);
                    ModifiedIndexes.Remove(listItemInfo->ListItem->Renderer->OwnerNode->NodeId);
                }
            }
            
            onListPopulate!.Original(unitBase, listItemInfo, nodeList);

            if (shouldModifyElement) {
                UpdateElement.Invoke(unitBase, listItemInfo, nodeList);
                ModifiedIndexes.Add(listItemInfo->ListItem->Renderer->OwnerNode->NodeId);
            }
        }
        catch (Exception e) {
            Log.Exception(e);
        }
    }

    public delegate bool ShouldModifyElementHandler(AtkUnitBase* unitBase, ListItemInfo* listItemInfo, AtkResNode** nodeList);
    public delegate AtkComponentListItemRenderer* GetPopulatorNodeHandler(AtkUnitBase* addon);
    public delegate void UpdateElementHandler(AtkUnitBase* unitBase, ListItemInfo* listItemInfo, AtkResNode** nodeList);
    public delegate void ResetElementHandler(AtkUnitBase* unitBase, ListItemInfo* listItemInfo, AtkResNode** nodeList);

    private Action? OnInnerClose { get; set; }
    private Action? OnInnerOpen { get; set; }
}
