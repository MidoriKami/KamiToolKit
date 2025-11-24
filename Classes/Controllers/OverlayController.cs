using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using KamiToolKit.Nodes;

namespace KamiToolKit.Classes.Controllers;

internal class OverlayAddon : NativeAddon;

public abstract class OverlayNode : SimpleOverlayNode {

    public abstract OverlayLayer OverlayLayer { get; }

    public virtual void Update() { }
}

public unsafe class OverlayController : IDisposable {

    private readonly Dictionary<OverlayLayer, List<OverlayNode>> overlayNodes = [];
    private readonly Dictionary<OverlayLayer, Pointer<AtkUnitBase>> overlayAddons = [];

    private bool overlaysActive;
    
    public OverlayController() {
        DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "NamePlate", (_,_) => AddOverlays());
        DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, "NamePlate",  (_,_) => RemoveOverlays());
        
        var addon = RaptureAtkUnitManager.Instance()->GetAddonByName("NamePlate");
        if (addon is not null) {
            AddOverlays();
        }
    }

    private void AddOverlays() {
        foreach (var overlayLayer in Enum.GetValues<OverlayLayer>()) {
            var addonName = overlayLayer.GetDescription();

            DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, addonName, OnOverlayAddonSetup);
            DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PreUpdate, addonName, OnOverlayAddonUpdate);
            DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, addonName, OnOverlayAddonFinalize);

            var addon = RaptureAtkUnitManager.Instance()->GetAddonByName(addonName);
            if (addon is not null) {
                overlayAddons.TryAdd(overlayLayer, addon);
            }
            else {
                CreateOverlayAddon(overlayLayer).Open();
            }
        }

        overlaysActive = true;
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

    public void AddNode(OverlayNode node) => DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {         
        overlayNodes.TryAdd(node.OverlayLayer, []);

        if (!overlayNodes[node.OverlayLayer].Contains(node)) {
            overlayNodes[node.OverlayLayer].Add(node);

            if (overlaysActive && overlayAddons.TryGetValue(node.OverlayLayer, out var addon)) {
                node.NodeId = (uint)addon.Value->UldManager.NodeListCount + 1;
                node.AttachNode(addon);
            }
        }
    });

    public void CreateNode(Func<OverlayNode> creationFunction) => DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {         
        var newNode = creationFunction();
        AddNode(newNode);
    });

    public void RemoveNode(OverlayNode node) => DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
        if (overlayNodes.TryGetValue(node.OverlayLayer, out var list)) {
            if (overlaysActive && list.Remove(node)) {
                node.DetachNode();
            }
        }
    });

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

    private static OverlayAddon CreateOverlayAddon(OverlayLayer layer) => new() {
        Title = layer.GetDescription(),
        InternalName = layer.GetDescription(),
        Size = AtkStage.Instance()->ScreenSize,
        LastClosePosition = Vector2.Zero,
        DepthLayer = layer.GetOverlayLayer(),
        IsOverlayAddon = true,
    };

    public void Dispose() {
        overlaysActive = false;

        DalamudInterface.Instance.AddonLifecycle.UnregisterListener(AddonEvent.PostSetup, "NamePlate");
        DalamudInterface.Instance.AddonLifecycle.UnregisterListener(AddonEvent.PreFinalize, "NamePlate");
        DalamudInterface.Instance.AddonLifecycle.UnregisterListener(OnOverlayAddonFinalize, OnOverlayAddonSetup, OnOverlayAddonUpdate);

        foreach (var node in overlayNodes.SelectMany(nodeList => nodeList.Value)) {
            node.Dispose();
        }

        overlayNodes.Clear();
        overlayAddons.Clear();
    }
}
