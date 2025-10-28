using System.Runtime.CompilerServices;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using KamiToolKit.Classes;

namespace KamiToolKit.Extensions;

public static unsafe class MainThreadSafety {

    /// <summary>
    /// Returns true if <em>not</em> on the main thread. Use this to return early.
    /// </summary>
    public static bool TryAssertMainThread([CallerFilePath] string? callerFilePath = null, [CallerMemberName] string? callerName = null) {
        if (Framework.Instance()->IsUnloading()) return true;

        if (!ThreadSafety.IsMainThread) {
            Log.Error($"{callerFilePath?.Split(@"\")[^1][..^2]}{callerName} must be invoked from the main thread.");
            return true;
        }

        return false;
    }
}
