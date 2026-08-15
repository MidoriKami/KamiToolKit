using System;
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

/// <inheritdoc/>
public class AddonController : AddonController<AtkUnitBase>;

/// <summary>
/// Helper class intended to make interacting with native addons much easier.
/// The primary feature is automatic unloading, and reloading when an addon loads/unloads/reloads.
/// </summary>
public class AddonController<T> : IAddonEventController<T>, IAsyncDisposable, IDisposable where T : unmanaged {

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
    public unsafe void Enable() {
        ThreadSafety.AssertMainThread();
        if (IsEnabled) return;

        IAddonLifecycle.Get().RegisterListener(AddonEvent.PostSetup, AddonName, OnAddonEvent);
        IAddonLifecycle.Get().RegisterListener(AddonEvent.PreFinalize, AddonName, OnAddonEvent);

        if (OnRefresh is not null || OnPreRefresh is not null) {
            IAddonLifecycle.Get().RegisterListener(AddonEvent.PreRefresh, AddonName, OnAddonEvent);
            IAddonLifecycle.Get().RegisterListener(AddonEvent.PreRequestedUpdate, AddonName, OnAddonEvent);
            IAddonLifecycle.Get().RegisterListener(AddonEvent.PostRefresh, AddonName, OnAddonEvent);
            IAddonLifecycle.Get().RegisterListener(AddonEvent.PostRequestedUpdate, AddonName, OnAddonEvent);
        }

        if (OnUpdate is not null) {
            IAddonLifecycle.Get().RegisterListener(AddonEvent.PostUpdate, AddonName, OnAddonEvent);
        }

        if (OnPreUpdate is not null) {
            IAddonLifecycle.Get().RegisterListener(AddonEvent.PreUpdate, AddonName, OnAddonEvent);
        }

        if (OnDraw is not null) {
            IAddonLifecycle.Get().RegisterListener(AddonEvent.PreDraw, AddonName, OnAddonEvent);
        }

        if (AddonPointer is not null) {
            OnSetup?.Invoke(AddonPointer);
        }

        IsEnabled = true;
    }

    /// <inheritdoc/>
    public async Task EnableAsync() {
        if (IsEnabled) return;

        IAddonLifecycle.Get().RegisterListener(AddonEvent.PostSetup, AddonName, OnAddonEvent);
        IAddonLifecycle.Get().RegisterListener(AddonEvent.PreFinalize, AddonName, OnAddonEvent);

        if (OnRefresh is not null || OnPreRefresh is not null) {
            IAddonLifecycle.Get().RegisterListener(AddonEvent.PreRefresh, AddonName, OnAddonEvent);
            IAddonLifecycle.Get().RegisterListener(AddonEvent.PreRequestedUpdate, AddonName, OnAddonEvent);
            IAddonLifecycle.Get().RegisterListener(AddonEvent.PostRefresh, AddonName, OnAddonEvent);
            IAddonLifecycle.Get().RegisterListener(AddonEvent.PostRequestedUpdate, AddonName, OnAddonEvent);
        }

        if (OnUpdate is not null) {
            IAddonLifecycle.Get().RegisterListener(AddonEvent.PostUpdate, AddonName, OnAddonEvent);
        }

        if (OnPreUpdate is not null) {
            IAddonLifecycle.Get().RegisterListener(AddonEvent.PreUpdate, AddonName, OnAddonEvent);
        }

        if (OnDraw is not null) {
            IAddonLifecycle.Get().RegisterListener(AddonEvent.PreDraw, AddonName, OnAddonEvent);
        }

        await IFramework.Get().Run(() => {
            unsafe {
                if (AddonPointer is not null) {
                    OnSetup?.Invoke(AddonPointer);
                }
            }
        });

        IsEnabled = true;
    }

    /// <inheritdoc/>
    public unsafe void Disable() {
        ThreadSafety.AssertMainThread();
        if (!IsEnabled) return;
        IsEnabled = false;

        IAddonLifecycle.Get().UnregisterListener(OnAddonEvent);

        if (AddonPointer is not null) {
            OnFinalize?.Invoke(AddonPointer);
        }
    }

    /// <inheritdoc/>
    public async Task DisableAsync() {
        if (!IsEnabled) return;
        IsEnabled = false;

        IAddonLifecycle.Get().UnregisterListener(OnAddonEvent);

        await IFramework.Get().Run(() => {
            unsafe {
                if (AddonPointer is not null) {
                    OnFinalize?.Invoke(AddonPointer);
                }
            }
        });
    }

    /// <inheritdoc/>
    public virtual void Dispose()
        => Disable();

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
        => await DisableAsync();

    private unsafe void OnAddonEvent(AddonEvent type, AddonArgs args) {
        var addon = (T*)args.Addon.Address;

        switch (type) {
            case AddonEvent.PostSetup:
                OnSetup?.Invoke(addon);
                return;

            case AddonEvent.PreFinalize:
                OnFinalize?.Invoke(addon);
                return;

            case AddonEvent.PreRefresh or AddonEvent.PreRequestedUpdate when args.Addon.IsReady:
                OnPreRefresh?.Invoke(addon);
                break;

            case AddonEvent.PostRefresh or AddonEvent.PostRequestedUpdate when args.Addon.IsReady:
                OnRefresh?.Invoke(addon);
                return;

            case AddonEvent.PreUpdate when args.Addon.IsReady:
                OnPreUpdate?.Invoke(addon);
                break;

            case AddonEvent.PostUpdate when args.Addon.IsReady:
                OnUpdate?.Invoke(addon);
                return;

            case AddonEvent.PreDraw when args.Addon.IsReady:
                OnDraw?.Invoke(addon);
                return;
        }
    }

    private unsafe T* AddonPointer => (T*)RaptureAtkUnitManager.Instance()->GetAddonByName(AddonName);

    private bool IsEnabled { get; set; }
}
