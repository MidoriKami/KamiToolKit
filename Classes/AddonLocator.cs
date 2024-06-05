using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Classes;

public static unsafe class AddonLocator {
    /// <summary>
    /// Searches all loaded addons and all nodes contained within to find the addon that owns the targeted node.
    /// </summary>
    /// <param name="node">The node to search for</param>
    /// <returns>AtkUnitBase* to the addon </returns>
    public static AtkUnitBase* GetAddonForNode(AtkResNode* node) {
        foreach (ref readonly var unit in AtkStage.Instance()->RaptureAtkUnitManager->AllLoadedUnitsList.Entries) {
            if (unit.Value is null) continue;
            if (!unit.Value->IsReady) continue;
            if (unit.Value->UldManager.LoadedState is not AtkLoadState.Loaded) continue;
            
            if (AddonContainsNode(ref unit.Value->UldManager, node)) {
                return unit.Value;
            }
        }

        return null;
    }

    private static bool AddonContainsNode(ref AtkUldManager uldManager, AtkResNode* targetNode) {
        var nodeListCount = uldManager.NodeListCount;
        
        for (var index = 0; index < nodeListCount; ++index) {
            ref var currentNode = ref uldManager.NodeList[index];
            if (targetNode == currentNode) return true;

            if ((uint) currentNode->Type > 1000u) {
                var componentNode = ((AtkComponentNode*) currentNode)->Component;
                if (componentNode is not null) {
                    if (componentNode->UldManager.LoadedState is not AtkLoadState.Loaded) continue;
                    if (AddonContainsNode(ref componentNode->UldManager, targetNode)) {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}