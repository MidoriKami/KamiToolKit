using System;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Interfaces;
using KamiToolKit.Internal.Classes;
using KamiToolKit.UiOverlay;

namespace KamiToolKit.Controllers;

/// <inheritdoc/>
public class AddonController : AddonController<AtkUnitBase>;

/// <summary>
/// Helper class intended to make interacting with native addons much easier.
/// The primary feature is automatic unloading, and reloading when an addon loads/unloads/reloads.
/// </summary>
public unsafe class AddonController<T> : IAddonEventController<T>, IDisposable where T : unmanaged {

    /// <summary>
    /// The addon name to bind to.
    /// </summary>
    /// <exception cref="Exception">Exception when attempting to attach to NamePlate addon, use <see cref="OverlayController"/> instead.</exception>
    public required string AddonName {
        get;
        init {
            if (value is "NamePlate") {
                throw new Exception("Attaching to NamePlate is not supported. Use OverlayController Instead");
            }
            field = value;
        }
    }

    /// <inheritdoc/>
    public IAddonEventController<T>.AddonControllerEvent? OnSetup { get; init; }

    /// <inheritdoc/>
    public IAddonEventController<T>.AddonControllerEvent? OnFinalize { get; init; }

    /// <inheritdoc/>
    public IAddonEventController<T>.AddonControllerEvent? OnPreRefresh { get; init; }

    /// <inheritdoc/>
    public IAddonEventController<T>.AddonControllerEvent? OnRefresh { get; init; }

    /// <inheritdoc/>
    public IAddonEventController<T>.AddonControllerEvent? OnUpdate { get; init; }

    /// <inheritdoc/>
    public IAddonEventController<T>.AddonControllerEvent? OnPreUpdate { get; init; }

    /// <inheritdoc />
    public IAddonEventController<T>.AddonControllerEvent? OnDraw { get; init; }

    /// <inheritdoc/>
    public void Enable() {
        ThreadSafety.AssertMainThread();
        if (IsEnabled) return;

        Services.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, AddonName, OnAddonEvent);
        Services.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, AddonName, OnAddonEvent);

        if (OnRefresh is not null || OnPreRefresh is not null) {
            Services.AddonLifecycle.RegisterListener(AddonEvent.PreRefresh, AddonName, OnAddonEvent);
            Services.AddonLifecycle.RegisterListener(AddonEvent.PreRequestedUpdate, AddonName, OnAddonEvent);
            Services.AddonLifecycle.RegisterListener(AddonEvent.PostRefresh, AddonName, OnAddonEvent);
            Services.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, AddonName, OnAddonEvent);
        }

        if (OnUpdate is not null) {
            Services.AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, AddonName, OnAddonEvent);
        }

        if (OnPreUpdate is not null) {
            Services.AddonLifecycle.RegisterListener(AddonEvent.PreUpdate, AddonName, OnAddonEvent);
        }

        if (OnDraw is not null) {
            Services.AddonLifecycle.RegisterListener(AddonEvent.PreDraw, AddonName, OnAddonEvent);
        }

        if (AddonPointer is not null) {
            OnSetup?.Invoke((T*)AddonPointer);
            isSetupComplete = true;
        }

        IsEnabled = true;
    }

    /// <inheritdoc/>
    public void Disable() {
        ThreadSafety.AssertMainThread();
        if (!IsEnabled) return;

        Services.AddonLifecycle.UnregisterListener(OnAddonEvent);

        if (AddonPointer is not null) {
            OnFinalize?.Invoke((T*)AddonPointer);
        }

        IsEnabled = false;
    }

    /// <inheritdoc/>
    public virtual void Dispose()
        => Disable();

    private void OnAddonEvent(AddonEvent type, AddonArgs args) {
        var addon = (T*)args.Addon.Address;

        switch (type) {
            case AddonEvent.PostSetup:
                OnSetup?.Invoke(addon);
                isSetupComplete = true;
                return;

            case AddonEvent.PreFinalize:
                OnFinalize?.Invoke(addon);
                isSetupComplete = false;
                return;

            case AddonEvent.PreRefresh or AddonEvent.PreRequestedUpdate when isSetupComplete:
                OnPreRefresh?.Invoke(addon);
                break;

            case AddonEvent.PostRefresh or AddonEvent.PostRequestedUpdate when isSetupComplete:
                OnRefresh?.Invoke(addon);
                return;

            case AddonEvent.PreUpdate when isSetupComplete:
                OnPreUpdate?.Invoke(addon);
                break;

            case AddonEvent.PostUpdate when isSetupComplete:
                OnUpdate?.Invoke(addon);
                return;

            case AddonEvent.PreDraw when isSetupComplete:
                OnDraw?.Invoke(addon);
                return;
        }
    }

    private AtkUnitBase* AddonPointer => Services.GameGui.GetAddonByName<AtkUnitBase>(AddonName);
    private bool IsEnabled { get; set; }
    private bool isSetupComplete;
}
