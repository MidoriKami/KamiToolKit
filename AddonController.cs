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
public unsafe class AddonController<T> : IDisposable where T : unmanaged {
	[PluginService] public IAddonLifecycle AddonLifecycle { get; set; } = null!;
	[PluginService] public IGameGui GameGui { get; set; } = null!;
	[PluginService] public IFramework Framework { get; set; } = null!;

	private readonly string addonName;
	private AtkUnitBase* AddonPointer => (AtkUnitBase*)GameGui.GetAddonByName(addonName);
	private bool IsEnabled { get; set; }
	
	public AddonController(IDalamudPluginInterface pluginInterface, string addonName) {
		pluginInterface.Inject(this);
		
		this.addonName = addonName;
	}

	public void Enable() {
		Framework.RunOnFrameworkThread(() => {
			if (IsEnabled) return;

			PreEnable?.Invoke((T*) AddonPointer);

			AddonLifecycle.RegisterListener(AddonEvent.PostSetup, addonName, OnAddonEvent);
			AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, addonName, OnAddonEvent);
			AddonLifecycle.RegisterListener(AddonEvent.PostRefresh, addonName, OnAddonEvent);
			AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, addonName, OnAddonEvent);
			AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, addonName, OnAddonEvent);

			if (AddonPointer is not null) {
				OnAttach?.Invoke((T*) AddonPointer);
			}

			IsEnabled = true;

			PostEnable?.Invoke((T*) AddonPointer);
		});
	}

	private void OnAddonEvent(AddonEvent type, AddonArgs args) {
		var addon = (T*) args.Addon;

		switch (type) {
			case AddonEvent.PostSetup:
				OnAttach?.Invoke(addon);
				return;
			
			case AddonEvent.PreFinalize:
				OnDetach?.Invoke(addon);
				return;
			
			case AddonEvent.PostRefresh or AddonEvent.PostRequestedUpdate:
				OnRefresh?.Invoke(addon);
				return;
			
			case AddonEvent.PostUpdate:
				OnUpdate?.Invoke(addon);
				return;
		}
	}

	public void Disable() {
		Framework.RunOnFrameworkThread(() => {
			if (!IsEnabled) return;

			PreDisable?.Invoke((T*) AddonPointer);

			AddonLifecycle.UnregisterListener(OnAddonEvent);

			if (AddonPointer is not null) {
				OnDetach?.Invoke((T*) AddonPointer);
			}

			IsEnabled = false;

			PostDisable?.Invoke((T*) AddonPointer);
		});
	}

	public virtual void Dispose() => Disable();

	public delegate void AddonControllerEvent(T* addon);

	public event AddonControllerEvent? OnAttach;
	public event AddonControllerEvent? OnDetach;
	public event AddonControllerEvent? OnRefresh;
	public event AddonControllerEvent? OnUpdate;
	
	public event AddonControllerEvent? PreEnable;
	public event AddonControllerEvent? PostEnable;
	public event AddonControllerEvent? PreDisable;
	public event AddonControllerEvent? PostDisable;
}