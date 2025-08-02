using System;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Extensions;

namespace KamiToolKit;

/// <summary>
///     This class provides functionality to add-and manage custom elements for any Addon
/// </summary>
public unsafe class AddonController<T> : IDisposable where T : unmanaged {

    public delegate void AddonControllerEvent(T* addon);

    private readonly string addonName;

    public AddonController(string addonName) {
        this.addonName = addonName;
    }

    public AddonController(IDalamudPluginInterface pluginInterface) {
        pluginInterface.Inject(this);

        addonName = AtkUnitBaseExtensions.GetAddonTypeName<T>();
    }

    private AtkUnitBase* AddonPointer => (AtkUnitBase*)DalamudInterface.Instance.GameGui.GetAddonByName(addonName);
    private bool IsEnabled { get; set; }

    public virtual void Dispose() => Disable();

    public void Enable() {
        DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
            if (IsEnabled) return;

            PreEnable?.Invoke((T*)AddonPointer);

            DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, addonName, OnAddonEvent);
            DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, addonName, OnAddonEvent);
            DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PostRefresh, addonName, OnAddonEvent);
            DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, addonName, OnAddonEvent);
            DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, addonName, OnAddonEvent);

            if (AddonPointer is not null) {
                OnAttach?.Invoke((T*)AddonPointer);
            }

            IsEnabled = true;

            PostEnable?.Invoke((T*)AddonPointer);
        });
    }

    private void OnAddonEvent(AddonEvent type, AddonArgs args) {
        var addon = (T*)args.Addon;

        switch (type) {
            case AddonEvent.PostSetup:
                OnAttach?.Invoke(addon);
                return;

            case AddonEvent.PreFinalize:
                OnDetach?.Invoke(addon);
                return;

            case AddonEvent.PostRefresh or AddonEvent.PostRequestedUpdate:
                OnRefresh?.Invoke(addon);
                return;

            case AddonEvent.PostUpdate:
                OnUpdate?.Invoke(addon);
                return;
        }
    }

    public void Disable() {
        DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
            if (!IsEnabled) return;

            PreDisable?.Invoke((T*)AddonPointer);

            DalamudInterface.Instance.AddonLifecycle.UnregisterListener(OnAddonEvent);

            if (AddonPointer is not null) {
                OnDetach?.Invoke((T*)AddonPointer);
            }

            IsEnabled = false;

            PostDisable?.Invoke((T*)AddonPointer);
        });
    }

    public event AddonControllerEvent? OnAttach;
    public event AddonControllerEvent? OnDetach;
    public event AddonControllerEvent? OnRefresh;
    public event AddonControllerEvent? OnUpdate;

    public event AddonControllerEvent? PreEnable;
    public event AddonControllerEvent? PostEnable;
    public event AddonControllerEvent? PreDisable;
    public event AddonControllerEvent? PostDisable;
}
