using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Dalamud;
using KamiToolKit.Enums;

namespace KamiToolKit.Overlay.UiOverlay;

public unsafe class OverlayController : IDisposable {
    private readonly Dictionary<OverlayLayer, List<OverlayNode>> overlayNodes = [];
    private readonly Dictionary<OverlayLayer, OverlayAddonState> addonState = [];

    private bool isDisposed;
    private ControllerState controllerState = ControllerState.WaitForNameplate;

    public OverlayController() {
        ClearState();

        Services.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, "NamePlate", OnNamePlatePreFinalize);

        foreach (var overlayLayer in Enum.GetValues<OverlayLayer>()) {
            var addonName = overlayLayer.Description;

            Services.AddonLifecycle.RegisterListener(AddonEvent.PreUpdate, addonName, OnOverlayAddonUpdate);
            Services.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, addonName, OnOverlayAddonFinalize);
        }

        BeginStateCheck();
    }

    public void Dispose() {
        if (isDisposed) return;
        isDisposed = true;

        Services.Framework.Update -= CheckOverlayState;
        Services.AddonLifecycle.UnregisterListener(AddonEvent.PreFinalize, "NamePlate");
        Services.AddonLifecycle.UnregisterListener(OnOverlayAddonFinalize, OnOverlayAddonUpdate);

        foreach (var node in overlayNodes.SelectMany(nodeList => nodeList.Value).ToList()) {
            if (!node.IsDisposed) {
                node.Dispose();
            }
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
        if (isDisposed) return;
        Services.Framework.Update -= CheckOverlayState;
        Services.Framework.Update += CheckOverlayState;
    }

    private void CheckOverlayState(IFramework framework) {
        if (isDisposed) {
            Services.Framework.Update -= CheckOverlayState;
            return;
        }

        switch (controllerState) {
            case ControllerState.WaitForNameplate:
                CheckNameplateReady();
                break;

            case ControllerState.WaitForReady:
                CheckOverlayAddonsReady();
                break;

            case ControllerState.Ready:
                Services.Framework.Update -= CheckOverlayState;
                break;
        }
    }

    private void CheckNameplateReady() {
        var nameplate = RaptureAtkUnitManager.Instance()->GetAddonByName("NamePlate");
        if (nameplate is null) return;
        if (!nameplate->IsReady) return;

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
            if (addon is null) continue;
            if (!addon->IsReady) continue;

            if (addonState[overlayLayer] is OverlayAddonState.WaitForReady) {
                AttachAllNodes(overlayLayer);
                addonState[overlayLayer] = OverlayAddonState.Ready;
            }
            totalAddonsReady++;
        }

        if (totalAddonsReady == totalAddons) {
            controllerState = ControllerState.Ready;
        }
    }

    private void AttachAllNodes(OverlayLayer layer) {
        var addon = RaptureAtkUnitManager.Instance()->GetAddonByName(layer.Description);
        if (addon is null) return;

        foreach (var node in GetLiveNodes(layer)) {
            AttachNode(addon, node);
        }
    }

    //
    // Public node access
    //

    public void CreateNode(Func<OverlayNode> creationFunction) => Services.Framework.RunOnFrameworkThread(() => {
        AddNode(creationFunction());
    });

    public void AddNode(OverlayNode node) => Services.Framework.RunOnFrameworkThread(() => {
        if (isDisposed) return;
        if (node.IsDisposed) return;

        overlayNodes.TryAdd(node.OverlayLayer, []);

        if (overlayNodes[node.OverlayLayer].Contains(node)) return;

        overlayNodes[node.OverlayLayer].Add(node);

        if (addonState[node.OverlayLayer] is not OverlayAddonState.Ready) return;

        var addon = RaptureAtkUnitManager.Instance()->GetAddonByName(node.OverlayLayer.Description);
        if (addon is null) return;

        AttachNode(addon, node);
    });

    public void RemoveNode(OverlayNode node) => Services.Framework.RunOnFrameworkThread(() => {
        if (TryUntrackNode(node)) {
            if (!node.IsDisposed) {
                node.Dispose();
            }
        }
    });

    public void RemoveAllNodes() => Services.Framework.RunOnFrameworkThread(() => {
        foreach (var node in overlayNodes.SelectMany(set => set.Value).ToList()) {
            RemoveNode(node);
        }
    });

    //
    // Events
    //

    private void OnNamePlatePreFinalize(AddonEvent type, AddonArgs args) {
        if (isDisposed) return;

        ClearState();

        foreach (var overlayLayer in Enum.GetValues<OverlayLayer>()) {
            foreach (var node in GetLiveNodes(overlayLayer)) {
                node.DetachNode();
            }
        }

        BeginStateCheck();
    }

    private void OnOverlayAddonFinalize(AddonEvent type, AddonArgs args) {
        if (isDisposed) return;

        var addon = (AtkUnitBase*)args.Addon.Address;
        var overlayLayer = addon->DepthLayer.GetOverlayLayer();

        foreach (var node in GetLiveNodes(overlayLayer)) {
            node.DetachNode();
        }

        addonState[overlayLayer] = OverlayAddonState.None;
    }

    private void OnOverlayAddonUpdate(AddonEvent type, AddonArgs args) {
        if (isDisposed) return;

        var addon = (AtkUnitBase*)args.Addon.Address;
        var overlayLayer = addon->DepthLayer.GetOverlayLayer();

        if (addonState[overlayLayer] is not OverlayAddonState.Ready) return;

        foreach (var node in GetLiveNodes(overlayLayer)) {
            node.Update();
        }
    }

    private bool TryUntrackNode(OverlayNode node) {
        if (!overlayNodes.TryGetValue(node.OverlayLayer, out var list)) return false;

        return list.Remove(node);
    }

    private List<OverlayNode> GetLiveNodes(OverlayLayer layer) {
        if (!overlayNodes.TryGetValue(layer, out var list)) return [];

        list.RemoveAll(node => node.IsDisposed);
        return list.ToList();
    }

    //
    // Helpers
    //

    private static NativeAddon CreateOverlayAddon(OverlayLayer layer) => new() {
        Title = layer.Description,
        InternalName = layer.Description,
        DepthLayer = layer.DepthLayer,
        IsOverlayAddon = true,
    };

    private static void AttachNode(AtkUnitBase* addon, OverlayNode node) {
        node.NodeId = (uint)addon->UldManager.NodeListCount + 1;
        node.AttachNode(addon);
    }
}
