using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Dalamud;

namespace KamiToolKit;

public unsafe partial class NativeAddon {
    private static Hook<AtkUnitBase.Delegates.FireCallback>? fireCallbackHook;

    internal static void InitializeCloseCallback() {
        fireCallbackHook = Services.GameInteropProvider.HookFromAddress<AtkUnitBase.Delegates.FireCallback>(AtkUnitBase.Addresses.FireCallback.Value, OnFireCallback);
        fireCallbackHook.Enable();
    }
    
    private static bool OnFireCallback(AtkUnitBase* thisPtr, uint valueCount, AtkValue* values, bool close) {
        Services.Log.Excessive($"[{thisPtr->NameString}] OnFireCallback");

        foreach (var addon in CreatedAddons) {
            if (addon == thisPtr && close && addon is { RespectCloseAll: true, IsOverlayAddon: false }) {
                addon.Close();
                return true;
            }
        }

        return fireCallbackHook!.Original(thisPtr, valueCount, values, close);
    }

    internal static void DisposeCloseCallback() {
        fireCallbackHook?.Dispose();
        fireCallbackHook = null;
    }
}
