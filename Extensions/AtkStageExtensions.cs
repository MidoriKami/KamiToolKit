using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Extensions;

public static unsafe class AtkStageExtensions {
    public static void ClearNodeFocus(ref this AtkStage atkStage, AtkResNode* targetNode) {
        if (targetNode is null) return;
        
        foreach (ref var focusEntry in atkStage.AtkInputManager->FocusList) {

            // If this entry has no listener/addon, skip it
            if (focusEntry.AtkEventListener is null) continue;

            // If this entry has our target node
            if (focusEntry.AtkEventTarget == targetNode) {
                
                // Clear the entry
                focusEntry.AtkEventTarget = null;
                focusEntry.FocusParam = 0;
                
                // Clear the input managers focused node
                atkStage.AtkInputManager->FocusedNode = null;
                
                // Clear collision managers collision node
                atkStage.AtkCollisionManager->IntersectingCollisionNode = null;

                // Also remove this node from any additional focus nodes the addon might reference
                var addon = (AtkUnitBase*) focusEntry.AtkEventListener;
                foreach (ref var node in addon->AdditionalFocusableNodes) {
                    if (node.Value == targetNode) {
                        node = null;
                    }
                }
            }
        }
    }
}
