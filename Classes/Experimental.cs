using Dalamud.Utility.Signatures;

namespace KamiToolKit.Classes;

internal class Experimental {
	private static Experimental? instance;
	public static Experimental Instance => instance ??= new Experimental();

	public void EnableHooks() {
	}

	public void DisposeHooks() {
	}

	[Signature("4C 8D 3D ?? ?? ?? ?? 4C 89 3F 41 F6 C4")]
	public nint AtkEventListenerVirtualTable = nint.Zero;
}
