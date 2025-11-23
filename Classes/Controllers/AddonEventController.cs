using System;

namespace KamiToolKit.Classes.Controllers;

public abstract unsafe class AddonEventController<T> where T : unmanaged {
    
    public delegate void AddonControllerEvent(T* addon);
    
    public event AddonControllerEvent? OnAttach {
        add => OnInnerAttach += value;
        remove => throw new Exception("Do not remove events, on dispose addon state will be managed properly.");
    }

    public event AddonControllerEvent? OnDetach {
        add => OnInnerDetach += value;
        remove => throw new Exception("Do not remove events, on dispose addon state will be managed properly.");
    }

    public event AddonControllerEvent? OnRefresh {
        add => OnInnerRefresh += value;
        remove => throw new Exception("Do not remove events, on dispose addon state will be managed properly.");
    }

    public event AddonControllerEvent? OnUpdate {
        add => OnInnerUpdate += value;
        remove => throw new Exception("Do not remove events, on dispose addon state will be managed properly.");
    }



    protected AddonControllerEvent? OnInnerAttach;
    protected AddonControllerEvent? OnInnerDetach;
    protected AddonControllerEvent? OnInnerRefresh;
    protected AddonControllerEvent? OnInnerUpdate;

}
