using System;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit;


/// <summary>
/// This class provides functionality to add-and manage custom elements for any Addon
/// </summary>
public abstract unsafe class AddonController<T> : IDisposable where T : unmanaged {
	[PluginService] public IAddonLifecycle AddonLifecycle { get; set; } = null!;
	[PluginService] public IGameGui GameGui { get; set; } = null!;

	private readonly string addonName;
	private AtkUnitBase* AddonPointer => (AtkUnitBase*)GameGui.GetAddonByName(addonName);

	protected AddonController(IDalamudPluginInterface pluginInterface, string addonName) {
		pluginInterface.Inject(this);
		
		this.addonName = addonName;
	}

	/// <summary>
	/// Enables Addon Setup/Finalize Listeners, and calls AttachNodes if addon is already active.
	/// </summary>
	public void Enable() {
		PreEnable();
		
		AddonLifecycle.RegisterListener(AddonEvent.PostSetup, addonName, OnAddonSetup);
		AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, addonName, OnAddonFinalize);

		if (AddonPointer is not null) {
			AttachNodes((T*) AddonPointer);
		}
		
		PostEnable();
	}

	/// <summary>
	/// Disables Addon Setup/Finalize Listeners, and calls DetachNodes if addon is already active.
	/// </summary>
	public void Disable() {
		PreDisable();
		
		AddonLifecycle.UnregisterListener(OnAddonSetup, OnAddonFinalize);

		if (AddonPointer is not null) {
			DetachNodes((T*) AddonPointer);
		}
		
		PostDisable();
	}
	
	private void OnAddonSetup(AddonEvent type, AddonArgs args)
		=> AttachNodes((T*) args.Addon);

	private void OnAddonFinalize(AddonEvent type, AddonArgs args)
		=> DetachNodes((T*) args.Addon);

	public void Dispose()
		=> Disable();

	/// <summary>
	/// Implement your node creation and attachment logic in this function.
	/// </summary>
	protected abstract void AttachNodes(T* addon);
	
	/// <summary>
	/// Implement your node detachment and disposal logic in this function.
	/// </summary>
	protected abstract void DetachNodes(T* addon);

	/// <summary>
	/// Overridable function that is called before enabling Addon Setup/Finalize listeners
	/// </summary>
	protected virtual void PreEnable() { }
	
	/// <summary>
	/// Overridable function that is called after enabling Addon Setup/Finalize listeners
	/// </summary>
	protected virtual void PostEnable() { }
	
	/// <summary>
	/// Overridable function that is before after disabling Addon Setup/Finalize listeners
	/// </summary>
	protected virtual void PreDisable() { }
	
	/// <summary>
	/// Overridable function that is called after disabling Addon Setup/Finalize listeners
	/// </summary>
	protected virtual void PostDisable() { }
}