using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Enums;
using KamiToolKit.Internal.Classes;
using KamiToolKit.Nodes;

namespace KamiToolKit.BaseTypes;

public abstract unsafe partial class NodeBase {

    /// <summary>
    /// Attaches this node to targetAddon's root node using targetPosition as the relative positioning.
    /// </summary>
    [OverloadResolutionPriority(1)]
    public void AttachNode(NativeAddon? targetAddon, NodePosition targetPosition = NodePosition.AsLastChild)
        => PerformManagedAttach(targetAddon, targetPosition);

    /// <inheritdoc cref="AttachNode(NativeAddon?, NodePosition)"/>
    public void AttachNode(AtkUnitBase* targetAddon, NodePosition targetPosition = NodePosition.AsLastChild)
        => PerformNativeAttach(targetAddon is not null ? targetAddon->RootNode : null, targetPosition);

    /// <summary>
    /// Attaches this node to the targetNode node using targetPosition to determine where to insert the node relatively.
    /// </summary>
    [OverloadResolutionPriority(1)]
    public void AttachNode(NodeBase? targetNode, NodePosition targetPosition = NodePosition.AsLastChild)
        => PerformManagedAttach(targetNode, targetPosition);

    /// <inheritdoc cref="AttachNode(NodeBase?, NodePosition)"/>
    public void AttachNode(AtkResNode* targetNode, NodePosition targetPosition = NodePosition.AsLastChild)
        => PerformNativeAttach(targetNode, targetPosition);

    /// <inheritdoc cref="AttachNode(NodeBase?, NodePosition)"/>
    public void AttachNode(AtkImageNode* targetNode, NodePosition targetPosition = NodePosition.AsLastChild)
        => PerformNativeAttach((AtkResNode*)targetNode, targetPosition);

    /// <inheritdoc cref="AttachNode(NodeBase?, NodePosition)"/>
    public void AttachNode(AtkTextNode* targetNode, NodePosition targetPosition = NodePosition.AsLastChild)
        => PerformNativeAttach((AtkResNode*)targetNode, targetPosition);

    /// <inheritdoc cref="AttachNode(NodeBase?, NodePosition)"/>
    public void AttachNode(AtkNineGridNode* targetNode, NodePosition targetPosition = NodePosition.AsLastChild)
        => PerformNativeAttach((AtkResNode*)targetNode, targetPosition);

    /// <inheritdoc cref="AttachNode(NodeBase?, NodePosition)"/>
    public void AttachNode(AtkCounterNode* targetNode, NodePosition targetPosition = NodePosition.AsLastChild)
        => PerformNativeAttach((AtkResNode*)targetNode, targetPosition);

    /// <inheritdoc cref="AttachNode(NodeBase?, NodePosition)"/>
    public void AttachNode(AtkCollisionNode* targetNode, NodePosition targetPosition = NodePosition.AsLastChild)
        => PerformNativeAttach((AtkResNode*)targetNode, targetPosition);

    /// <inheritdoc cref="AttachNode(NodeBase?, NodePosition)"/>
    public void AttachNode(AtkClippingMaskNode* targetNode, NodePosition targetPosition = NodePosition.AsLastChild)
        => PerformNativeAttach((AtkResNode*)targetNode, targetPosition);

    /// <inheritdoc cref="AttachNode(NodeBase?, NodePosition)"/>
    public void AttachNode(AtkComponentNode* targetNode, NodePosition targetPosition = NodePosition.AfterAllSiblings)
        => PerformNativeAttach((AtkResNode*)targetNode, targetPosition);

    /// <summary>
    /// Detaches this node from the current tree, and removes native references to it.
    /// This is only intended to be used for very specific use cases.
    /// Generally speaking you probably want "Dispose" instead.
    /// </summary>
    /// <remarks>
    /// <em>Do not call this immediately before calling dispose!</em>
    /// </remarks>
    public void DetachNode() {
        ThreadSafety.AssertMainThread();
        if (ResNode is null) return;

        UnlinkFromNative();
        RemoveUldManagerObjectReferences();
        RemoveParentNodeReferences();
        RemoveParentAddonReferences();
    }

    private void PerformManagedAttach(NativeAddon? targetAddon, NodePosition targetPosition = NodePosition.AsLastChild) {
        ThreadSafety.AssertMainThread();
        if (targetAddon is null) return;

        PerformNativeAttach(targetAddon.RootNode, targetPosition);

        parentNode = targetAddon.RootNode;
        parentNode.ChildNodes.Add(this);

        // Reassign IDs starting from NodeIdBase for the addon's UldManager scope
        uint nextId = NodeIdBase;
        ReassignSubtreeIds(ref nextId);
    }

    private void PerformManagedAttach(NodeBase? targetNode, NodePosition targetPosition) {
        if (this == targetNode) {
            IPluginLog.Get().Warning("Attempted to attach self to self, attach was aborted.");
            return;
        }

        ThreadSafety.AssertMainThread();
        if (targetNode is null) return;

        // Guard against double-attach, double attaching will deadlock the game.
        var childNodeList = targetPosition switch {
            NodePosition.AsFirstChild or NodePosition.AsLastChild
                => targetNode.ChildNodes,

            NodePosition.BeforeAllSiblings or NodePosition.AfterAllSiblings or NodePosition.BeforeTarget or NodePosition.AfterTarget
                => targetNode.parentNode?.ChildNodes,

            _ => null,
        };

        if (childNodeList?.Any(childNode => childNode == this) ?? false) {
            IPluginLog.Get().Warning("Attempted to double-attach node to parent, attach was aborted.");
            return;
        }

        PerformNativeAttach(targetNode, targetPosition);

        parentNode = targetNode;
        parentNode.ChildNodes.Add(this);

        // Reassign IDs for the attached subtree: find the next available ID in the parent's UldManager scope
        // by scanning all siblings' IDs and their subtrees, then assign sequentially from there.
        ReassignSubtreeIdsFromParent();
    }

    private void PerformNativeAttach(AtkResNode* targetNode, NodePosition targetPosition) {
        if (ResNode == targetNode) {
            IPluginLog.Get().Warning("Attempted to attach self to self, attach was aborted.");
            return;
        }

        ThreadSafety.AssertMainThread();
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
            }
        }

        NodeLinker.AttachNode(this, targetNode, targetPosition);
        UpdateParentAddon(targetNode);
        UpdateNative();

        // Reassign IDs for native attach: if target is a game-managed node (ID < NodeIdBase),
        // start a fresh counter at NodeIdBase for this UldManager scope.
        uint targetNodeId = targetNode->NodeId;
        if (targetNodeId < NodeIdBase) {
            uint nextId = NodeIdBase;
            ReassignSubtreeIds(ref nextId);
        }
    }

    internal void ReattachNode(AtkResNode* newTarget) {
        if (ResNode == newTarget) {
            IPluginLog.Get().Warning("Attempted to attach self to self, attach was aborted.");
            return;
        }

        if (newTarget is null) return;

        DetachNode();
        AttachNode(newTarget);
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

        if (!suppressAddonUpdate) {
            ParentAddon->UldManager.UpdateDrawNodeList();
            ParentAddon->UpdateCollisionNodeList(false);
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

        // Set this node and all children to dirty to have the
        // game recalc their visible location.
        MarkDirty();

        if (ParentUldManager is null) {
            ParentUldManager = GetUldManagerForNode(this);
        }

        if (ParentUldManager is not null) {
            ParentUldManager->AddNodeToObjectList(this);

            foreach (var child in GetAllChildren(this)) {
                child.ParentUldManager = ParentUldManager;
            }

            if (this is TextNode { TextId: not 0 }) {
                ParentUldManager->SetupText();
            }
        }

        if (ParentAddon is not null) {
            if (ParentAddon->NameString is "NamePlate") {
                IPluginLog.Get().Warning("Warning, attaching to AddonNamePlate is not supported. Use OverlayController instead.");
            }

            ParentAddon->UldManager.UpdateDrawNodeList();
            ParentAddon->UpdateCollisionNodeList(false);
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

    internal readonly List<NodeBase> ChildNodes = [];
    private NodeBase? parentNode;
    private bool suppressAddonUpdate;

    internal AtkUldManager* ParentUldManager { get; set; }
    internal AtkUnitBase* ParentAddon { get; private set; }

    #region Deterministic ID Reassignment

    /// <summary>
    /// Recursively assigns deterministic IDs for this node and all its descendants.
    /// Runs only when attaching to the game (NativeAddon or game-managed AtkResNode).
    /// Each UldManager scope gets its own incrementing counter.
    /// ComponentNode boundaries start a new UldManager scope where counting starts from 1.
    /// Nodes with IDs below NodeIdBase (game-managed) are preserved; all others get incremental IDs.
    /// </summary>
    private static void AssignIds(NodeBase node, ref uint nextId) {
        // Skip nodes with hardcoded game-style IDs
        if (node.NodeId < NodeIdBase) return;

        // Assign the next sequential ID
        node.NodeId = nextId++;

        // Assign IDs to all children recursively
        AssignChildrenIds(node, ref nextId);
    }

    /// <summary>
    /// Recursively assigns IDs to a node's children without re-assigning the node's own ID.
    /// Used after the parent has already assigned the node's ID (e.g. ComponentNode boundaries).
    /// </summary>
    private static void AssignChildrenIds(NodeBase node, ref uint nextId) {
        int count = node.ChildNodes.Count;
        for (int i = 0; i < count; i++) {
            var child = node.ChildNodes[i];
            // Skip children with hardcoded game-style IDs
            if (child.NodeId < NodeIdBase) continue;

            // ComponentNode creates a new UldManager scope: assign its own ID from
            // the current counter, then start its children from 1.
            if (child is ComponentNode) {
                child.NodeId = nextId++;
                uint childNextId = 1;
                AssignChildrenIds(child, ref childNextId);
            }
            else {
                child.NodeId = nextId++;
                AssignChildrenIds(child, ref nextId);
            }
        }
    }

    /// <summary>
    /// Entry point for deterministic ID reassignment — calls the static walker on this node.
    /// </summary>
    private void ReassignSubtreeIds(ref uint nextId) {
        AssignIds(this, ref nextId);
    }

    /// <summary>
    /// Reassigns IDs for this subtree when attaching to an already-attached parent.
    /// Finds the maximum ID used by all siblings (including their subtrees) and assigns
    /// IDs sequentially from there, ensuring no collisions with existing nodes.
    /// </summary>
    private void ReassignSubtreeIdsFromParent() {
        if (parentNode is null) return;

        // Find the maximum ID in the parent's UldManager scope in a single pass,
        // then assign new IDs starting from maxId + 1.
        uint maxId = parentNode.NodeId;
        foreach (var sibling in parentNode.ChildNodes) {
            if (sibling == this) continue;
            maxId = Math.Max(maxId, MaxIdInSubtree(sibling));
        }

        uint nextId = maxId + 1;
        ReassignSubtreeIds(ref nextId);
    }

    /// <summary>
    /// Returns the maximum NodeId in the given node's subtree (including the node itself).
    /// </summary>
    private static uint MaxIdInSubtree(NodeBase node) {
        uint max = node.NodeId;
        foreach (var child in node.ChildNodes) {
            max = Math.Max(max, MaxIdInSubtree(child));
        }
        return max;
    }

    #endregion
}
