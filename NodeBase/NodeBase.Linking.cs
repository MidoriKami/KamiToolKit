using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Dalamud;
using KamiToolKit.Nodes;

namespace KamiToolKit;

public abstract unsafe partial class NodeBase {

    internal readonly List<NodeBase> ChildNodes = [];
    private NodeBase? parentNode;

    private readonly HashSet<string> addonsPendingUpdate = [];
    private readonly HashSet<nint> uldManagersPendingUpdate = [];

    internal AtkUldManager* ParentUldManager { get; set; }
    internal AtkUnitBase* ParentAddon { get; private set; }

    [OverloadResolutionPriority(1)]
    public void AttachNode(NativeAddon? targetAddon, NodePosition targetPosition = NodePosition.AsLastChild)
        => PerformManagedAttach(targetAddon, targetPosition);

    public void AttachNode(AtkUnitBase* targetAddon, NodePosition targetPosition = NodePosition.AsLastChild)
        => PerformNativeAttach(targetAddon is not null ? targetAddon->RootNode : null, targetPosition);

    [OverloadResolutionPriority(1)]
    public void AttachNode(NodeBase? targetNode, NodePosition targetPosition = NodePosition.AsLastChild)
        => PerformManagedAttach(targetNode, targetPosition);

    public void AttachNode(AtkResNode* targetNode, NodePosition targetPosition = NodePosition.AsLastChild)
        => PerformNativeAttach(targetNode, targetPosition);

    public void AttachNode(AtkImageNode* targetNode, NodePosition targetPosition = NodePosition.AsLastChild)
        => PerformNativeAttach((AtkResNode*)targetNode, targetPosition);

    public void AttachNode(AtkTextNode* targetNode, NodePosition targetPosition = NodePosition.AsLastChild)
        => PerformNativeAttach((AtkResNode*)targetNode, targetPosition);

    public void AttachNode(AtkNineGridNode* targetNode, NodePosition targetPosition = NodePosition.AsLastChild)
        => PerformNativeAttach((AtkResNode*)targetNode, targetPosition);

    public void AttachNode(AtkCounterNode* targetNode, NodePosition targetPosition = NodePosition.AsLastChild)
        => PerformNativeAttach((AtkResNode*)targetNode, targetPosition);

    public void AttachNode(AtkCollisionNode* targetNode, NodePosition targetPosition = NodePosition.AsLastChild)
        => PerformNativeAttach((AtkResNode*)targetNode, targetPosition);

    public void AttachNode(AtkClippingMaskNode* targetNode, NodePosition targetPosition = NodePosition.AsLastChild)
        => PerformNativeAttach((AtkResNode*)targetNode, targetPosition);

    public void AttachNode(AtkComponentNode* targetNode, NodePosition targetPosition = NodePosition.AfterAllSiblings)
        => PerformNativeAttach((AtkResNode*)targetNode, targetPosition);

    private void PerformManagedAttach(NativeAddon? targetAddon, NodePosition targetPosition = NodePosition.AsLastChild) {
        if (MainThreadSafety.TryAssertMainThread()) return;
        if (targetAddon is null) return;

        // Check the Addon's node list to find out what NodeId we should be, and set that before attaching
        if (NodeId > NodeIdBase) {
            NodeId = targetAddon.InternalAddon->UldManager.GetMaxNodeId() + 1;
        }

        PerformNativeAttach(targetAddon.RootNode, targetPosition);

        parentNode = targetAddon.RootNode;
        parentNode.ChildNodes.Add(this);
    }

    private void PerformManagedAttach(NodeBase? targetNode, NodePosition targetPosition) {
        if (MainThreadSafety.TryAssertMainThread()) return;
        if (targetNode is null) return;

        PerformNativeAttach(targetNode, targetPosition);

        parentNode = targetNode;
        parentNode.ChildNodes.Add(this);
    }

    private void PerformNativeAttach(AtkResNode* targetNode, NodePosition targetPosition) {
        if (MainThreadSafety.TryAssertMainThread()) return;
        if (targetNode is null) return;

        if (targetNode->GetNodeType() is NodeType.Component) {

            // If target is a ComponentNode,
            // then we don't ever wanna be a child of the ComponentNode itself,
            // we will want to be a sibling of the root node.
            // Therefore, redirect the target position to be siblings.
            targetPosition = targetPosition switch {
                NodePosition.AsLastChild => NodePosition.AfterAllSiblings,
                NodePosition.AsFirstChild => NodePosition.BeforeAllSiblings,
                _ => targetPosition,
            };

            // If however, we are using BeforeTarget or AfterTarget,
            // then we do want to attach to the ComponentNode
            // else, attach to its root node.
            var componentNode = targetNode->GetAsAtkComponentNode();
            if (componentNode is not null) {
                targetNode = targetPosition switch {
                    NodePosition.AfterTarget => targetNode,
                    NodePosition.BeforeTarget => targetNode,
                    NodePosition.AfterAllSiblings => componentNode->Component->UldManager.RootNode,
                    NodePosition.BeforeAllSiblings => componentNode->Component->UldManager.RootNode,
                    _ => throw new ArgumentOutOfRangeException(nameof(targetPosition), targetPosition, null),
                };

                // If we aren't inserting as this elements child, then we need to find the parent UldManager and use that instead
                if (targetPosition is NodePosition.AfterTarget or NodePosition.BeforeTarget) {
                    if (NodeId > NodeIdBase) {
                        var targetsParentUldManager = GetUldManagerForNode(targetNode);

                        // Failed to get uldManager from node tree
                        if (targetsParentUldManager is null) {

                            // Try to get parent addon from target node
                            var parentAddon = RaptureAtkUnitManager.Instance()->GetAddonByNode(targetNode);
                            if (parentAddon is not null) {
                                targetsParentUldManager = &parentAddon->UldManager;
                            }
                        }

                        if (targetsParentUldManager is not null) {
                            NodeId = targetsParentUldManager->GetMaxNodeId() + 1;
                        }
                    }
                }
                else {
                    // We also need to check the components node list, to get a safely assigned nodeId
                    if (NodeId > NodeIdBase) {
                        NodeId = componentNode->Component->UldManager.GetMaxNodeId() + 1;
                    }
                }
            }
        }

        NodeLinker.AttachNode(this, targetNode, targetPosition);
        UpdateParentAddon(targetNode);
        UpdateNative();
    }

    internal void ReattachNode(AtkResNode* newTarget) {
        if (newTarget is null) return;

        DetachNode();
        AttachNode(newTarget);
    }

    public void DetachNode() {
        if (MainThreadSafety.TryAssertMainThread()) return;
        if (ResNode is null) return;

        UnlinkFromNative();
        RemoveUldManagerObjectReferences();
        RemoveParentAddonReferences();
        RemoveParentNodeReferences();
    }

    private void UnlinkFromNative() {
        NodeLinker.DetachNode(ResNode);
        ResNode->ParentNode = null;
        ResNode->NextSiblingNode = null;
        ResNode->PrevSiblingNode = null;
    }

    private void RemoveUldManagerObjectReferences() {
        if (ParentUldManager is null) return;

        ParentUldManager->RemoveNodeFromObjectList(this);
        ParentUldManager = null;
    }

    private void RemoveParentAddonReferences() {
        if (ParentAddon is null) return;

        var addonName = ParentAddon->NameString;

        // Queue collision update for next frame
        if (addonsPendingUpdate.Add(addonName)) {
            Services.Framework.RunOnTick(() => {
                var currentInstance = RaptureAtkUnitManager.Instance()->GetAddonByName(addonName);
                if (currentInstance is not null) {
                    currentInstance->UldManager.UpdateDrawNodeList();
                    currentInstance->UpdateCollisionNodeList(false);
                }

                addonsPendingUpdate.Remove(addonName);
            });
        }

        ParentAddon = null;

        foreach (var child in GetAllChildren(this)) {
            child.ParentAddon = null;
        }
    }

    private void RemoveParentNodeReferences() {
        if (parentNode is null) return;

        parentNode.ChildNodes.Remove(this);
        parentNode = null;
    }

    private void UpdateNative() {
        if (ResNode is null) return;

        MarkDirty();

        if (ParentUldManager is null) {
            ParentUldManager = GetUldManagerForNode(ResNode);
        }

        if (ParentUldManager is not null) {
            // Queue UldManager update for next frame
            var manager = ParentUldManager;

            if (uldManagersPendingUpdate.Add((nint)ParentUldManager)) {
                Services.Framework.RunOnTick(() => {
                    if (ResNode is null) return;

                    manager->AddNodeToObjectList(this);
                    manager->SetupText();
                    uldManagersPendingUpdate.Remove((nint)manager);
                });
            }

            ParentUldManager->AddNodeToObjectList(this);
        }

        if (ParentAddon is not null) {
            if (ParentAddon->NameString is "NamePlate") {
                Services.Log.Warning("Warning, attaching to AddonNamePlate is not supported. Use OverlayController instead.");
            }

            var addonName = ParentAddon->NameString;

            // Queue collision update for next frame
            if (addonsPendingUpdate.Add(addonName)) {
                Services.Framework.RunOnTick(() => {
                    var currentInstance = RaptureAtkUnitManager.Instance()->GetAddonByName(addonName);
                    if (currentInstance is not null) {
                        currentInstance->UldManager.UpdateDrawNodeList();
                        currentInstance->UpdateCollisionNodeList(false);
                    }

                    addonsPendingUpdate.Remove(addonName);
                });
            }
        }
    }

    private void UpdateParentAddon(AtkResNode* node) {
        if (parentNode is not null && parentNode.ParentAddon is not null) {
            ParentAddon = parentNode.ParentAddon;
        }
        else if (ParentAddon is null) {
            var targetParentAddon = RaptureAtkUnitManager.Instance()->GetAddonByNode(node);
            if (targetParentAddon is not null) {
                ParentAddon = targetParentAddon;
            }
        }

        if (ParentAddon is not null) {
            foreach (var child in GetAllChildren(this)) {
                child.ParentAddon = ParentAddon;
            }
        }
    }

    private AtkUldManager* GetUldManagerForNode(AtkResNode* node) {
        if (node is null) return null;

        var targetNode = node;

        if (targetNode->GetNodeType() is NodeType.Component) {
            targetNode = targetNode->ParentNode;
        }

        // Try to get UldManager via the first parent that is a component
        while (targetNode is not null) {
            if (targetNode->GetNodeType() is NodeType.Component) {
                var componentNode = (AtkComponentNode*)targetNode;
                return &componentNode->Component->UldManager;
            }

            targetNode = targetNode->ParentNode;
        }

        // We failed to find a parent component, try to get a parent addon instead
        if (ParentAddon is not null) {
            return &ParentAddon->UldManager;
        }

        return null;
    }

    private static IEnumerable<NodeBase> GetAllChildren(NodeBase parent) {
        foreach (var child in parent.ChildNodes) {
            yield return child;
            foreach (var childNode in GetAllChildren(child)) {
                yield return childNode;
            }
        }
    }

    internal static IEnumerable<NodeBase> GetLocalChildren(NodeBase parent) {
        if (parent is ComponentNode) yield break;

        foreach (var child in parent.ChildNodes) {
            yield return child;

            if (child is ComponentNode) continue;
            foreach (var childNode in GetLocalChildren(child)) {
                yield return childNode;
            }
        }
    }
}
