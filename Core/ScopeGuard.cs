using System;

namespace KamiToolKit.Core;

/// <summary>
/// Disposes an action when the scope ends, typically used with <c>using</c> to
/// temporarily set and then restore a boolean flag.
/// </summary>
/// <param name="restoreAction">The action to restore the previous state when disposed.</param>
public readonly struct ScopeGuard(Action restoreAction) : IDisposable {
    /// <inheritdoc />
    public void Dispose() => restoreAction();
}
