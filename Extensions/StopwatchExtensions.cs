using System.Diagnostics;
using KamiToolKit.Dalamud;

namespace KamiToolKit.Extensions;

/// <summary>
/// Stopwatch extension methods. For easily logging how long multiple stages take.
/// </summary>
public static class StopwatchExtensions {
    extension(Stopwatch stopwatch) {

        /// <summary>
        /// Logs the current stopwatch time and resets the stopwatch.
        /// </summary>
        /// <param name="logMessage">Label to print to the log for this time period.</param>
        public void LogTime(string logMessage) {
            Services.Log.Debug($"{logMessage,-15}: {stopwatch,15} :: {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Restart();
        }
    }
}
