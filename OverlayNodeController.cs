using System.Collections.Generic;
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
    
    /// <summary>
    /// Returns a newly allocated overlay node, that will automatically be attached and reattached to the native ui.
    /// It is recommended to only have one Overlay Node per plugin, or per major feature set. Try not to pollute the NameplateAddon's node space.
    ///
    /// You can attach sub nodes to the overlay node if you need several subcategories of overlays.
    /// </summary>
    /// <param name="nodeId">String name for your overlay node, used for tracking and deduplication purposes.</param>
    /// <returns>A newly allocated overlay node, that may or may not be attached. It will be attached and unattached as NamePlate loads and unloads.</returns>
    public static SimpleOverlayNode GetOverlayNode(string nodeId) {
        instance ??= new NameplateAddonController();

        if (!OverlayNodes.TryGetValue(nodeId, out var value)) {
            var newOverlayNode = new SimpleOverlayNode {
                NodeId = NodeBase.NodeIdBase + (ushort)nodeId.GetHashCode(),
                IsVisible = true,
            };

            instance.OnAttach += addon => {
                newOverlayNode.AttachNode(addon->RootNode);
            };

            instance.OnDetach += _ => {
                newOverlayNode.DetachNode();
            };

            value = newOverlayNode;
            OverlayNodes.Add(nodeId, value);
        }
        
        return value;
    }
}
