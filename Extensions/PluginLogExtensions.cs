using System;
using System.Runtime.CompilerServices;
using Dalamud.Plugin.Services;

namespace KamiToolKit.Extensions;

public static class PluginLogExtensions {
    public static bool EnableExcessiveLogging { get; set; } = false;
    
    extension(IPluginLog log) {
        public void Exception(Exception e, [CallerMemberName] string callerName = "")
            => log.Error(e, $"Exception from {callerName}");

        public void Excessive(string message) {
            if (EnableExcessiveLogging) {
                log.Verbose(message);
            }
        }
    }
}
