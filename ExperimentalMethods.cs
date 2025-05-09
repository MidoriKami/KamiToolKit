using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit;

public unsafe class ExperimentalMethods {
	private static ExperimentalMethods? instance;
	public static ExperimentalMethods Instance => instance ??= new ExperimentalMethods();
	public static bool Initialized { get; private set; }

	[PluginService] public IGameInteropProvider Hooker { get; set; } = null!; // Still not gonna call it a GameInteropProvider

	public static void Initialize(IDalamudPluginInterface pluginInterface) {
		pluginInterface.Inject(Instance);
		Initialized = true;
		
		Instance.Hooker.InitializeFromAttributes(Instance);
	}

	public delegate void ExpandNodeListSizeDelegate(AtkUldManager* atkUldManager, int newSize);

	[Signature("E8 ?? ?? ?? ?? 66 41 3B B7 ?? ?? ?? ??")]
	public ExpandNodeListSizeDelegate? ExpandNodeListSize = null;
}