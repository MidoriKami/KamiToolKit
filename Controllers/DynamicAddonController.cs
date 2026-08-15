using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Interfaces;
using KamiToolKit.Internal.Classes;
using KamiToolKit.UiOverlay;

namespace KamiToolKit.Controllers;

/// <summary>
/// Addon controller for dynamically managing addons, typical use case is intended to
/// be for a single tasks, that can apply to one or many addons at once.
/// </summary>
public class DynamicAddonController : IAddonEventController<AtkUnitBase>, IDisposable, IAsyncDisposable {

    private readonly HashSet<string> trackedAddons = [];
    private bool isEnabled;

    /// <summary>
    /// Addon names to bind to.
    /// </summary>
    /// <exception cref="Exception">Throws when attempting to attach to NamePlate, use <see cref="OverlayController"/> instead.</exception>
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

    /// <inheritdoc/>>
    public IAddonEventController<AtkUnitBase>.AddonControllerEvent? OnSetup { get; init; }

    /// <inheritdoc/>>
    public IAddonEventController<AtkUnitBase>.AddonControllerEvent? OnFinalize { get; init; }

    /// <inheritdoc/>>
    public IAddonEventController<AtkUnitBase>.AddonControllerEvent? OnPreRefresh { get; init; }

    /// <inheritdoc/>>
    public IAddonEventController<AtkUnitBase>.AddonControllerEvent? OnRefresh { get; init; }

    /// <inheritdoc/>>
    public IAddonEventController<AtkUnitBase>.AddonControllerEvent? OnUpdate { get; init; }

    /// <inheritdoc/>>
    public IAddonEventController<AtkUnitBase>.AddonControllerEvent? OnDraw { get; init; }

    /// <inheritdoc/>>
    public IAddonEventController<AtkUnitBase>.AddonControllerEvent? OnPreUpdate { get; init; }

    /// <inheritdoc/>>
    public void Enable() {
        foreach (var name in trackedAddons) {
            AddListeners(name);
        }

        isEnabled = true;
    }

    /// <inheritdoc/>>
    public async Task EnableAsync() {
        await Task.WhenAll(trackedAddons.Select(AddListenersAsync));

        isEnabled = true;
    }

    /// <inheritdoc/>>
    public void Disable() {
        isEnabled = false;

        foreach (var name in trackedAddons) {
            RemoveListeners(name);
        }
    }

    /// <inheritdoc/>>
    public async Task DisableAsync() {
        isEnabled = false;

        await Task.WhenAll(trackedAddons.Select(RemoveListenersAsync));
    }

    /// <summary>
    /// Adds an addon to the tracked addons list.
    /// </summary>
    public void AddAddon(string name) {
        if (name is "NamePlate") {
            IPluginLog.Get().Error("Attaching to NamePlate is not supported. Use OverlayController instead.");
            return;
        }

        trackedAddons.Add(name);

        if (isEnabled) {
            AddListeners(name);
        }
    }

    /// <summary>
    /// Removes an addon from the tracked addons list.
    /// </summary>
    /// <param name="name"></param>
    public void RemoveAddon(string name) {
        trackedAddons.Remove(name);

        if (isEnabled) {
            RemoveListeners(name);
        }
    }

    /// <inheritdoc />
    public void Dispose() {
        IAddonLifecycle.Get().UnregisterListener(OnAddonEvent);
        Disable();
    }

    /// <inheritdoc/>>
    public async ValueTask DisposeAsync() {
        IAddonLifecycle.Get().UnregisterListener(OnAddonEvent);

        await DisableAsync();
    }

    private unsafe void OnAddonEvent(AddonEvent type, AddonArgs args) {
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

            case AddonEvent.PreDraw:
                OnDraw?.Invoke(addon);
                return;
        }
    }

    private unsafe void AddListeners(string name) {
        ThreadSafety.AssertMainThread();

        RegisterListeners(name);

        var addon = RaptureAtkUnitManager.Instance()->GetAddonByName(name);
        if (addon is not null) {
            OnSetup?.Invoke(addon);
        }
    }

    private async Task AddListenersAsync(string name) {
        RegisterListeners(name);

        await IFramework.Get().Run(() => {
            unsafe {
                var addon = RaptureAtkUnitManager.Instance()->GetAddonByName(name);
                if (addon is not null) {
                    OnSetup?.Invoke(addon);
                }
            }
        });
    }

    private unsafe void RemoveListeners(string name) {
        ThreadSafety.AssertMainThread();

        UnregisterListeners(name);

        var addon = RaptureAtkUnitManager.Instance()->GetAddonByName(name);
        if (addon is not null) {
            OnFinalize?.Invoke(addon);
        }
    }

    private async Task RemoveListenersAsync(string name) {
        UnregisterListeners(name);

        await IFramework.Get().Run(() => {
            unsafe {
                var addon = RaptureAtkUnitManager.Instance()->GetAddonByName(name);
                if (addon is not null) {
                    OnFinalize?.Invoke(addon);
                }
            }
        });
    }

    private void RegisterListeners(string name) {
        IAddonLifecycle.Get().RegisterListener(AddonEvent.PostSetup, name, OnAddonEvent);
        IAddonLifecycle.Get().RegisterListener(AddonEvent.PreFinalize, name, OnAddonEvent);
        IAddonLifecycle.Get().RegisterListener(AddonEvent.PreRefresh, name, OnAddonEvent);
        IAddonLifecycle.Get().RegisterListener(AddonEvent.PreRequestedUpdate, name, OnAddonEvent);
        IAddonLifecycle.Get().RegisterListener(AddonEvent.PreUpdate, name, OnAddonEvent);
        IAddonLifecycle.Get().RegisterListener(AddonEvent.PostRefresh, name, OnAddonEvent);
        IAddonLifecycle.Get().RegisterListener(AddonEvent.PostRequestedUpdate, name, OnAddonEvent);
        IAddonLifecycle.Get().RegisterListener(AddonEvent.PostUpdate, name, OnAddonEvent);
        IAddonLifecycle.Get().RegisterListener(AddonEvent.PreDraw, name, OnAddonEvent);
    }

    private void UnregisterListeners(string name) {
        IAddonLifecycle.Get().UnregisterListener(AddonEvent.PostSetup, name, OnAddonEvent);
        IAddonLifecycle.Get().UnregisterListener(AddonEvent.PreFinalize, name, OnAddonEvent);
        IAddonLifecycle.Get().UnregisterListener(AddonEvent.PreRefresh, name, OnAddonEvent);
        IAddonLifecycle.Get().UnregisterListener(AddonEvent.PreRequestedUpdate, name, OnAddonEvent);
        IAddonLifecycle.Get().UnregisterListener(AddonEvent.PreUpdate, name, OnAddonEvent);
        IAddonLifecycle.Get().UnregisterListener(AddonEvent.PostRefresh, name, OnAddonEvent);
        IAddonLifecycle.Get().UnregisterListener(AddonEvent.PostRequestedUpdate, name, OnAddonEvent);
        IAddonLifecycle.Get().UnregisterListener(AddonEvent.PostUpdate, name, OnAddonEvent);
        IAddonLifecycle.Get().UnregisterListener(AddonEvent.PreDraw, name, OnAddonEvent);
    }
}
