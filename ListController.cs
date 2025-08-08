using System;
using System.Collections.Generic;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using ListItemInfo = FFXIVClientStructs.FFXIV.Component.GUI.AtkComponentListItemPopulator.ListItemInfo;

namespace KamiToolKit;

/// <summary>
/// Controller for modifying how a ListPopulator works
/// </summary>
public abstract unsafe class ListController<T> : IDisposable where T : unmanaged {
    
    private Hook<AtkComponentListItemPopulator.PopulateDelegate>? onListPopulate;
    public readonly List<uint> ModifiedIndexes = [];
    
    protected ListController(string addonName) {
        DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, addonName, OnAddonSetup);
        DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, addonName, OnAddonFinalize);
    }
    
    private void OnAddonSetup(AddonEvent type, AddonArgs args) {
        var addon = (T*)args.Addon.Address;
        var populateMethod = GetPopulateDelegate(addon);
        
        onListPopulate = DalamudInterface.Instance.GameInteropProvider.HookFromAddress<AtkComponentListItemPopulator.PopulateDelegate>(populateMethod, OnPopulateDetour);
        onListPopulate?.Enable();
        
        OnOpen?.Invoke();
    }
    
    private void OnAddonFinalize(AddonEvent type, AddonArgs args) {
        onListPopulate?.Dispose();
        ModifiedIndexes.Clear();
        
        OnClose?.Invoke();
    }

    private void OnPopulateDetour(AtkUnitBase* unitBase, ListItemInfo* listItemInfo, AtkResNode** nodeList) {
        try {
            var index = listItemInfo->ListItem->Renderer->OwnerNode->NodeId;

            // If this index has been modified
            if (ModifiedIndexes.Contains(index)) {

                // And it is a valid modification target
                if (ShouldModifyElement(listItemInfo)) {
                    try {
                        Update?.Invoke(new ListPopulatorData<T> {
                            Addon = (T*)unitBase,
                            ItemInfo = listItemInfo,
                            NodeList = nodeList,
                            Index = index,
                        });
                    }
                    catch (Exception e) {
                        Log.Exception(e);
                    }
                }

                // It is no longer a valid modification target
                else {
                    try {
                        Reset?.Invoke(new ListPopulatorData<T> {
                            Addon = (T*)unitBase,
                            ItemInfo = listItemInfo,
                            NodeList = nodeList,
                            Index = index,
                        });
                    }
                    catch (Exception e) {
                        Log.Exception(e);
                    } finally {
                        ModifiedIndexes.Remove(index);
                    }
                }
            }
            
            // This index has not been modified
            else {

                // But should
                if (ShouldModifyElement(listItemInfo)) {
                    try {                     
                        Apply?.Invoke(new ListPopulatorData<T> {
                            Addon = (T*) unitBase,
                            ItemInfo = listItemInfo,
                            NodeList = nodeList,
                            Index = index,
                        });
                    }
                    catch (Exception e) {
                        Log.Exception(e);
                    }
                    
                    ModifiedIndexes.Add(index);
                }

                // But shouldn't, so we should leave it alone
                else {
                    
                }
            }
        }
        catch (Exception e) {
            Log.Exception(e);
        } finally {
            onListPopulate!.Original(unitBase, listItemInfo, nodeList);
        }
    }
    
    public void Dispose() {
        DalamudInterface.Instance.AddonLifecycle.UnregisterListener(OnAddonSetup, OnAddonFinalize);
        onListPopulate?.Dispose();
    }

    public void UntrackElement(uint index)
        => ModifiedIndexes.Remove(index);

    protected abstract delegate*unmanaged<AtkUnitBase*, ListItemInfo*, AtkResNode**, void> GetPopulateDelegate(T* addon);

    public Action<ListPopulatorData<T>>? Apply { get; set; }
    public Action<ListPopulatorData<T>>? Update { get; set; }
    public Action<ListPopulatorData<T>>? Reset { get; set; }
    public Action? OnClose { get; set; }
    public Action? OnOpen { get; set; }
    
    protected abstract bool ShouldModifyElement(ListItemInfo* listItemInfo);
}
