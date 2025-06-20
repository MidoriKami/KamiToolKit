using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace KamiToolKit.Classes;
#if DEBUG
public class Experimental {
#else
internal class Experimental {
#endif
	private static Experimental? instance;
	public static Experimental Instance => instance ??= new Experimental();

	public void EnableHooks() {
	}

	public void DisposeHooks() {
	}

	[Signature("4C 8D 3D ?? ?? ?? ?? 4C 89 3F 41 F6 C4")]
	public nint AtkEventListenerVirtualTable = nint.Zero;
	
#if DEBUG
	// WARNING: May result in undefined state or accidental network requests
	// Use at your own risk.
	public static unsafe void ForceOpenAddon(AgentId agentId) {
		DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
			AgentModule.Instance()->GetAgentByInternalId(agentId)->Show();
		});
	}

	// WARNING: May result in undefined state or accidental network requests
	// Use at your own risk.
	public static unsafe void ForceCloseAddon(AgentId agentId) {
		DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
			AgentModule.Instance()->GetAgentByInternalId(agentId)->Hide();
		});
	}
#endif
}
