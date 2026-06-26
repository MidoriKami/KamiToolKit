namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing the functions expected of various AddonEvent Controllers.
/// </summary>
public unsafe interface IAddonEventController<T> where T : unmanaged {

    /// <summary>
    /// Enable the events for this AddonEvent Controller.
    /// </summary>
    void Enable();

    /// <summary>
    /// Disable/Disposes the events for this AddonEvent Controller.
    /// </summary>
    void Disable();

    /// <summary>
    /// Standard delegate method used for all AddonController Events.
    /// </summary>
    delegate void AddonControllerEvent(T* addon);

    /// <summary>
    /// Called when the addon finishes setting up, or immediately if the addon is already setup.
    /// </summary>
    AddonControllerEvent? OnSetup { get; init; }

    /// <summary>
    /// Called before the addon begins to finalize.
    /// </summary>
    AddonControllerEvent? OnFinalize { get; init; }

    /// <summary>
    /// Called before addon Refresh or RequestedUpdate.
    /// </summary>
    AddonControllerEvent? OnPreRefresh { get; init; }

    /// <summary>
    /// Called after addon Refresh or RequestedUpdate.
    /// </summary>
    AddonControllerEvent? OnRefresh { get; init; }

    /// <summary>
    /// Called before addon Update.
    /// </summary>
    AddonControllerEvent? OnPreUpdate { get; init; }

    /// <summary>
    /// Called after addon Update.
    /// </summary>
    AddonControllerEvent? OnUpdate { get; init; }

    /// <summary>
    /// Called before addon draw.
    /// </summary>
    AddonControllerEvent? OnDraw {get; init; }
}
