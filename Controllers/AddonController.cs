using System;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Dalamud;

namespace KamiToolKit.Controllers;

public class AddonController : AddonController<AtkUnitBase>;

/// <summary>
///     This class provides functionality to add-and manage custom elements for any Addon
/// </summary>
public unsafe class AddonController<T> : IAddonEventController<T>, IDisposable where T : unmanaged {
    private AtkUnitBase* AddonPointer => Services.GameGui.GetAddonByName<AtkUnitBase>(AddonName);
    private bool IsEnabled { get; set; }
    private bool isSetupComplete;

    public required string AddonName {
        get;
        init {
            if (value is "NamePlate") {
                throw new Exception("Attaching to NamePlate is not supported. Use OverlayController Instead");
            }
            field = value;
        }
    }

    public void Enable() {
        Services.Framework.RunOnFrameworkThread(() => {
            if (IsEnabled) return;

            Services.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, AddonName, OnAddonEvent);
            Services.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, AddonName, OnAddonEvent);
            Services.AddonLifecycle.RegisterListener(AddonEvent.PreRefresh, AddonName, OnAddonEvent);
            Services.AddonLifecycle.RegisterListener(AddonEvent.PreRequestedUpdate, AddonName, OnAddonEvent);
            Services.AddonLifecycle.RegisterListener(AddonEvent.PostRefresh, AddonName, OnAddonEvent);
            Services.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, AddonName, OnAddonEvent);
            Services.AddonLifecycle.RegisterListener(AddonEvent.PreUpdate, AddonName, OnAddonEvent);
            Services.AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, AddonName, OnAddonEvent);

            if (AddonPointer is not null) {
                OnSetup?.Invoke((T*)AddonPointer);
                isSetupComplete = true;
            }

            IsEnabled = true;
        });
    }

    public virtual void Dispose()
        => Disable();

    public void Disable() {
        Services.Framework.RunOnFrameworkThread(() => {
            if (!IsEnabled) return;

            Services.AddonLifecycle.UnregisterListener(OnAddonEvent);

            if (AddonPointer is not null) {
                OnFinalize?.Invoke((T*)AddonPointer);
            }

            IsEnabled = false;
        });
    }

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

            case AddonEvent.PreUpdate:
                OnPreUpdate?.Invoke(addon);
                break;
            
            case AddonEvent.PostUpdate:
                OnUpdate?.Invoke(addon);
                return;
        }
    }

    public IAddonEventController<T>.AddonControllerEvent? OnSetup { get; init; }
    public IAddonEventController<T>.AddonControllerEvent? OnFinalize { get; init; }
    public IAddonEventController<T>.AddonControllerEvent? OnPreRefresh { get; init; }
    public IAddonEventController<T>.AddonControllerEvent? OnRefresh { get; init; }
    public IAddonEventController<T>.AddonControllerEvent? OnUpdate { get; init; }
    public IAddonEventController<T>.AddonControllerEvent? OnPreUpdate { get; init; }
}
