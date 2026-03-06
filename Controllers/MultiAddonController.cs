using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Controllers;

public class MultiAddonController(params string[] addonNames) : MultiAddonController<AtkUnitBase>(addonNames);

/// <summary>
/// For use with addons that have multiple persistent variants, but where only one is used at a time.
/// For example, Inventories or CastBars.
/// Using this with other addons will duplicate their associated events incorrectly.
/// </summary>
public unsafe class MultiAddonController<T> : AddonEventController<T>, IDisposable where T : unmanaged {
    
    private readonly List<AddonController<T>> addonControllers = [];

    public MultiAddonController(params string[] addonNames) {
        foreach (var addonName in addonNames) {
            if (addonName is "NamePlate") {
                Log.Error("Attaching to NamePlate is not supported. Use OverlayController instead.");
                continue;
            }

            // Don't allow duplicate addon controllers
            if (addonControllers.Any(controller => controller.AddonName == addonName)) continue;

            var newController = new AddonController<T>(addonName);

            addonControllers.Add(newController);

            newController.OnAttach += ControllerOnAttach;
            newController.OnDetach += ControllerOnDetach;
            newController.OnRefresh += ControllerOnRefresh;
            newController.OnUpdate += ControllerOnUpdate;
        }
    }

    private void ControllerOnAttach(T* addon) 
        => OnInnerAttach?.Invoke(addon);

    private void ControllerOnDetach(T* addon)
        => OnInnerDetach?.Invoke(addon);

    private void ControllerOnRefresh(T* addon)
        => OnInnerRefresh?.Invoke(addon);

    private void ControllerOnUpdate(T* addon)
        => OnInnerUpdate?.Invoke(addon);

    public void Dispose() {
        DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
            addonControllers.ForEach(controller => controller.Dispose());
            addonControllers.Clear();
        });
    }

    public void Enable() {
        addonControllers.ForEach(controller => controller.Enable());
    }

    public void Disable()
        => addonControllers.ForEach(controller => controller.Disable());
}
