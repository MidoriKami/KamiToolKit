using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Arrays;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit;

/// <summary>
///     Simplified controller for using AddonNamePlate for basic overlays.
/// </summary>
public sealed unsafe class NameplateAddonController : AddonController<AddonNamePlate> {
    public NameplateAddonController() : base("NamePlate") {
        OnPostEnable += RefreshAddon;
        OnPostDisable += RefreshAddon;
        base.OnUpdate += UpdateNamePlate;
    }

    private void UpdateNamePlate(AddonNamePlate* addon) {
        DalamudInterface.Instance.Framework.RunOnTick(() => {
            OnInnerUpdate?.Invoke(addon);
        }, delayTicks: 1);
    }

    private static void RefreshAddon(AddonNamePlate* addon) {
        if (addon is not null) {
            if (addon->UldManager.LoadedState is AtkLoadState.Loaded) {
                addon->UldManager.UpdateDrawNodeList();
            }

            addon->UpdateCollisionNodeList(false);

            addon->DoFullUpdate = 1;
            NamePlateNumberArray.Instance()->DoFullUpdate = true;
        }
    }

    private event AddonControllerEvent? OnInnerUpdate;

    public override event AddonControllerEvent? OnUpdate {
        add => OnInnerUpdate += value;
        remove => base.OnUpdate -= value; // This is fine, it'll call original which will throw
    }
}
