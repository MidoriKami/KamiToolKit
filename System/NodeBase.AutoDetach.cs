using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.System;

public unsafe partial class NodeBase {
	private NativeController? NativeController { get; set; }
	private bool IsAttached { get; set; }
	private AtkUnitBase* AddonPointer { get; set; }
	private string? AddonName { get; set; }
	
	internal void RegisterAutoDetach(NativeController controller, AtkUnitBase* addon) {
		if (IsAttached) {
			Log.Verbose("Tried to register auto detach, to already attached node.");
			return;
		}
		
		Log.Verbose($"Registering auto detach, setting up finalize listener for {addon->NameString} for node {GetType()}");
		DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, addon->NameString, OnAddonFinalize);

		AddonPointer = addon;
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

		NativeController?.DetachFromAddon(this, addon);
	}

	private void TryForceDetach(bool warn) {
		if (!IsAttached) return;

		if (AddonPointer == (AtkUnitBase*) DalamudInterface.Instance.GameGui.GetAddonByName(AddonName!) && AddonPointer is not null) {
			if (warn) Log.Warning($"{GetType()} was not detached from {AddonName} before dispose. Forcing Detach from Addon.");
			NativeController?.DetachFromAddon(this, AddonPointer);
			IsAttached = false;
		}
		else {
			Log.Error($"{GetType()} was attached to {AddonName} which is an already finalized addon somehow.");
		}
	}
}
