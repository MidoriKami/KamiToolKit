using System;
using Dalamud.Game.Addon.Events;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes.ComponentNodes.Abstract;

public abstract unsafe class ButtonBase : ComponentNode<AtkComponentButton, AtkUldComponentDataButton> {

	protected AtkComponentButton* ButtonNode => (AtkComponentButton*) InternalNode;

	protected ButtonBase() {
		SetInternalComponentType(ComponentType.Button);
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			NativeMemoryHelper.UiFree(Data);
			Data = null;

			base.Dispose(disposing);
		}
	}
	
	private Action? InternalOnClick { get; set; }

	public Action? OnClick {
		get => InternalOnClick;
		set {
			if (value is null ) {
				if (InternalOnClick is not null) {
					RemoveEvent(AddonEventType.ButtonClick, InternalOnClick);
					InternalOnClick = null;
				}
			}
			else {
				if (InternalOnClick is not null) {
					AddEvent(AddonEventType.ButtonClick, InternalOnClick);
					AddEvent(AddonEventType.ButtonClick, value);
					InternalOnClick = value;
				}
				else {
					AddEvent(AddonEventType.ButtonClick, value);
					InternalOnClick = value;
				}
			}
		}
	}
	
	public void SetEnabled(bool enable)
		=> ComponentBase->SetEnabledState(enable);

	public void Disable()
		=> ComponentBase->SetEnabledState(false);

	public void Enable()
		=> ComponentBase->SetEnabledState(false);

	public void Toggle()
		=> ComponentBase->SetEnabledState(!Component->IsEnabled);
}