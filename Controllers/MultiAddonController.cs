using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Dalamud;

namespace KamiToolKit.Controllers;

public class MultiAddonController : MultiAddonController<AtkUnitBase>;

/// <summary>
/// For use with addons that have multiple persistent variants, but where only one is used at a time.
/// For example, Inventories or CastBars.
/// Using this with other addons will duplicate their associated events incorrectly.
/// </summary>
public unsafe class MultiAddonController<T> : IAddonEventController<T>, IDisposable where T : unmanaged {
    
    private readonly List<AddonController<T>> addonControllers = [];

    public required List<string> AddonNames {
        init {
            foreach (var addonName in value) {
                if (addonName is "NamePlate") {
                    Services.Log.Error("Attaching to NamePlate is not supported. Use OverlayController instead.");
                    continue;
                }

                // Don't allow duplicate addon controllers
                if (addonControllers.Any(controller => controller.AddonName == addonName)) continue;

                var newController = new AddonController<T> {
                    AddonName = addonName,
                    OnSetup = ControllerOnAttach,
                    OnFinalize = ControllerOnDetach,
                    OnRefresh = ControllerOnRefresh,
                    OnUpdate = ControllerOnUpdate,
                    OnPreRefresh = ControllerOnPreRefresh,
                    OnPreUpdate = ControllerOnPreUpdate,
                };

                addonControllers.Add(newController);
            }
        }
    }
    
    public void Enable()
        => addonControllers.ForEach(controller => controller.Enable());

    public void Disable()
        => addonControllers.ForEach(controller => controller.Disable());

    private void ControllerOnAttach(T* addon) 
        => OnSetup?.Invoke(addon);

    private void ControllerOnDetach(T* addon)
        => OnFinalize?.Invoke(addon);

    private void ControllerOnRefresh(T* addon)
        => OnRefresh?.Invoke(addon);

    private void ControllerOnUpdate(T* addon)
        => OnUpdate?.Invoke(addon);
    
    private void ControllerOnPreRefresh(T* addon)
        => OnPreRefresh?.Invoke(addon);
    
    private void ControllerOnPreUpdate(T* addon)
        => OnPreUpdate?.Invoke(addon);

    public void Dispose() => Services.Framework.RunOnFrameworkThread(() => {
        addonControllers.ForEach(controller => controller.Dispose());
        addonControllers.Clear();
    });

    public IAddonEventController<T>.AddonControllerEvent? OnSetup { get; init; }
    public IAddonEventController<T>.AddonControllerEvent? OnFinalize { get; init; }
    public IAddonEventController<T>.AddonControllerEvent? OnPreRefresh { get; init; }
    public IAddonEventController<T>.AddonControllerEvent? OnRefresh { get; init; }
    public IAddonEventController<T>.AddonControllerEvent? OnUpdate { get; init; }
    public IAddonEventController<T>.AddonControllerEvent? OnPreUpdate { get; init; }
}
