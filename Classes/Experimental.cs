using System;
using System.Runtime.CompilerServices;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Classes;


internal unsafe class Experimental {
	private static Experimental? instance;
	public static Experimental Instance => instance ??= new Experimental();

	public void EnableHooks() {
		// OnUldManagerUpdateHook?.Enable();
		// UpdateUldFromParentHook?.Enable();
	}

	public void DisposeHooks() {
		// OnUldManagerUpdateHook?.Dispose();
		// UpdateUldFromParentHook?.Dispose();
	}

	public delegate void ExpandNodeListSizeDelegate(AtkUldManager* atkUldManager, int newSize);

	[Signature("E8 ?? ?? ?? ?? 66 41 3B B7 ?? ?? ?? ??")]
	public ExpandNodeListSizeDelegate? ExpandNodeListSize = null;
	
	public delegate void DestroyUldManagerDelegate(AtkUldManager* atkUldManager);
	
	[Signature("40 57 48 83 EC 30 0F B6 81 ?? ?? ?? ?? 48 8B F9 A8 01")]
	public DestroyUldManagerDelegate? DestroyUldManager = null;
	
	public delegate void OnUldManagerUpdateDelegate(AtkUldManager* atkUldManager);

	[Signature("E8 ?? ?? ?? ?? 80 BB ?? ?? ?? ?? ?? 74 49", DetourName = nameof(OnUldManagerUpdate))]
	public Hook<OnUldManagerUpdateDelegate>? OnUldManagerUpdateHook = null;

	public void OnUldManagerUpdate(AtkUldManager* atkUldManager) {
		try {
			OnUldManagerUpdateHook!.Original(atkUldManager);

			if (atkUldManager->Assets is not null) {
				if (atkUldManager->Assets->AtkTexture.Resource is not null) {
					if (atkUldManager->Assets->AtkTexture.Resource->TexFileResourceHandle is not null) {
						var path = atkUldManager->Assets->AtkTexture.Resource->TexFileResourceHandle->FileName.ToString();

					}
				}
			}
		}
		catch (Exception e) {
			Log.Exception(e);
		}
	}
	
	public delegate void UpdateUldFromParentDelegate(AtkUldManager* atkUldManager, AtkResNode* atkResNode, nint a3, byte a4);
	
	[Signature("E8 ?? ?? ?? ?? 40 84 F6 74 2A", DetourName = nameof(UpdateUldFromParent))]
	public Hook<UpdateUldFromParentDelegate>? UpdateUldFromParentHook = null;

	public void UpdateUldFromParent(AtkUldManager* atkUldManager, AtkResNode* atkResNode, nint a3, byte a4) {
		try {
			if (atkUldManager->Assets is not null) {
				if (atkUldManager->Assets->AtkTexture.Resource is not null) {
					if (atkUldManager->Assets->AtkTexture.Resource->TexFileResourceHandle is not null) {
						var path = atkUldManager->Assets->AtkTexture.Resource->TexFileResourceHandle->FileName.ToString();
						// Log.Debug(path);
					}
				}
			}
			
			if (atkResNode is null) {
				Log.Fatal("CAUGHT FATAL ERROR, ATKRESNODE IS NULL");
				return;
			}
		}
		catch (Exception e) {
			Log.Exception(e);
		}
		
		UpdateUldFromParentHook!.Original(atkUldManager, atkResNode, a3, a4);
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