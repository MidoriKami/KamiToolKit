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

    [Signature("4C 8D 3D ?? ?? ?? ?? 4C 89 3F 41 F6 C4")]
    public nint AtkEventListenerVirtualTable = nint.Zero;

    public AtkEventManager* ViewportEventManager => (AtkEventManager*)((nint)AtkStage.Instance() + 0x870);

    public delegate void AtkComponentNumericInputSetValueDelegate(AtkComponentNumericInput* thisPtr, int value, bool triggerCallback, bool playSoundEffect);

    [Signature("E9 ?? ?? ?? ?? 33 D2 F7 F1")]
    public AtkComponentNumericInputSetValueDelegate? AtkComponentNumericInputSetValueCallback = null;

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
