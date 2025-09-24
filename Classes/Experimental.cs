using System.Diagnostics;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace KamiToolKit.Classes;

/// WARNING: These features are potentially extremely volatile, use at your own risk.
public unsafe class Experimental {
    private static Experimental? instance;
    public static Experimental Instance => instance ??= new Experimental();

    public void EnableHooks() { }

    public void DisposeHooks() { }

    // WARNING: May result in undefined state or accidental network requests
    // Use at your own risk.
    [Conditional("DEBUG")]
    public static void ForceOpenAddon(AgentId agentId)
        => DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
            AgentModule.Instance()->GetAgentByInternalId(agentId)->Show();
        });

    // WARNING: May result in undefined state or accidental network requests
    // Use at your own risk.
    [Conditional("DEBUG")]
    public static void ForceCloseAddon(AgentId agentId)
        => DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
            AgentModule.Instance()->GetAgentByInternalId(agentId)->Hide();
        });
}
