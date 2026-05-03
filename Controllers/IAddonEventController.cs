namespace KamiToolKit.Controllers;

public unsafe interface IAddonEventController<T> where T : unmanaged {
    void Enable();
    void Disable();

    delegate void AddonControllerEvent(T* addon);

    /// <summary>
    /// Called when the addon finishes setting up, or immediately if the addon is already setup.
    /// </summary>
    AddonControllerEvent? OnSetup { get; init; }

    /// <summary>
    /// Called before the addon begins to finalize
    /// </summary>
    AddonControllerEvent? OnFinalize { get; init; }

    /// <summary>
    /// Called before addon Refresh or RequestedUpdate
    /// </summary>
    AddonControllerEvent? OnPreRefresh { get; init; }

    /// <summary>
    /// Called after addon Refresh or RequestedUpdate
    /// </summary>
    AddonControllerEvent? OnRefresh { get; init; }

    /// <summary>
    /// Called before addon Update
    /// </summary>
    AddonControllerEvent? OnPreUpdate { get; init; }

    /// <summary>
    /// Called after addon Update
    /// </summary>
    AddonControllerEvent? OnUpdate { get; init; }
}
