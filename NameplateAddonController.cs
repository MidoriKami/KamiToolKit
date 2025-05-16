using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Arrays;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit;

/// <summary>
/// Simplified controller for using AddonNamePlate for basic overlays.
/// </summary>
public unsafe class NameplateAddonController : AddonController<AddonNamePlate> {
	public NameplateAddonController(IDalamudPluginInterface pluginInterface) : base(pluginInterface, "NamePlate") {
		PostEnable += RefreshAddon;
		PostDisable += RefreshAddon;
	}

	public override void Dispose() {
		PostEnable -= RefreshAddon;
		PostDisable -= RefreshAddon;
		
		base.Dispose();
	}

	private void RefreshAddon(AddonNamePlate* addon) {
		if (addon is not null) {
			if (addon->UldManager.LoadedState is AtkLoadState.Loaded) {
				addon->UldManager.UpdateDrawNodeList();
			}
			
			addon->UpdateCollisionNodeList(false);
			
			addon->DoFullUpdate = 1;
			NamePlateNumberArray.Instance()->DoFullUpdate = true;
		}
	}
}