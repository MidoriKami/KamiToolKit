using System;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Config;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using AddonNamePlateNumberArray = FFXIVClientStructs.FFXIV.Client.UI.AddonNamePlate.AddonNamePlateNumberArray;

namespace KamiToolKit;

/// <summary>
/// Simplified controller for using AddonNamePlate for basic overlays.
/// </summary>
public abstract unsafe class NativeUiOverlayController(IAddonLifecycle addonLifecycle, IFramework framework, IGameGui gameGui, IGameConfig gameConfig) : IDisposable {
	private AddonNamePlate* AddonNamePlate => (AddonNamePlate*) gameGui.GetAddonByName("NamePlate");

	public void Load() {
		LoadConfig();
		
		// Force 4k / HD textures, so things map correctly.
		gameConfig.Set(SystemConfigOption.UiAssetType, 1);
		
		addonLifecycle.RegisterListener(AddonEvent.PostSetup, "NamePlate", OnNamePlateSetup);
		addonLifecycle.RegisterListener(AddonEvent.PreFinalize, "NamePlate", OnNamePlateFinalize);
		
		if (AddonNamePlate is not null) {
			AttachToNative(AddonNamePlate);

			AddonNamePlate->DoFullUpdate = 1;
			AddonNamePlateNumberArray.Instance()->DoFullUpdate = true;
		}
	}
	
	public void Unload() {
		addonLifecycle.UnregisterListener(OnNamePlateSetup);
		addonLifecycle.UnregisterListener(OnNamePlateFinalize);

		if (AddonNamePlate is not null) {
			DetachFromNative(AddonNamePlate);
		}
	}
	
	public void Dispose()
		=> Unload();

	private void OnNamePlateSetup(AddonEvent type, AddonArgs args)
		=> AttachToNative((AddonNamePlate*)args.Addon);

	private void AttachToNative(AddonNamePlate* addonNamePlate)
		=> framework.RunOnFrameworkThread(() => AttachNodes(addonNamePlate));

	private void OnNamePlateFinalize(AddonEvent type, AddonArgs args)
		=> DetachFromNative((AddonNamePlate*)args.Addon);

	private void DetachFromNative(AddonNamePlate* addonNamePlate)
		=> framework.RunOnFrameworkThread(() => DetachNodes(addonNamePlate));

	protected abstract void AttachNodes(AddonNamePlate* addonNamePlate);
	protected abstract void DetachNodes(AddonNamePlate* addonNamePlate);
	protected abstract void LoadConfig();

	protected void RefreshAddon() {
		if (AddonNamePlate is not null) {
			if (AddonNamePlate->UldManager.LoadedState is AtkLoadState.Loaded) {
				AddonNamePlate->UldManager.UpdateDrawNodeList();
			}
			
			AddonNamePlate->UpdateCollisionNodeList(false);
		}
	}
}