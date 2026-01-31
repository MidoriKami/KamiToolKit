using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Enums;

namespace KamiToolKit.Overlay;

public unsafe class OverlayController : IDisposable {
    private readonly Dictionary<OverlayLayer, List<OverlayNode>> overlayNodes = [];
    private readonly Dictionary<OverlayLayer, OverlayAddonState> addonState = [];

    private ControllerState controllerState = ControllerState.WaitForNameplate;

    public OverlayController() {
        ClearState();

        DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, "NamePlate", OnNamePlatePreFinalize);

        foreach (var overlayLayer in Enum.GetValues<OverlayLayer>()) {
            var addonName = overlayLayer.Description;

            DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PreUpdate, addonName, OnOverlayAddonUpdate);
            DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, addonName, OnOverlayAddonFinalize);
        }

        BeginStateCheck();
    }

    public void Dispose() {
        DalamudInterface.Instance.AddonLifecycle.UnregisterListener(AddonEvent.PreFinalize, "NamePlate");
        DalamudInterface.Instance.AddonLifecycle.UnregisterListener(OnOverlayAddonFinalize, OnOverlayAddonUpdate);

        foreach (var node in overlayNodes.SelectMany(nodeList => nodeList.Value)) {
            node.Dispose();
        }

        overlayNodes.Clear();
    }

    //
    // State management (framework thread)
    //

    private void ClearState() {
        controllerState = ControllerState.WaitForNameplate;

        foreach (var overlayLayer in Enum.GetValues<OverlayLayer>()) {
            addonState[overlayLayer] = OverlayAddonState.None;
        }
    }

    private void BeginStateCheck() {
        DalamudInterface.Instance.Framework.Update -= CheckOverlayState;
        DalamudInterface.Instance.Framework.Update += CheckOverlayState;
    }

    private void CheckOverlayState(IFramework framework) {
        if (controllerState == ControllerState.WaitForNameplate) {
            CheckNameplateReady();
        }
        else if (controllerState == ControllerState.WaitForReady) {
            CheckOverlayAddonsReady();
        }
        else if (controllerState == ControllerState.Ready) {
            DalamudInterface.Instance.Framework.Update -= CheckOverlayState;
        }
    }

    private void CheckNameplateReady() {
        var nameplate = RaptureAtkUnitManager.Instance()->GetAddonByName("NamePlate");
        if (nameplate is null || !nameplate->IsReady)
            return;

        foreach (var overlayLayer in Enum.GetValues<OverlayLayer>()) {
            var addon = RaptureAtkUnitManager.Instance()->GetAddonByName(overlayLayer.Description);
            if (addon is null) {
                if (addonState[overlayLayer] == OverlayAddonState.None) {
                    addonState[overlayLayer] = OverlayAddonState.WaitForReady;
                    CreateOverlayAddon(overlayLayer).Open();
                }
            }
            else {
                addonState[overlayLayer] = OverlayAddonState.WaitForReady;
            }
        }

        controllerState = ControllerState.WaitForReady;
    }

    private void CheckOverlayAddonsReady() {
        var totalAddons = Enum.GetValues<OverlayLayer>().Length;
        var totalAddonsReady = 0;

        foreach (var overlayLayer in Enum.GetValues<OverlayLayer>()) {
            var addon = RaptureAtkUnitManager.Instance()->GetAddonByName(overlayLayer.Description);
            if (addon is not null && addon->IsReady) {
                var state = addonState[overlayLayer];
                if (state == OverlayAddonState.WaitForReady) {
                    AttachAllNodes(overlayLayer);
                    addonState[overlayLayer] = OverlayAddonState.Ready;
                }
                totalAddonsReady++;
            }
        }

        if (totalAddonsReady == totalAddons)
            controllerState = ControllerState.Ready;
    }

    private void AttachAllNodes(OverlayLayer layer) {
        if (!overlayNodes.TryGetValue(layer, out var list)) return;

        var addon = RaptureAtkUnitManager.Instance()->GetAddonByName(layer.Description);
        if (addon is not null) {
            foreach (var node in list) {
                AttachNode(addon, node);
            }
        }
    }

    //
    // Public node access
    //

    public void CreateNode(Func<OverlayNode> creationFunction) {
        AddNode(creationFunction());
    }

    public void AddNode(OverlayNode node) => DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
        overlayNodes.TryAdd(node.OverlayLayer, []);

        if (!overlayNodes[node.OverlayLayer].Contains(node)) {
            overlayNodes[node.OverlayLayer].Add(node);

            if (addonState[node.OverlayLayer] == OverlayAddonState.Ready) {
                var addon = RaptureAtkUnitManager.Instance()->GetAddonByName(node.OverlayLayer.Description);
                if (addon is not null) {
                    AttachNode(addon, node);
                }
            }
        }
    });

    public void RemoveNode(OverlayNode node) => DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
        if (overlayNodes.TryGetValue(node.OverlayLayer, out var list)) {
            if (list.Remove(node)) {
                node.Dispose();
            }
        }
    });

    public void RemoveAllNodes() => DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
        foreach (var node in overlayNodes.SelectMany(set => set.Value).ToList()) {
            RemoveNode(node);
        }
    });

    //
    // Events
    //

    private void OnNamePlatePreFinalize(AddonEvent type, AddonArgs args) {
        ClearState();

        foreach (var overlayLayer in Enum.GetValues<OverlayLayer>()) {
            if (overlayNodes.TryGetValue(overlayLayer, out var list)) {
                foreach (var node in list) {
                    node.DetachNode();
                }
            }
        }

        BeginStateCheck();
    }

    private void OnOverlayAddonFinalize(AddonEvent type, AddonArgs args) {
        var addon = (AtkUnitBase*)args.Addon.Address;
        var overlayLayer = addon->DepthLayer.GetOverlayLayer();

        if (overlayNodes.TryGetValue(overlayLayer, out var list)) {
            foreach (var node in list) {
                node.DetachNode();
            }
        }

        addonState[overlayLayer] = OverlayAddonState.None;
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

    //
    // Helpers
    //

    private static OverlayAddon CreateOverlayAddon(OverlayLayer layer) {
        return new OverlayAddon {
            Title = layer.Description,
            InternalName = layer.Description,
            DepthLayer = layer.DepthLayer,
            IsOverlayAddon = true,
        };
    }

    private static void AttachNode(AtkUnitBase* addon, OverlayNode node) {
        node.NodeId = (uint)addon->UldManager.NodeListCount + 1;
        node.AttachNode(addon);
    }
}
