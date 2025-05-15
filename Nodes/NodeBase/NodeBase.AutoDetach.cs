using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public unsafe partial class NodeBase {
	private AtkUnitBase* AttachedAddon { get; set; }
	private string? AttachedAddonName { get; set; }
	private NativeController? NativeController { get; set; }
	private bool IsAttached { get; set; }
	
	internal void TryRegisterAutoDetach(NativeController controller, AtkUnitBase* addon) {
		if (NativeController is not null) return;
		if (AttachedAddon is not null) return;
		if (AttachedAddonName is not null) return;
		
		NativeController = controller;
		AttachedAddon = addon;
		AttachedAddonName = addon->NameString;
		IsAttached = true;
	}

	internal void TryUnregisterAutoDetach() {
		if (NativeController is null) return;
		if (AttachedAddon is null) return;
		if (AttachedAddonName is null) return;

		NativeController = null;
		AttachedAddon = null;
		AttachedAddonName = null;
		IsAttached = false;
	}

	/// <summary>
	/// Attempts to remove this node from its attached addon
	/// </summary>
	public bool TryDetach() {
		if (!IsAttached) return false;
		
		if (NativeController is null) return false;
		if (AttachedAddon is null) return false;
		if (!IsAddonPointerValid()) return false;

		NativeController.DetachFromAddon(this, AttachedAddon);
		return true;
	}
	
	private void TryAutoDetach() {
		// Only log a warning if it was actually detached from an active addon
		if (TryDetach()) {
			Log.Warning($"{GetType()} was not detached before disposal, failsafe has detached this node from native UI");
		}
	}

	private bool IsAddonPointerValid() {
		if (AttachedAddon is null) return false;
		if (AttachedAddonName is null) return false;
		
		var currentPointer = (AtkUnitBase*) DalamudInterface.Instance.GameGui.GetAddonByName(AttachedAddonName);
		if (currentPointer is null) return false;

		if (currentPointer != AttachedAddon) {
			Log.Debug($"{GetType()} Stored AddonPointer ({(nint)AttachedAddon:X}) doesn't match CurrentAddon ({(nint)currentPointer:X}) Pointer. {AttachedAddonName}");
			return false;
		}

		return true;
	}
}
