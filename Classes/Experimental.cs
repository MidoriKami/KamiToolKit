using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Classes;

internal unsafe class Experimental {
	private static Experimental? instance;
	public static Experimental Instance => instance ??= new Experimental();

	public void EnableHooks() {
	}

	public void DisposeHooks() {
	}

	[Signature("4C 8D 3D ?? ?? ?? ?? 4C 89 3F 41 F6 C4")]
	public nint AtkEventListenerVirtualTable = nint.Zero;

	public delegate AtkUnitBase* GetAddonByNodeDelegate(RaptureAtkUnitManager* manager, AtkResNode* node);

	[Signature("E8 ?? ?? ?? ?? 48 3B E8 75 0E")]
	public GetAddonByNodeDelegate? GetAddonByNode = null;
}
