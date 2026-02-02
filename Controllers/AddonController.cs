using System;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Controllers;

public class AddonController(string addonName) : AddonController<AtkUnitBase>(addonName);

/// <summary>
///     This class provides functionality to add-and manage custom elements for any Addon
/// </summary>
public unsafe class AddonController<T> : AddonEventController<T>, IDisposable where T : unmanaged {

    internal readonly string AddonName;

    private AtkUnitBase* AddonPointer => (AtkUnitBase*)DalamudInterface.Instance.GameGui.GetAddonByName(AddonName).Address;
    private bool IsEnabled { get; set; }

    private bool isSetupComplete;

    /// <summary>
    ///     This class provides functionality to add-and manage custom elements for any Addon
    /// </summary>
    public AddonController(string addonName) {
        if (addonName is "NamePlate") {
            throw new Exception("Attaching to NamePlate is not supported. Use OverlayController instead.");
        }
        
        AddonName = addonName;
    }

    public virtual void Dispose() => Disable();

    public void Enable() {
        DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
            if (IsEnabled) return;

            onInnerPreEnable?.Invoke((T*)AddonPointer);

            DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, AddonName, OnAddonEvent);
            DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, AddonName, OnAddonEvent);
            DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PostRefresh, AddonName, OnAddonEvent);
            DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, AddonName, OnAddonEvent);
            DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, AddonName, OnAddonEvent);

            if (AddonPointer is not null) {
                OnInnerAttach?.Invoke((T*)AddonPointer);
                isSetupComplete = true;
            }

            IsEnabled = true;

            onInnerPostEnable?.Invoke((T*)AddonPointer);
        });
    }

    private void OnAddonEvent(AddonEvent type, AddonArgs args) {
        var addon = (T*)args.Addon.Address;

        switch (type) {
            case AddonEvent.PostSetup:
                OnInnerAttach?.Invoke(addon);
                isSetupComplete = true;
                return;

            case AddonEvent.PreFinalize:
                OnInnerDetach?.Invoke(addon);
                isSetupComplete = false;
                return;

            case AddonEvent.PostRefresh or AddonEvent.PostRequestedUpdate when isSetupComplete:
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

            onInnerPreDisable?.Invoke((T*)AddonPointer);

            DalamudInterface.Instance.AddonLifecycle.UnregisterListener(OnAddonEvent);

            if (AddonPointer is not null) {
                OnInnerDetach?.Invoke((T*)AddonPointer);
            }

            IsEnabled = false;

            onInnerPostDisable?.Invoke((T*)AddonPointer);
        });
    }
    
    public event AddonControllerEvent? OnPreEnable {
        add => onInnerPreEnable += value;
        remove => throw new Exception("Do not remove events, on dispose addon state will be managed properly.");
    }

    public event AddonControllerEvent? OnPostEnable {
        add => onInnerPostEnable += value;
        remove => throw new Exception("Do not remove events, on dispose addon state will be managed properly.");
    }

    public event AddonControllerEvent? OnPreDisable {
        add => onInnerPreDisable += value;
        remove => throw new Exception("Do not remove events, on dispose addon state will be managed properly.");
    }

    public event AddonControllerEvent? OnPostDisable {
        add => onInnerPostDisable += value;
        remove => throw new Exception("Do not remove events, on dispose addon state will be managed properly.");
    }

    private AddonControllerEvent? onInnerPreEnable;
    private AddonControllerEvent? onInnerPostEnable;
    private AddonControllerEvent? onInnerPreDisable;
    private AddonControllerEvent? onInnerPostDisable;
}
