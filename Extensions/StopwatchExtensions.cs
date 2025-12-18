using System.Diagnostics;
using KamiToolKit.Classes;

namespace KamiToolKit.Extensions;

public static class StopwatchExtensions {
    extension(Stopwatch stopwatch) {
        public void LogTime(string logMessage) {
            DalamudInterface.Instance.Log.Debug($"{logMessage, -15}: {stopwatch, 15} :: {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Restart();
        }
    }
}
