using System;
using System.Collections.Generic;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Dalamud;

namespace KamiToolKit.Controllers;

/// <summary>
/// Addon controller for dynamically managing addons, typical use case is intended to
/// be for a single tasks, that can apply to one or many addons at once.
/// </summary>
public unsafe class DynamicAddonController : AddonEventController<AtkUnitBase>, IDisposable {

    private readonly HashSet<string> trackedAddons = [];
    private bool isEnabled;
    
    public DynamicAddonController(params string[] addonNames) {
        foreach (var addonName in addonNames) {
            AddAddon(addonName);
        }
    }

    public void AddAddon(string name) {
        if (name is "NamePlate") {
            Services.Log.Error("Attaching to NamePlate is not supported. Use OverlayController instead.");
            return;
        }
        
        trackedAddons.Add(name);

        if (isEnabled) {
            AddListeners(name);
        }
    }

    public void RemoveAddon(string name) {
        trackedAddons.Remove(name);

        if (isEnabled) {
            RemoveListeners(name);
        }
    }

    private void OnAddonEvent(AddonEvent type, AddonArgs args) {
        var addon = (AtkUnitBase*)args.Addon.Address;

        switch (type) {
            case AddonEvent.PostSetup:
                OnInnerAttach?.Invoke(addon);
                return;

            case AddonEvent.PreFinalize:
                OnInnerDetach?.Invoke(addon);
                return;

            case AddonEvent.PostRefresh or AddonEvent.PostRequestedUpdate:
                OnInnerRefresh?.Invoke(addon);
                return;

            case AddonEvent.PostUpdate:
                OnInnerUpdate?.Invoke(addon);
                return;
        }
    }

    public void Enable() {
        foreach (var name in trackedAddons) {
            AddListeners(name);
        }

        isEnabled = true;
    }

    public void Disable() {
        isEnabled = false;
        
        foreach (var name in trackedAddons) {
            RemoveListeners(name);
        }
    }

    private void AddListeners(string name) {
        Services.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, name, OnAddonEvent);
        Services.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, name, OnAddonEvent);
        Services.AddonLifecycle.RegisterListener(AddonEvent.PostRefresh, name, OnAddonEvent);
        Services.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, name, OnAddonEvent);
        Services.AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, name, OnAddonEvent);

        Services.Framework.RunOnFrameworkThread(() => {
            var addon = RaptureAtkUnitManager.Instance()->GetAddonByName(name);
            if (addon is not null) {
                OnInnerAttach?.Invoke(addon);
            }
        });
    }

    private void RemoveListeners(string name) {
        Services.AddonLifecycle.UnregisterListener(AddonEvent.PostSetup, name, OnAddonEvent);
        Services.AddonLifecycle.UnregisterListener(AddonEvent.PreFinalize, name, OnAddonEvent);
        Services.AddonLifecycle.UnregisterListener(AddonEvent.PostRefresh, name, OnAddonEvent);
        Services.AddonLifecycle.UnregisterListener(AddonEvent.PostRequestedUpdate, name, OnAddonEvent);
        Services.AddonLifecycle.UnregisterListener(AddonEvent.PostUpdate, name, OnAddonEvent);

        Services.Framework.RunOnFrameworkThread(() => {
            var addon = RaptureAtkUnitManager.Instance()->GetAddonByName(name);
            if (addon is not null) {
                OnInnerDetach?.Invoke(addon);
            }
        });
    }

    public void Dispose() {
        Services.AddonLifecycle.UnregisterListener(OnAddonEvent);
        Disable();
    }
}
