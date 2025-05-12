using Dalamud.Interface;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace KamiToolKit.Classes;

internal unsafe class DalamudInterface {
	private static DalamudInterface? instance;
	public static DalamudInterface Instance => instance ??= new DalamudInterface();
	
	[PluginService] public IPluginLog Log { get; set; } = null!;
	
	[PluginService] public IDalamudPluginInterface PluginInterface { get; set; } = null!;

	public IUiBuilder UiBuilder => PluginInterface.UiBuilder;

}

internal static class Log {
	internal static void Debug(string message) {
		DalamudInterface.Instance.Log.Debug(message);
	}
}