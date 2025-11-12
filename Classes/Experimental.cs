using System.Diagnostics;
using System.Runtime.InteropServices;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Classes;

/// WARNING: These features are potentially extremely volatile, use at your own risk.
public unsafe class Experimental {
    private static Experimental? instance;
    public static Experimental Instance => instance ??= new Experimental();

    public void EnableHooks() { }

    public void DisposeHooks() {
        ProcessCursorFlagsHook?.Dispose();
    }

    // KeyModifiers:
    // [0] = Control
    // [1] = Shift
    // [2] = Alt
    public delegate bool ProcessKeyShortcutDelegate(AtkTextInput* textInput, SeVirtualKey a2, KeyModifiers* keyModifiers);

    [Signature("E8 ?? ?? ?? ?? 84 C0 0F 85 ?? ?? ?? ?? C6 44 24 ?? ??")]
    public ProcessKeyShortcutDelegate? ProcessKeyShortcutFunction = null;

    [StructLayout(LayoutKind.Explicit, Size = 0x06)]
    public struct KeyModifiers {
        [FieldOffset(0x00)] public bool IsControlDown;
        [FieldOffset(0x01)] public bool IsShiftDown;
        [FieldOffset(0x02)] public bool IsAltDown;
        [FieldOffset(0x03)] public bool IsCapitalDown;
        [FieldOffset(0x04)] public bool IsNumlockDown;
        [FieldOffset(0x05)] public bool IsScrollDown;
    }

    public delegate void ProcessCursorFlagsDelegate(RaptureAtkUnitManager* unitManager);

    // Unused at the moment, might consider using unused flag bits to handle grabby hand
    // sub_1400E1880
    [Signature("E8 ?? ?? ?? ?? 0F 28 CE 48 8B CB E8 ?? ?? ?? ?? 0F 28 CE 48 8D 8B ?? ?? ?? ??", DetourName = nameof(ProcessCursorFlags))]
    public Hook<ProcessCursorFlagsDelegate>? ProcessCursorFlagsHook = null;

    private void ProcessCursorFlags(RaptureAtkUnitManager* unitManager)
        => ProcessCursorFlagsHook?.Original(unitManager);

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
