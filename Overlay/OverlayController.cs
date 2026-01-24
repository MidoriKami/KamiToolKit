using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Enums;

namespace KamiToolKit.Overlay;

public unsafe class OverlayController : IDisposable {
    private readonly Dictionary<OverlayLayer, List<OverlayNode>> overlayNodes = [];
    
    public OverlayController() {
        DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "NamePlate", (_,_) => AddOverlays());
        DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, "NamePlate",  (_,_) => RemoveOverlays());

        // Register Listeners Immediately
        foreach (var overlayLayer in Enum.GetValues<OverlayLayer>()) {
            var addonName = overlayLayer.Description;

            DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, addonName, OnOverlayAddonSetup);
            DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PreUpdate, addonName, OnOverlayAddonUpdate);
            DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, addonName, OnOverlayAddonFinalize);
        }

        // Ensure multiple instance of this controller aren't 
        DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
            if (RaptureAtkUnitManager.Instance()->GetAddonByName("NamePlate") is not null) {
                DalamudInterface.Instance.Framework.RunOnTick(AddOverlays, delayTicks: 3);
            }
        });
    }

    public void AddNode(OverlayNode node) => DalamudInterface.Instance.Framework.RunOnTick(() => {         
        overlayNodes.TryAdd(node.OverlayLayer, []);

        if (!overlayNodes[node.OverlayLayer].Contains(node)) {
            overlayNodes[node.OverlayLayer].Add(node);

            var addon = RaptureAtkUnitManager.Instance()->GetAddonByName(node.OverlayLayer.Description);
            if (addon is not null) {
                node.NodeId = (uint)addon->UldManager.NodeListCount + 1;
                node.AttachNode(addon);
            }
        }
    }, delayTicks: 3);

    public void CreateNode(Func<OverlayNode> creationFunction) => DalamudInterface.Instance.Framework.RunOnTick(() => {         
        var newNode = creationFunction();
        AddNode(newNode);
    }, delayTicks: 3);

    public void RemoveNode(OverlayNode node) => DalamudInterface.Instance.Framework.RunOnTick(() => {
        if (overlayNodes.TryGetValue(node.OverlayLayer, out var list)) {
            if (list.Remove(node)) {
                node.Dispose();
            }
        }
    }, delayTicks: 3);

    public void RemoveAllNodes() => DalamudInterface.Instance.Framework.RunOnTick(() => {
        foreach (var node in overlayNodes.SelectMany(set => set.Value).ToList()) {
            RemoveNode(node);
        }
    }, delayTicks: 3);
    
    public void Dispose() {
        DalamudInterface.Instance.AddonLifecycle.UnregisterListener(AddonEvent.PostSetup, "NamePlate");
        DalamudInterface.Instance.AddonLifecycle.UnregisterListener(AddonEvent.PreFinalize, "NamePlate");
        DalamudInterface.Instance.AddonLifecycle.UnregisterListener(OnOverlayAddonFinalize, OnOverlayAddonSetup, OnOverlayAddonUpdate);

        foreach (var node in overlayNodes.SelectMany(nodeList => nodeList.Value)) {
            node.Dispose();
        }

        overlayNodes.Clear();
    }
    
    private void AddOverlays() {
        foreach (var overlayLayer in Enum.GetValues<OverlayLayer>()) {
            var addon = RaptureAtkUnitManager.Instance()->GetAddonByName(overlayLayer.Description);
            if (addon is null) {
                CreateOverlayAddon(overlayLayer).Open();
            }
        }
    }

    private void RemoveOverlays() {
        foreach (var overlayLayer in Enum.GetValues<OverlayLayer>()) {
            if (overlayNodes.TryGetValue(overlayLayer, out var list)) {
                foreach (var node in list) {
                    node.DetachNode();
                }
            }
        }
    }
    
    private static OverlayAddon CreateOverlayAddon(OverlayLayer layer) => new() {
        Title = layer.Description,
        InternalName = layer.Description,
        DepthLayer = layer.DepthLayer,
        IsOverlayAddon = true,
    };

    private void OnOverlayAddonSetup(AddonEvent type, AddonArgs args) {
        var addon = (AtkUnitBase*)args.Addon.Address;
        var overlayLayer = addon->DepthLayer.GetOverlayLayer();

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

        DetachNodes(overlayLayer);
    }

    private void AttachNodes(OverlayLayer layer) {
        if (!overlayNodes.TryGetValue(layer, out var list)) return;

        var addon = RaptureAtkUnitManager.Instance()->GetAddonByName(layer.Description);
        if (addon is not null) {
            foreach (var node in list) {
                node.NodeId = (uint)addon->UldManager.NodeListCount + 1;
                node.AttachNode(addon);
            }
        }
    }

    private void DetachNodes(OverlayLayer layer) {
        if (!overlayNodes.TryGetValue(layer, out var list)) return;

        foreach (var node in list) {
            node.DetachNode();
        }
    }
}
