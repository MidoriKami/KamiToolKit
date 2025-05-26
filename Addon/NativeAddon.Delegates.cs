using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Addon;

public unsafe partial class NativeAddon {

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate AtkEventListener* DestructorDelegate(AtkUnitBase* addon, byte flags);
	
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate void InitializeDelegate(AtkUnitBase* addon);
}