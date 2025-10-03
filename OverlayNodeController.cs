using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.System;

namespace KamiToolKit;

public static unsafe class OverlayNodeController {
    private static NameplateAddonController? instance;
    private static readonly Dictionary<string, SimpleOverlayNode> OverlayNodes = [];

    public static void Dispose() {
        instance?.Dispose();
        instance = null;

        foreach (var node in OverlayNodes.Values) {
            node.Dispose();
        }
        OverlayNodes.Clear();
    }

    public static Action? OnOverlayUpdated { get; set; }

    /// <summary>
    /// Returns a newly allocated overlay node, that will automatically be attached and reattached to the native ui.
    /// It is recommended to only have one Overlay Node per plugin, or per major feature set. Try not to pollute the NameplateAddon's node space.
    ///
    /// You can attach sub nodes to the overlay node if you need several subcategories of overlays.
    /// </summary>
    /// <remarks>
    /// Overlay Controller takes ownership of the created node, do not attempt to dispose or detach the node.
    /// Instead, be sure to call RemoveOverlayNode when you are done with it.
    /// </remarks>
    /// <param name="nodeId">String name for your overlay node, used for tracking and deduplication purposes.</param>
    /// <returns>A newly allocated overlay node, that may or may not be attached. It will be attached and unattached as NamePlate loads and unloads.</returns>
    public static SimpleOverlayNode GetOverlayNode(string nodeId) {
        ThreadSafety.AssertMainThread("Getting overlay nodes while not on the main thread is not supported.");
        
        if (instance is null) {
            instance = new NameplateAddonController();
            instance.OnAttach += OnNamePlateAttach;
            instance.OnDetach += OnNamePlateDetach;
            instance.OnUpdate += _ => OnOverlayUpdated?.Invoke();
            instance.Enable();
        }

        if (!OverlayNodes.TryGetValue(nodeId, out var value)) {
            value = new SimpleOverlayNode {
                NodeId = NodeBase.NodeIdBase + (ushort)nodeId.GetHashCode(),
                Size = new Vector2(AtkStage.Instance()->ScreenSize.Width, AtkStage.Instance()->ScreenSize.Height),
                IsVisible = true,
            };

            OverlayNodes.Add(nodeId, value);

            var nameplateAddon = RaptureAtkUnitManager.Instance()->GetAddonByName("NamePlate");
            if (nameplateAddon is not null) {
                value.AttachNode(nameplateAddon->RootNode, NodePosition.AsFirstChild);
            }
        }
    
        return value;
    }

    public static void RemoveOverlayNode(string nodeId)
        => DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {

            if (OverlayNodes.TryGetValue(nodeId, out var value)) {
                value.DetachNode();
                value.Dispose();
                OverlayNodes.Remove(nodeId);
            }
        });

    private static void OnNamePlateAttach(AddonNamePlate* addon) {
        foreach (var node in OverlayNodes.Values) {
            node.AttachNode(addon->RootNode, NodePosition.AsFirstChild);
        }
    }

    private static void OnNamePlateDetach(AddonNamePlate* addon) {
        foreach (var node in OverlayNodes.Values) {
            node.DetachNode();
        }
    }
}
