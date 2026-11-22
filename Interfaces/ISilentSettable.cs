namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a value that can be set without triggering callbacks.
/// </summary>
public interface ISilentSettable<in T> {

    /// <summary>
    /// Sets the value without triggering any update callback.
    /// Use this when initializing or resetting values on existing nodes.
    /// </summary>
    void SetValueSilent(T value);
}
