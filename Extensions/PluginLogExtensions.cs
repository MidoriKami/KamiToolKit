using System;
using System.Runtime.CompilerServices;
using Dalamud.Plugin.Services;

namespace KamiToolKit.Extensions;

/// <summary>
/// Extension methods for IPluginLog
/// </summary>
public static class PluginLogExtensions {

    /// <summary>
    /// Gets or sets a value that enables or disables excessive logging.
    /// Enabling this will likely spam the log several times a frame as events such as Draw and Update are logged to Excessive.
    /// </summary>
    public static bool EnableExcessiveLogging { get; set; } = false;

    extension(IPluginLog log) {

        /// <summary>
        /// Logs an exception using context from the callers name.
        /// </summary>
        public void Exception(Exception e, [CallerMemberName] string callerName = "")
            => log.Error(e, $"Exception from {callerName}");

        /// <summary>
        /// Logs a message with excessive logging, can be toggled with <see cref="PluginLogExtensions.EnableExcessiveLogging"/>
        /// </summary>
        public void Excessive(string message) {
            if (EnableExcessiveLogging) {
                log.Verbose(message);
            }
        }
    }
}
