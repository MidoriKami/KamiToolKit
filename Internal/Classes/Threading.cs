namespace KamiToolKit.Internal.Classes;

/// <summary>
/// Static helper class for handling the case where something should be done on the main thread, but the game is shutting down.
/// </summary>
internal static class Threading {

    /// <summary>
    /// Assert that this is being called from the main thread, unless the game is shutting down.
    /// If the game is shutting down, return true.
    /// </summary>
    /// <returns>
    /// True when the associated functions should be <b>skipped</b>.
    /// </returns>
    internal static bool AssertMainThreadOrUnloading() {
        if (Services.Framework.IsFrameworkUnloading) return true;
        Dalamud.Utility.ThreadSafety.AssertMainThread();

        return false;
    }

    /// <summary>
    /// Assert that this is <b>not</b> being called from the main thread, unless the game is shutting down.
    /// If the game is shutting down, return true.
    /// </summary>
    /// <returns>
    /// True when the associated functions should be <b>skipped</b>.
    /// </returns>
    internal static bool AssertNotMainThreadOrUnloading() {
        if (Services.Framework.IsFrameworkUnloading) return true;
        Dalamud.Utility.ThreadSafety.AssertNotMainThread();

        return false;
    }
}
