using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.System;

public unsafe partial class NodeBase {
	private NativeController? NativeController { get; set; }
	private bool IsAttached { get; set; }
	private AtkUnitBase* AddonPointer { get; set; }
	private AtkComponentBase* ComponentPointer { get; set; }
	private string? AddonName { get; set; }
	
	internal void RegisterAutoDetach(NativeController controller, AtkUnitBase* addon, AtkComponentBase* component = null) {
		if (IsAttached) {
			Log.Verbose("Tried to register auto detach, to already attached node.");
			return;
		}
		
		Log.Verbose($"Registering auto detach, setting up finalize listener for {addon->NameString} for node {GetType()}");
		DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, addon->NameString, OnAddonFinalize);

		AddonPointer = addon;
		ComponentPointer = component;
		AddonName = addon->NameString;
		NativeController = controller;
		IsAttached = true;
	}

	internal void UnregisterAutoDetach() {
		if (!IsAttached) {
			Log.Verbose("Tried to unregister auto detach, to unattached node.");
			return;
		}
		
		Log.Verbose($"Unregistering auto detach gracefully for node {GetType()}");
		DalamudInterface.Instance.AddonLifecycle.UnregisterListener(OnAddonFinalize);
		NativeController = null;
		AddonPointer = null;
		AddonName = null;
		IsAttached = false;
	}
	
	private void OnAddonFinalize(AddonEvent type, AddonArgs args) {
		var addon = (AtkUnitBase*) args.Addon;

		if (ComponentPointer is not null) {
			NativeController?.DetachFromComponent(this, ComponentPointer, addon);
			ComponentPointer = null;
		}
		else {
			NativeController?.DetachFromAddon(this, addon);
		}
	}

	private void TryForceDetach(bool warn) {
		if (!IsAttached) return;

		if (IsAddonPointerValid(AddonPointer, AddonName)) {
			if (warn) Log.Warning($"{GetType()} was not detached from {AddonName} before dispose. Forcing Detach from Addon.");

			if (ComponentPointer is not null) {
				NativeController?.DetachFromComponent(this, ComponentPointer, AddonPointer);
				ComponentPointer = null;
			}
			else {
				NativeController?.DetachFromAddon(this, AddonPointer);
			}
			
			IsAttached = false;
		}
		else {
			Log.Error($"{GetType()} was attached to {AddonName} which is an already finalized addon somehow.");
		}
	}

	private bool IsAddonPointerValid(AtkUnitBase* addon, string? addonName) {
		if (addon is null) return false;
		if (addonName is null) return false;

		var nativePointer = (AtkUnitBase*) DalamudInterface.Instance.GameGui.GetAddonByName(addonName);

		if (nativePointer is null) return false;
		
		return addon == nativePointer;
	}
}
