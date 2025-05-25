using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Classes;


internal unsafe class Experimental {
	private static Experimental? instance;
	public static Experimental Instance => instance ??= new Experimental();

	public void EnableHooks() {
		// OnUldManagerUpdateHook?.Enable();
		// UpdateUldFromParentHook?.Enable();
		// ButtonReceiveEventHook?.Enable();
		// CreateTimelineManagerHook?.Enable();
	}

	public void DisposeHooks() {
		OnUldManagerUpdateHook?.Dispose();
		UpdateUldFromParentHook?.Dispose();
		ButtonReceiveEventHook?.Dispose();
		CreateTimelineManagerHook?.Dispose();
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
	
	public delegate void AtkComponentButtonReceiveEventDelegate(AtkComponentButton* atkComponentButton, AtkEventType eventType, int param, AtkEvent* eventPointer, AtkEventData* eventData);
	
	[Signature("E8 ?? ?? ?? ?? 48 8B BE ?? ?? ?? ?? 48 85 FF 74 11", DetourName = nameof(ButtonReceiveEvent))]
	public Hook<AtkComponentButtonReceiveEventDelegate>? ButtonReceiveEventHook = null;
	
	internal List<nint> TrackedComponents = [];
	public void ButtonReceiveEvent(AtkComponentButton* atkComponentButton, AtkEventType eventType, int param, AtkEvent* eventPointer, AtkEventData* eventData) {
		ButtonReceiveEventHook!.Original(atkComponentButton, eventType, param, eventPointer, eventData);

		try {
			if (TrackedComponents.Contains((nint) atkComponentButton)) {
				Log.Debug("Event Received");
			}
			else {
				var parsedComponents = string.Join(", ", TrackedComponents.Select(component => $"{component:X}"));

				Log.Debug($"Event Received, but not matched: Actual: {(nint) atkComponentButton:X}, Desired: {parsedComponents}");
			}
		}
		catch (Exception e) {
			Log.Exception(e);
		}
	}
	
	public delegate int CreateTimelineManagerDelegate(AtkUldManager* atkUldManager, IMemorySpace* memorySpace, nint a3, nint a4, ushort a5);
	
	[Signature("40 57 41 56 41 57 48 83 EC 30 45 8B 71 18", DetourName = nameof(CreateTimelineManagerDetour))]
	public Hook<CreateTimelineManagerDelegate>? CreateTimelineManagerHook = null;

	public int CreateTimelineManagerDetour(AtkUldManager* atkUldManager, IMemorySpace* memorySpace, nint a3, nint a4, ushort a5) {
		var result = CreateTimelineManagerHook!.Original(atkUldManager, memorySpace, a3, a4, a5);

		try {
			Log.Debug($"a3: {a3:X}");
			Log.Debug($"a4: {a4:X}");
			Log.Debug($"a5: {a5}");
		}
		catch (Exception e) {
			Log.Exception(e);
		}

		return result;
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
// vf19 - [Button] GetHeight
// vf22 - [Button] Seems to center the text inside the button
// vf23 - [Button] Also seems to do the same, but slightly different
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

	public static void InitializeFromComponentData(ref this AtkComponentBase atkComponentBase, AtkUldComponentDataBase* atkUldComponentData) {
		var ptr = (AtkComponentBase*) Unsafe.AsPointer(ref atkComponentBase);
		
		((delegate* unmanaged<AtkComponentBase*, AtkUldComponentDataBase*, void>) (*(nint**) ptr)[17])(ptr, atkUldComponentData);
	}
	
	public static void UpdateComponentLayout(ref this AtkComponentBase atkComponentBase) {
		var ptr = (AtkComponentBase*) Unsafe.AsPointer(ref atkComponentBase);
		
		((delegate* unmanaged<AtkComponentBase*, void>) (*(nint**) ptr)[22])(ptr);
	}
}