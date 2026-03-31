using System;
using System.Collections.Generic;
using System.Linq;
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
public unsafe class DynamicAddonController : IAddonEventController<AtkUnitBase>, IDisposable {

    private readonly HashSet<string> trackedAddons = [];
    private bool isEnabled;

    public required List<string> AddonNames {
        init {
            if (value.Any(name => name is "NamePlate")) {
                throw new Exception("Attaching to NamePlate is not supported. Use OverlayController Instead");
            }

            foreach (var addonName in value) {
                AddAddon(addonName);
            }
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
                OnSetup?.Invoke(addon);
                return;

            case AddonEvent.PreFinalize:
                OnFinalize?.Invoke(addon);
                return;

            case AddonEvent.PreRefresh or AddonEvent.PreRequestedUpdate:
                OnPreRefresh?.Invoke(addon);
                return;
            
            case AddonEvent.PostRefresh or AddonEvent.PostRequestedUpdate:
                OnRefresh?.Invoke(addon);
                return;

            case AddonEvent.PreUpdate:
                OnPreUpdate?.Invoke(addon);
                return;
            
            case AddonEvent.PostUpdate:
                OnUpdate?.Invoke(addon);
                return;
        }
    }

    private void AddListeners(string name) {
        Services.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, name, OnAddonEvent);
        Services.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, name, OnAddonEvent);
        Services.AddonLifecycle.RegisterListener(AddonEvent.PreRefresh, name, OnAddonEvent);
        Services.AddonLifecycle.RegisterListener(AddonEvent.PreRequestedUpdate, name, OnAddonEvent);
        Services.AddonLifecycle.RegisterListener(AddonEvent.PreUpdate, name, OnAddonEvent);
        Services.AddonLifecycle.RegisterListener(AddonEvent.PostRefresh, name, OnAddonEvent);
        Services.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, name, OnAddonEvent);
        Services.AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, name, OnAddonEvent);

        Services.Framework.RunOnFrameworkThread(() => {
            var addon = RaptureAtkUnitManager.Instance()->GetAddonByName(name);
            if (addon is not null) {
                OnSetup?.Invoke(addon);
            }
        });
    }

    private void RemoveListeners(string name) {
        Services.AddonLifecycle.UnregisterListener(AddonEvent.PostSetup, name, OnAddonEvent);
        Services.AddonLifecycle.UnregisterListener(AddonEvent.PreFinalize, name, OnAddonEvent);
        Services.AddonLifecycle.UnregisterListener(AddonEvent.PreRefresh, name, OnAddonEvent);
        Services.AddonLifecycle.UnregisterListener(AddonEvent.PreRequestedUpdate, name, OnAddonEvent);
        Services.AddonLifecycle.UnregisterListener(AddonEvent.PreUpdate, name, OnAddonEvent);
        Services.AddonLifecycle.UnregisterListener(AddonEvent.PostRefresh, name, OnAddonEvent);
        Services.AddonLifecycle.UnregisterListener(AddonEvent.PostRequestedUpdate, name, OnAddonEvent);
        Services.AddonLifecycle.UnregisterListener(AddonEvent.PostUpdate, name, OnAddonEvent);

        Services.Framework.RunOnFrameworkThread(() => {
            var addon = RaptureAtkUnitManager.Instance()->GetAddonByName(name);
            if (addon is not null) {
                OnFinalize?.Invoke(addon);
            }
        });
    }

    public void Dispose() {
        Services.AddonLifecycle.UnregisterListener(OnAddonEvent);
        Disable();
    }

    public IAddonEventController<AtkUnitBase>.AddonControllerEvent? OnSetup { get; init; }
    public IAddonEventController<AtkUnitBase>.AddonControllerEvent? OnFinalize { get; init; }
    public IAddonEventController<AtkUnitBase>.AddonControllerEvent? OnPreRefresh { get; init; }
    public IAddonEventController<AtkUnitBase>.AddonControllerEvent? OnRefresh { get; init; }
    public IAddonEventController<AtkUnitBase>.AddonControllerEvent? OnUpdate { get; init; }
    public IAddonEventController<AtkUnitBase>.AddonControllerEvent? OnPreUpdate { get; init; }
}
