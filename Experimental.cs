using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit;

public unsafe class Experimental {
	private static Experimental? instance;
	public static Experimental Instance => instance ??= new Experimental();
	public static bool Initialized { get; private set; }

	[PluginService] private IGameInteropProvider Hooker { get; set; } = null!; // Still not gonna call it a GameInteropProvider
	[PluginService] public IPluginLog Log { get; set; } = null!;

	public static void Initialize(IDalamudPluginInterface pluginInterface) {
		pluginInterface.Inject(Instance);
		Initialized = true;
		
		Instance.Hooker.InitializeFromAttributes(Instance);
	}

	public delegate void ExpandNodeListSizeDelegate(AtkUldManager* atkUldManager, int newSize);

	[Signature("E8 ?? ?? ?? ?? 66 41 3B B7 ?? ?? ?? ??")]
	public ExpandNodeListSizeDelegate? ExpandNodeListSize = null;
	
	public delegate void DestroyUldManagerDelegate(AtkUldManager* atkUldManager);
	
	[Signature("40 57 48 83 EC 30 0F B6 81 ?? ?? ?? ?? 48 8B F9 A8 01")]
	public DestroyUldManagerDelegate? DestroyUldManager = null;
}

internal static class Log {
	internal static void Debug(string message) {
		Experimental.Instance.Log.Debug(message);
	}
}