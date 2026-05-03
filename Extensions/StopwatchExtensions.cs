using System.Diagnostics;
using KamiToolKit.Dalamud;

namespace KamiToolKit.Extensions;

public static class StopwatchExtensions {
    extension(Stopwatch stopwatch) {
        public void LogTime(string logMessage) {
            Services.Log.Debug($"{logMessage,-15}: {stopwatch,15} :: {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Restart();
        }
    }
}
