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
    // UldManager-chain based ID tracking: tracks used IDs per UldManager scope to prevent duplicates.
    private static readonly Dictionary<List<uint>, HashSet<uint>> _uldManagerIdScopes = new();

    /// <summary>
    /// When true, preserves this node's ID during reassignment (for nodes with hardcoded game-style IDs).
    /// </summary>
    protected virtual bool PreserveNodeId => false;

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

        // Reassign IDs: each direct child of addon root gets NodeIdBase + childIndex
        int childIndex = targetAddon.RootNode.ChildNodes.IndexOf(this);
        ReassignSubtreeIdsWithTracking(NodeIdBase + (uint)childIndex);
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

        // Reassign IDs relative to parent
        int childIndex = targetNode.ChildNodes.IndexOf(this);
        uint baseId = targetNode.NodeId + 1 + (uint)childIndex;
        ReassignSubtreeIdsWithTracking(baseId);
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
        // use a unique base derived from the target pointer hash to avoid collisions between
        // different game-managed parents.
        uint targetNodeId = targetNode->NodeId;
        if (targetNodeId < NodeIdBase) {
            uint uniqueBase = NodeIdBase + (uint)(nint)targetNode % 10000;
            ReassignSubtreeIdsWithTracking(uniqueBase);
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
    /// Reassigns IDs for this node and all its descendants, with UldManager-chain based tracking.
    /// </summary>
    private void ReassignSubtreeIdsWithTracking(uint baseId, List<uint>? chain = null) {
        if (chain == null) {
            chain = BuildUldManagerChain();
        }

        // If chain is empty, node is not yet attached — skip tracking
        bool hasScope = chain.Count > 0;

        // Check if this node has a preserved ID (hardcoded game-style ID or manual override)
        if (PreserveNodeId || NodeId < NodeIdBase) {
            if (hasScope) {
                RegisterPreservedId(this, chain);
            }
            return;
        }

        // Get or create the scope for this chain
        HashSet<uint>? scope = null;
        if (hasScope && !_uldManagerIdScopes.TryGetValue(chain, out scope)) {
            scope = _uldManagerIdScopes[chain] = new HashSet<uint>();
        }

        // Find an available ID starting from baseId
        uint assignedId = baseId;
        if (hasScope && scope != null) {
            while (scope.Contains(assignedId)) {
                assignedId++;
            }
            scope.Add(assignedId);
        }

        this.NodeId = assignedId;

        // Reassign children
        var children = this.ChildNodes.ToArray();
        for (int i = 0; i < children.Length; i++) {
            var child = children[i];

            // Check if child has a preserved ID
            if (child.NodeId < NodeIdBase) {
                if (hasScope) {
                    RegisterPreservedId(child, chain);
                }
                continue;
            }

            // Create child chain if child is a ComponentNode (has its own UldManager)
            List<uint> childChain;
            if (child is ComponentNode) {
                childChain = new List<uint>(chain) { assignedId };
            }
            else {
                childChain = chain;
            }

            uint childBaseId = assignedId + 1 + (uint)i;
            child.ReassignSubtreeIdsWithTracking(childBaseId, childChain);
        }
    }

    /// <summary>
    /// Builds the chain of node IDs from the root UldManager down to this node's UldManager.
    /// Only collects IDs of nodes that have their own UldManager (ComponentNode roots and addon roots).
    /// </summary>
    private List<uint> BuildUldManagerChain() {
        var chain = new List<uint>();
        NodeBase? node = this;

        while (node != null) {
            // Collect the ID of ancestors that are ComponentNodes (have their own UldManager)
            if (node is ComponentNode) {
                chain.Add(node.NodeId);
            }

            // Stop at addon root
            if (node.IsAddonRootNode) {
                chain.Add(node.NodeId);
                break;
            }

            // Walk up to parent
            var parent = node.parentNode;
            if (parent == null) {
                // No parent means not yet attached — return empty chain
                return new List<uint>();
            }
            node = parent;
        }

        chain.Reverse();
        return chain;
    }

    /// <summary>
    /// Registers a preserved node and all its descendants in the UldManager scope tracking.
    /// </summary>
    private static void RegisterPreservedId(NodeBase node, List<uint> chain) {
        if (!_uldManagerIdScopes.TryGetValue(chain, out var scope)) {
            scope = _uldManagerIdScopes[chain] = new HashSet<uint>();
        }
        scope.Add(node.NodeId);

        foreach (var child in node.ChildNodes) {
            if (child is ComponentNode) {
                // ComponentNode has its own scope, register it in parent's scope
                scope.Add(child.NodeId);
            }
            else {
                RegisterPreservedId(child, chain);
            }
        }
    }

    /// <summary>
    /// Removes an ID from the scope tracking.
    /// </summary>
    internal static void RemoveIdFromScope(uint nodeId, List<uint> chain) {
        if (_uldManagerIdScopes.TryGetValue(chain, out var scope)) {
            scope.Remove(nodeId);
        }
    }

    /// <summary>
    /// Clears the scope for a chain and all chains that start with it (descendant UldManagers).
    /// </summary>
    internal static void ClearScopeAndDescendants(List<uint> chain) {
        var keysToRemove = _uldManagerIdScopes.Keys
            .Where(k => k.Count >= chain.Count && k.SequenceEqual(chain.Take(k.Count)))
            .ToList();

        foreach (var key in keysToRemove) {
            _uldManagerIdScopes.Remove(key);
        }
    }

    #endregion
}
