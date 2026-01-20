using System;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Enums;

namespace KamiToolKit.Overlay;

public unsafe partial class OverlayController {

    private void AddOverlays() {
        foreach (var overlayLayer in Enum.GetValues<OverlayLayer>()) {
            var addonName = overlayLayer.GetDescription();

            DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, addonName, OnOverlayAddonSetup);
            DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PreUpdate, addonName, OnOverlayAddonUpdate);
            DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, addonName, OnOverlayAddonFinalize);

            // Temporary until https://github.com/goatcorp/Dalamud/pull/2584 is merged
            DalamudInterface.Instance.Framework.RunOnTick(() => {
                var addon = RaptureAtkUnitManager.Instance()->GetAddonByName(addonName);
                if (addon is not null) {
                    overlayAddons.TryAdd(overlayLayer, addon);
                    AttachNodes(overlayLayer);
                }
                else {
                    CreateOverlayAddon(overlayLayer).Open();
                }
            }, delayTicks: 2);
        }
        
        DalamudInterface.Instance.Framework.RunOnTick(() => {
            overlaysActive = true;
        }, delayTicks: 3);
    }

    private void RemoveOverlays() {
        DalamudInterface.Instance.AddonLifecycle.UnregisterListener(OnOverlayAddonFinalize, OnOverlayAddonSetup, OnOverlayAddonUpdate);

        foreach (var overlayLayer in Enum.GetValues<OverlayLayer>()) {
            if (overlayNodes.TryGetValue(overlayLayer, out var list)) {
                foreach (var node in list) {
                    node.DetachNode();
                }
            }
        }

        overlayAddons.Clear();
        overlaysActive = false;
    }
    
    private static OverlayAddon CreateOverlayAddon(OverlayLayer layer) => new() {
        Title = layer.GetDescription(),
        InternalName = layer.GetDescription(),
        DepthLayer = layer.GetOverlayLayer(),
        IsOverlayAddon = true,
    };

    private void OnOverlayAddonSetup(AddonEvent type, AddonArgs args) {
        var addon = (AtkUnitBase*)args.Addon.Address;
        var overlayLayer = addon->DepthLayer.GetOverlayLayer();

        overlayAddons.TryAdd(overlayLayer, addon);
        AttachNodes(overlayLayer);
    }

    private void OnOverlayAddonUpdate(AddonEvent type, AddonArgs args) {
        var addon = (AtkUnitBase*)args.Addon.Address;
        var overlayLayer = addon->DepthLayer.GetOverlayLayer();

        if (overlayNodes.TryGetValue(overlayLayer, out var list)) {
            foreach (var node in list) {
                node.Update();
            }
        }
    }

    private void OnOverlayAddonFinalize(AddonEvent type, AddonArgs args) {
        var addon = (AtkUnitBase*)args.Addon.Address;
        var overlayLayer = addon->DepthLayer.GetOverlayLayer();
        
        overlayAddons.Remove(overlayLayer);
        DetachNodes(overlayLayer);
    }

    private void AttachNodes(OverlayLayer layer) {
        if (!overlayAddons.TryGetValue(layer, out var addon)) return;
        if (!overlayNodes.TryGetValue(layer, out var list)) return;

        foreach (var node in list) {
            node.NodeId = (uint)addon.Value->UldManager.NodeListCount + 1;
            node.AttachNode(addon);
        }
    }

    private void DetachNodes(OverlayLayer layer) {
        if (!overlayNodes.TryGetValue(layer, out var list)) return;

        foreach (var node in list) {
            node.DetachNode();
        }
    }
}
