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

    public delegate int GetIconFormatStringDelegate(byte* buffer, uint iconId, int textureScale, IconSubFolder subFolder);
    
    [Signature("E8 ?? ?? ?? ?? 90 48 FF C7")]
    public GetIconFormatStringDelegate? GetIconPath = null;
    
    [Signature("4C 8D 3D ?? ?? ?? ?? 4C 89 3F 41 F6 C4")]
    public nint AtkEventListenerVirtualTable = nint.Zero;

    public delegate void AtkComponentNumericInputSetValueDelegate(AtkComponentNumericInput* thisPtr, int value, bool triggerCallback, bool playSoundEffect);

    [Signature("E8 ?? ?? ?? ?? 40 80 FE ?? 74")]
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
