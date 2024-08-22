using System;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using AddonNamePlateNumberArray = FFXIVClientStructs.FFXIV.Client.UI.AddonNamePlate.AddonNamePlateNumberArray;

namespace KamiToolKit;

/// <summary>
/// Simplified controller for using AddonNamePlate for basic overlays.
/// </summary>
public abstract unsafe class NativeUiOverlayController(IAddonLifecycle addonLifecycle, IFramework framework, IGameGui gameGui) : IDisposable {
	private AddonNamePlate* AddonNamePlate => (AddonNamePlate*) gameGui.GetAddonByName("NamePlate");

	/// <summary>
	/// Enable this overlay controller, it will call PreAttach and AttachNodes anytime the NamePlate addon loads, and calls DetachNodes when it unloads. 
	/// </summary>
	public void Enable() {
		PreAttach();
		
		addonLifecycle.RegisterListener(AddonEvent.PostSetup, "NamePlate", OnNamePlateSetup);
		addonLifecycle.RegisterListener(AddonEvent.PreFinalize, "NamePlate", OnNamePlateFinalize);
		
		if (AddonNamePlate is not null) {
			AttachToNative(AddonNamePlate);
			RefreshAddon();
		}
	}
	
	/// <summary>
	/// Disable this overlay controller, unregisters listeners, and if the NamePlate addon is loaded, calls DetachNodes.
	/// </summary>
	public void Disable() {
		addonLifecycle.UnregisterListener(OnNamePlateSetup);
		addonLifecycle.UnregisterListener(OnNamePlateFinalize);

		if (AddonNamePlate is not null) {
			DetachFromNative(AddonNamePlate);
		}
	}
	
	/// <summary>
	/// Disables and unloads controller.
	/// </summary>
	public void Dispose()
		=> Disable();

	private void OnNamePlateSetup(AddonEvent type, AddonArgs args)
		=> AttachToNative((AddonNamePlate*)args.Addon);

	private void AttachToNative(AddonNamePlate* addonNamePlate)
		=> framework.RunOnFrameworkThread(() => AttachNodes(addonNamePlate));

	private void OnNamePlateFinalize(AddonEvent type, AddonArgs args)
		=> DetachFromNative((AddonNamePlate*)args.Addon);

	private void DetachFromNative(AddonNamePlate* addonNamePlate)
		=> framework.RunOnFrameworkThread(() => DetachNodes(addonNamePlate));

	protected abstract void PreAttach();
	protected abstract void AttachNodes(AddonNamePlate* addonNamePlate);
	protected abstract void DetachNodes(AddonNamePlate* addonNamePlate);

	private void RefreshAddon() {
		if (AddonNamePlate is not null) {
			if (AddonNamePlate->UldManager.LoadedState is AtkLoadState.Loaded) {
				AddonNamePlate->UldManager.UpdateDrawNodeList();
			}
			
			AddonNamePlate->UpdateCollisionNodeList(false);
			
			AddonNamePlate->DoFullUpdate = 1;
			AddonNamePlateNumberArray.Instance()->DoFullUpdate = true;
		}
	}
}