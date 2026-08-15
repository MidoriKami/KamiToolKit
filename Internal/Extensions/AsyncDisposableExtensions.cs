using System;
using System.Threading.Tasks;

namespace KamiToolKit.Internal.Extensions;

/// <summary>
/// Extensions for handling IAsyncDispose a little easier.
/// </summary>
internal static class AsyncDisposableExtensions {
    extension(IAsyncDisposable? asyncDisposable) {

        /// <summary>
        /// Allows awaiting a nullable async disposables dispose.
        /// </summary>
        /// <returns></returns>
        public async Task DisposeAsyncSafe() {
            if (asyncDisposable is null) return;

            await asyncDisposable.DisposeAsync();
        }
    }
}
