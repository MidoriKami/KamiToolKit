using System.Runtime.CompilerServices;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit;


internal unsafe class Experimental {
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

// vf3  - [InitializeAtkUldManager] Initialize function, sets AtkUldManager fields to zero (mostly)
// vf4  - [DeinitializeAtkUldManager] Deinitialize function, unregisters tween, clears resources, sets state to unloaded
// vf6  - [LoadFromUld] Reads UldResourceHandle to build objects/nodes/parts
// vf8  - (No base implementation) [RegisterEvents] Registers events with AtkComponentBase.AtkResNode as the target node
// vf9  - (No base implementation)
// vf11 - Something to do with Priority
// vf12 - Something to do with Priority
// vf13 - Potentially some kind of sound effect
// vf15 - Iterates DuplicateObjectList and calls some function on each node
// vf16 - (No base implementation)
// vf18 - Some kind of collision check, returns AtkCollisionNode
internal static unsafe class AtkComponentBaseExtensions {
	public static void RegisterEvents(ref this AtkComponentBase atkComponentBase) {
		var ptr = (AtkComponentBase*) Unsafe.AsPointer(ref atkComponentBase);
		
		((delegate* unmanaged<AtkComponentBase*, void>) (*(nint**) ptr)[8])(ptr);
	}

	public static void InitializeAtkUldManager(ref this AtkComponentBase atkComponentBase) {
		var ptr = (AtkComponentBase*) Unsafe.AsPointer(ref atkComponentBase);
		
		((delegate* unmanaged<AtkComponentBase*, void>) (*(nint**) ptr)[3])(ptr);
	}

	public static void DeinitializeAtkUldManager(ref this AtkComponentBase atkComponentBase) {
		var ptr = (AtkComponentBase*) Unsafe.AsPointer(ref atkComponentBase);
		
		((delegate* unmanaged<AtkComponentBase*, void>) (*(nint**) ptr)[4])(ptr);
	}
}