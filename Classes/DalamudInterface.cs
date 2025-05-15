using System;
using Dalamud.IoC;
using Dalamud.Plugin.Services;

namespace KamiToolKit.Classes;

internal class DalamudInterface {
	private static DalamudInterface? instance;
	public static DalamudInterface Instance => instance ??= new DalamudInterface();
	
	[PluginService] public IPluginLog Log { get; set; } = null!;
	[PluginService] public IGameGui GameGui { get; set; } = null!;
}

internal static class Log {
	internal static void Debug(string message) {
		DalamudInterface.Instance.Log.Debug(message);
	}

	internal static void Fatal(string message) {
		DalamudInterface.Instance.Log.Fatal(message);
	}

	internal static void Warning(string message) {
		DalamudInterface.Instance.Log.Warning(message);
	}

	internal static void Exception(Exception exception) {
		DalamudInterface.Instance.Log.Error(exception, "Shit broke yo.");
	}
}