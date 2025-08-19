using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace KamiToolKit.Classes;
#if DEBUG
public unsafe class Experimental {
#else
internal unsafe class Experimental {
#endif

    private static Experimental? instance;
    public static Experimental Instance => instance ??= new Experimental();

    public void EnableHooks() { }

    public void DisposeHooks() { }

#if DEBUG
    // WARNING: May result in undefined state or accidental network requests
    // Use at your own risk.
    public static void ForceOpenAddon(AgentId agentId)
        => DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
            AgentModule.Instance()->GetAgentByInternalId(agentId)->Show();
        });

    // WARNING: May result in undefined state or accidental network requests
    // Use at your own risk.
    public static void ForceCloseAddon(AgentId agentId)
        => DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
            AgentModule.Instance()->GetAgentByInternalId(agentId)->Hide();
        });
#endif
}
