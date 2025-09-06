using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;

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

    public delegate bool SetFocusDelegate(AtkInputManager* inputManager, AtkResNode* resNode, AtkUnitBase* addon, int focusParam);

    [Signature("E8 ?? ?? ?? ?? 49 8B 84 FF ?? ?? ?? ??")]
    public SetFocusDelegate? SetFocus = null;

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
