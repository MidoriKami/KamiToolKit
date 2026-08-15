using System;
using System.Threading.Tasks;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using KamiToolKit.Internal.Classes;

namespace KamiToolKit.Internal.Extensions;

/// <summary>
/// Extensions for making Dalamud's Hooks a little more Async Safe.
/// </summary>
internal static class HookExtensions {
    extension<T>(Hook<T>? hook) where T : Delegate {

        /// <summary>
        /// Enables the hook on the next game tick, on the main thread.
        /// </summary>
        /// <remarks>
        /// This isn't strictly necessary, however it's considered safer to do it this way.
        /// </remarks>
        public async Task EnableAsync() {
            if (hook is null) return;

            await IFramework.Get().Run(hook.Enable);
        }

        /// <summary>
        /// Disables the hook on the next game tick, on the main thread.
        /// </summary>
        /// <remarks>
        /// This isn't strictly necessary, however it's considered safer to do it this way.
        /// </remarks>
        public async Task DisableAsync() {
            if (hook is null) return;

            await IFramework.Get().Run(hook.Disable);
        }

        /// <summary>
        /// Disposes the hook on the next game tick, on the main thread.
        /// </summary>
        /// <remarks>
        /// This isn't strictly necessary, however it's considered safer to do it this way.
        /// </remarks>
        public async Task DisposeAsync() {
            if (hook is null) return;

            await IFramework.Get().Run(hook.Dispose);
        }
    }
}
