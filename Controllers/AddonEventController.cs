using System;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace KamiToolKit.Controllers;

public abstract unsafe class AddonEventController<T> where T : unmanaged {

    protected AddonEventController() {
        if (typeof(T) == typeof(AddonNamePlate)) {
            throw new NotSupportedException("Attaching to NamePlate is not supported. Use OverlayController.");
        }
    }
    
    public delegate void AddonControllerEvent(T* addon);
    
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
    
    /// <summary>
    /// You probably don't want this, use <see cref="OnUpdate"/>
    /// </summary>
    public virtual event AddonControllerEvent? OnPreUpdate {
        add => OnInnerPreUpdate += value;
        remove => throw new Exception("Do not remove events, on dispose addon state will be managed properly.");
    }

    protected AddonControllerEvent? OnInnerAttach;
    protected AddonControllerEvent? OnInnerDetach;
    protected AddonControllerEvent? OnInnerRefresh;
    protected AddonControllerEvent? OnInnerUpdate;
    protected AddonControllerEvent? OnInnerPreUpdate;
}
