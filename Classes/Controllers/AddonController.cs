using System;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Classes.Controllers;

public class AddonController(string addonName) : AddonController<AtkUnitBase>(addonName);

/// <summary>
///     This class provides functionality to add-and manage custom elements for any Addon
/// </summary>
public unsafe class AddonController<T>(string addonName) : IDisposable where T : unmanaged {

    public delegate void AddonControllerEvent(T* addon);

    internal readonly string AddonName = addonName;

    private AtkUnitBase* AddonPointer => (AtkUnitBase*)DalamudInterface.Instance.GameGui.GetAddonByName(AddonName).Address;
    private bool IsEnabled { get; set; }

    public virtual void Dispose() => Disable();

    public void Enable() {
        DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
            if (IsEnabled) return;

            OnInnerPreEnable?.Invoke((T*)AddonPointer);

            DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, AddonName, OnAddonEvent);
            DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, AddonName, OnAddonEvent);
            DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PostRefresh, AddonName, OnAddonEvent);
            DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, AddonName, OnAddonEvent);
            DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, AddonName, OnAddonEvent);

            if (AddonPointer is not null) {
                OnInnerAttach?.Invoke((T*)AddonPointer);
            }

            IsEnabled = true;

            OnInnerPostEnable?.Invoke((T*)AddonPointer);
        });
    }

    private void OnAddonEvent(AddonEvent type, AddonArgs args) {
        var addon = (T*)args.Addon.Address;

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

    public void Disable() {
        DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
            if (!IsEnabled) return;

            OnInnerPreDisable?.Invoke((T*)AddonPointer);

            DalamudInterface.Instance.AddonLifecycle.UnregisterListener(OnAddonEvent);

            if (AddonPointer is not null) {
                OnInnerDetach?.Invoke((T*)AddonPointer);
            }

            IsEnabled = false;

            OnInnerPostDisable?.Invoke((T*)AddonPointer);
        });
    }

    private event AddonControllerEvent? OnInnerAttach;
    private event AddonControllerEvent? OnInnerDetach;
    private event AddonControllerEvent? OnInnerRefresh;
    private event AddonControllerEvent? OnInnerUpdate;

    private event AddonControllerEvent? OnInnerPreEnable;
    private event AddonControllerEvent? OnInnerPostEnable;
    private event AddonControllerEvent? OnInnerPreDisable;
    private event AddonControllerEvent? OnInnerPostDisable;
    
    public virtual event AddonControllerEvent? OnAttach {
        add => OnInnerAttach += value;
        remove => throw new Exception("Do not remove events, on dispose addon state will be managed properly.");
    }

    public virtual event AddonControllerEvent? OnDetach {
        add => OnInnerDetach += value;
        remove => throw new Exception("Do not remove events, on dispose addon state will be managed properly.");
    }
    
    public virtual event AddonControllerEvent? OnRefresh {
        add => OnInnerRefresh += value;
        remove => throw new Exception("Do not remove events, on dispose addon state will be managed properly.");
    }
    public virtual event AddonControllerEvent? OnUpdate {
        add => OnInnerUpdate += value;
        remove => throw new Exception("Do not remove events, on dispose addon state will be managed properly.");
    }

    public virtual event AddonControllerEvent? OnPreEnable {
        add => OnInnerPreEnable += value;
        remove => throw new Exception("Do not remove events, on dispose addon state will be managed properly.");
    }
    
    public virtual event AddonControllerEvent? OnPostEnable {
        add => OnInnerPostEnable += value;
        remove => throw new Exception("Do not remove events, on dispose addon state will be managed properly.");
    }

    public virtual event AddonControllerEvent? OnPreDisable {
        add => OnInnerPreDisable += value;
        remove => throw new Exception("Do not remove events, on dispose addon state will be managed properly.");
    }

    public virtual event AddonControllerEvent? OnPostDisable {
        add => OnInnerPostDisable += value;
        remove => throw new Exception("Do not remove events, on dispose addon state will be managed properly.");
    }
}
