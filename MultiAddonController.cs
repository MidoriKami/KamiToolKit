using System;
using System.Collections.Generic;
using System.Linq;

namespace KamiToolKit;

/// <summary>
/// For use with controlling multiple addons at once, for example the various kinds of Target Cast Bars
/// </summary>
public class MultiAddonController : IDisposable {
    
    private readonly List<AddonController> addonControllers = [];

    public MultiAddonController(params string[] addonNames) {
        foreach (var addonName in addonNames) {
            // Don't allow duplicate addon controllers
            if (addonControllers.Any(controller => controller.AddonName == addonName)) continue;

            addonControllers.Add(new AddonController(addonName));
        }
    }

    public void Dispose() {
        foreach (var addonController in addonControllers) {
            addonController.Dispose();
        }

        addonControllers.Clear();
    }
    
    public void Enable() {
        foreach (var addonController in addonControllers) {
            addonController.Enable();
        }
    }

    public void Disable() {
        foreach (var addonController in addonControllers) {
            addonController.Disable();
        }
    }
    
    public virtual event AddonController.AddonControllerEvent? OnAttach {
        add {
            foreach (var addonController in addonControllers) {
                addonController.OnAttach += value;
            }
        }
        remove => throw new Exception("Do not remove events, on dispose addon state will be managed properly.");
    }

    public virtual event AddonController.AddonControllerEvent? OnDetach {
        add {
            foreach (var addonController in addonControllers) {
                addonController.OnAttach += value;
            }
        }
        remove => throw new Exception("Do not remove events, on dispose addon state will be managed properly.");
    }
    
    public virtual event AddonController.AddonControllerEvent? OnRefresh {
        add {
            foreach (var addonController in addonControllers) {
                addonController.OnAttach += value;
            }
        }
        remove => throw new Exception("Do not remove events, on dispose addon state will be managed properly.");
    }
    public virtual event AddonController.AddonControllerEvent? OnUpdate {
        add {
            foreach (var addonController in addonControllers) {
                addonController.OnAttach += value;
            }
        }
        remove => throw new Exception("Do not remove events, on dispose addon state will be managed properly.");
    }

    public virtual event AddonController.AddonControllerEvent? OnPreEnable {
        add {
            foreach (var addonController in addonControllers) {
                addonController.OnAttach += value;
            }
        }
        remove => throw new Exception("Do not remove events, on dispose addon state will be managed properly.");
    }
    
    public virtual event AddonController.AddonControllerEvent? OnPostEnable {
        add {
            foreach (var addonController in addonControllers) {
                addonController.OnAttach += value;
            }
        }
        remove => throw new Exception("Do not remove events, on dispose addon state will be managed properly.");
    }

    public virtual event AddonController.AddonControllerEvent? OnPreDisable {
        add {
            foreach (var addonController in addonControllers) {
                addonController.OnAttach += value;
            }
        }
        remove => throw new Exception("Do not remove events, on dispose addon state will be managed properly.");
    }

    public virtual event AddonController.AddonControllerEvent? OnPostDisable {
        add {
            foreach (var addonController in addonControllers) {
                addonController.OnAttach += value;
            }
        }
        remove => throw new Exception("Do not remove events, on dispose addon state will be managed properly.");
    }

    public void RegisterOnAttach(string addonName, AddonController.AddonControllerEvent callback) {
        var targetController = addonControllers.FirstOrDefault(controller => controller.AddonName == addonName);
        if (targetController == null) return;

        targetController.OnAttach += callback;
    }

    public void RegisterOnDetach(string addonName, AddonController.AddonControllerEvent callback) {
        var targetController = addonControllers.FirstOrDefault(controller => controller.AddonName == addonName);
        if (targetController == null) return;
        
        targetController.OnDetach += callback;
    }

    public void RegisterOnRefresh(string addonName, AddonController.AddonControllerEvent callback) {
        var targetController = addonControllers.FirstOrDefault(controller => controller.AddonName == addonName);
        if (targetController == null) return;

        targetController.OnRefresh += callback;
    }

    public void RegisterOnUpdate(string addonName, AddonController.AddonControllerEvent callback) {
        var targetController = addonControllers.FirstOrDefault(controller => controller.AddonName == addonName);
        if (targetController == null) return;

        targetController.OnUpdate += callback;
    }

    public void RegisterOnPreEnable(string addonName, AddonController.AddonControllerEvent callback) {
        var targetController = addonControllers.FirstOrDefault(controller => controller.AddonName == addonName);
        if (targetController == null) return;

        targetController.OnPreEnable += callback;
    }

    public void RegisterOnPostEnable(string addonName, AddonController.AddonControllerEvent callback) {
        var targetController = addonControllers.FirstOrDefault(controller => controller.AddonName == addonName);
        if (targetController == null) return;

        targetController.OnPostEnable += callback;
    }

    public void RegisterOnPreDisable(string addonName, AddonController.AddonControllerEvent callback) {
        var targetController = addonControllers.FirstOrDefault(controller => controller.AddonName == addonName);
        if (targetController == null) return;

        targetController.OnPreDisable += callback;
    }

    public void RegisterOnPostDisable(string addonName, AddonController.AddonControllerEvent callback) {
        var targetController = addonControllers.FirstOrDefault(controller => controller.AddonName == addonName);
        if (targetController == null) return;

        targetController.OnPostDisable += callback;
    }
}
