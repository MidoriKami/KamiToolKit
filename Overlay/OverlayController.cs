using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Addon.Lifecycle;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using KamiToolKit.Classes;

namespace KamiToolKit.Overlay;

public unsafe partial class OverlayController : IDisposable {

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

    public void RemoveAllNodes() => DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
        foreach (var node in overlayNodes.SelectMany(set => set.Value).ToList()) {
            RemoveNode(node);
        }
    });
    
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
