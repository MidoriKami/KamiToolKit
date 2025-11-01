using System.Diagnostics;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace KamiToolKit.Classes;

/// WARNING: These features are potentially extremely volatile, use at your own risk.
public unsafe class Experimental {
    private static Experimental? instance;
    public static Experimental Instance => instance ??= new Experimental();

    public void EnableHooks() { }

    public void DisposeHooks() { }

    public delegate void ProcessCursorFlags(RaptureAtkUnitManager* unitManager);

    // Unused at the moment, might consider using unused flag bits to handle grabby hand
    // sub_1400E1880
    [Signature("E8 ?? ?? ?? ?? 0F 28 CE 48 8B CB E8 ?? ?? ?? ?? 0F 28 CE 48 8D 8B ?? ?? ?? ??")]
    public Hook<ProcessCursorFlags>? ProcessCursorFlagsHook = null;

    // WARNING: May result in undefined state or accidental network requests
    // Use at your own risk.
    [Conditional("DEBUG")]
    public static void ForceOpenAddon(AgentId agentId, int delayTicks = 0) {
        if (delayTicks is not 0) {
            DalamudInterface.Instance.Framework.RunOnTick(() => {
                AgentModule.Instance()->GetAgentByInternalId(agentId)->Show();
            }, delayTicks: delayTicks);
        }
        else {
            DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
                AgentModule.Instance()->GetAgentByInternalId(agentId)->Show();
            });
        }
    }

    // WARNING: May result in undefined state or accidental network requests
    // Use at your own risk.
    [Conditional("DEBUG")]
    public static void ForceCloseAddon(AgentId agentId)
        => DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
            AgentModule.Instance()->GetAgentByInternalId(agentId)->Hide();
        });
}
