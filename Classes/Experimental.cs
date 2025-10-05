using System.Diagnostics;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Classes;

/// WARNING: These features are potentially extremely volatile, use at your own risk.
public unsafe class Experimental {
    private static Experimental? instance;
    public static Experimental Instance => instance ??= new Experimental();

    public void EnableHooks() { }

    public void DisposeHooks() { }

    [Signature("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 48 8D 05 ?? ?? ?? ?? 48 8B F1 48 89 01 8B FA 48 83 C1 18 E8 ?? ?? ?? ?? 48 8D 4E 18 E8 ?? ?? ?? ?? 48 8D 05 ?? ?? ?? ?? 48 89 06 40 F6 C7 01 74 0D BA ?? ?? ?? ?? 48 8B CE E8 ?? ?? ?? ?? 48 8B 5C 24 ?? 48 8B C6 48 8B 74 24 ?? 48 83 C4 20 5F C3 40 53 48 83 EC 20 48 8D 05 ?? ?? ?? ?? 48 8B D9 48 89 01 F6 C2 01 74 0A BA ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B C3 48 83 C4 20 5B C3 CC CC CC CC CC 40 53 48 83 EC 20 48 8D 05 ?? ?? ?? ?? 48 8B D9 48 89 01 48 8D 05 ?? ?? ?? ??")]
    public AtkResNode.Delegates.Destroy? StaticAtkResNodeDestroyFunction = null;

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
