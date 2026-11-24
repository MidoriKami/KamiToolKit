using System;
using Dalamud.Plugin.Services;

namespace KamiToolKit.Internal.Classes;

/// <summary>
/// Extension provider for IDalamudService, to add a .Get() method to get an instance of any dalamud service directly from typename.
/// </summary>
/// <code>
/// IPluginLog.Get().Debug(...);
/// </code>
internal static class ServiceExtension {

    /// <summary>
    /// Static class to hold the instance reference.
    /// </summary>
    private static class ServiceInstance<T> where T : class, IDalamudService {
        public static T? Instance => field ??= KamiToolKitLibrary.PluginInterface.GetService(typeof(T)) as T;
    }

    /// <summary>
    /// Extension provider to allow you to .Get() from the interface type.
    /// </summary>
    extension<T>(T) where T : class, IDalamudService {
        public static T Get() => ServiceInstance<T>.Instance ?? throw new InvalidOperationException($"Service {typeof(T).Name} not found.");
    }
}
