using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Addon;

public abstract unsafe partial class NativeAddon {

    private static Hook<AtkUnitBase.Delegates.FireCallback>? fireCallbackHook;
    
    private static void InitializeExtras() {
        fireCallbackHook ??= DalamudInterface.Instance.GameInteropProvider
            .HookFromAddress<AtkUnitBase.Delegates.FireCallback>(AtkUnitBase.Addresses.FireCallback.Value, OnFireCallback);
        fireCallbackHook.Enable();
    }
    
    private static bool OnFireCallback(AtkUnitBase* thisPtr, uint valueCount, AtkValue* values, bool close) {
        Log.Excessive($"[{thisPtr->NameString}] OnFireCallback");
        
        foreach (var addon in CreatedAddons) {
            if (addon.InternalAddon == thisPtr && close && addon is { WindowOptions.RespectCloseAll: true }) {
                addon.Close();
                return true;
            }
        }

        return fireCallbackHook!.Original(thisPtr, valueCount, values, close);
    }

    private static void DisposeExtras() {
        if (CreatedAddons.Count is 0) {
            fireCallbackHook?.Dispose();
            fireCallbackHook = null;
        }
    }
}
