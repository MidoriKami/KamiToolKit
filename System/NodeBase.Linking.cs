using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using KamiToolKit.Addon;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace KamiToolKit.System;

public abstract unsafe partial class NodeBase {

    internal readonly List<NodeBase> ChildNodes = [];
    private NodeBase? parentNode;

    internal AtkUldManager* ParentUldManager { get; set; }
    internal AtkUnitBase* ParentAddon { get; private set; }

    internal void AttachNode(AtkResNode* target, NodePosition position = NodePosition.AsLastChild) {
        NodeLinker.AttachNode(ResNode, target, position);
        UpdateParentAddonFromTarget(target);
        UpdateNative();
    }

    [OverloadResolutionPriority(1)] 
    internal void AttachNode(NodeBase target, NodePosition position = NodePosition.AsLastChild) {
        if (target is ComponentNode targetComponent) {
            AttachNode(targetComponent);
            return;
        }

        NodeLinker.AttachNode(ResNode, target, position);
        parentNode = target;
        parentNode.ChildNodes.Add(this);

        UpdateParentAddonFromTarget(target);
        UpdateNative();
    }

    [OverloadResolutionPriority(2)] 
    internal void AttachNode(ComponentNode target, NodePosition position = NodePosition.AfterAllSiblings) {
        NodeLinker.AttachNode(ResNode, target.ComponentBase->UldManager.RootNode, position);
        parentNode = target;
        parentNode.ChildNodes.Add(this);

        if (NodeId > NodeIdBase) {
            NodeId = GetMaxNodeId(&target.ComponentBase->UldManager) + 1;
        }

        UpdateParentAddonFromTarget(target);
        UpdateNative();
    }

    internal void AttachNode(AtkComponentNode* targetNode, NodePosition position = NodePosition.AfterAllSiblings) {
        void* attachTarget = position switch {
            NodePosition.AfterTarget => targetNode,
            NodePosition.BeforeTarget => targetNode,
            NodePosition.AfterAllSiblings => targetNode->Component->UldManager.RootNode,
            NodePosition.BeforeAllSiblings => targetNode->Component->UldManager.RootNode,
            NodePosition.AsLastChild => targetNode->Component->UldManager.RootNode,
            NodePosition.AsFirstChild => targetNode->Component->UldManager.RootNode,
            _ => throw new ArgumentOutOfRangeException(nameof(position), position, null),
        };

        NodeLinker.AttachNode(ResNode, (AtkResNode*)attachTarget, position);
        UpdateParentAddonFromTarget(&targetNode->AtkResNode);
        UpdateNative();
    }

    internal void AttachNode(NativeAddon addon, NodePosition position = NodePosition.AsLastChild) {
        NodeLinker.AttachNode(ResNode, addon.InternalAddon->RootNode, position);
        parentNode = addon.RootNode;
        parentNode.ChildNodes.Add(this);
        ParentAddon = addon;
        
        VisitManagedChildren(this, node => node.ParentAddon = addon);

        if (NodeId > NodeIdBase) {
            NodeId = GetMaxNodeId(&addon.InternalAddon->UldManager) + 1;
        }
        
        UpdateNative();
    }

    internal void ReattachNode(AtkResNode* newTarget) {
        DetachNode();
        AttachNode(newTarget);
    }

    internal void DetachNode() {
        NodeLinker.DetachNode(ResNode);
        ResNode->ParentNode = null;

        if (ParentUldManager is not null) {
            VisitChildren(ResNode, pointer => {
                ParentUldManager->RemoveNodeFromObjectList(pointer);
            });

            ParentUldManager->UpdateDrawNodeList();
            ParentUldManager = null;
        }

        if (ParentAddon is not null && ParentAddon->UldManager.ResourceFlags.HasFlag(AtkUldManagerResourceFlag.Initialized)) {
            ParentAddon->UldManager.UpdateDrawNodeList();
            ParentAddon->UpdateCollisionNodeList(false);
        }

        ParentAddon = null;

        if (parentNode is not null) {
            parentNode.ChildNodes.Remove(this);
            parentNode = null;
        }
    }

    private void UpdateNative() {
        if (ResNode is null) return;

        MarkDirty();

        if (ParentUldManager is null) {
            ParentUldManager = GetUldManagerForNode(ResNode);
        }

        if (ParentUldManager is not null) {
            VisitChildren(ResNode, pointer => {
                ParentUldManager->AddNodeToObjectList(pointer);
            });

            ParentUldManager->UpdateDrawNodeList();
        }

        if (ParentAddon is not null) {
            ParentAddon->UldManager.UpdateDrawNodeList();
            ParentAddon->UpdateCollisionNodeList(false);
        }
    }

    private void UpdateParentAddonFromTarget(AtkResNode* node) {
        if (parentNode is not null && parentNode.ParentAddon is not null) {
            ParentAddon = parentNode.ParentAddon;

            foreach (var child in ChildNodes.SelectMany(childNode => childNode.ChildNodes)) {
                child.ParentAddon = ParentAddon;
            }
        }
        else if (ParentAddon is null) {
            var targetParentAddon = GetAddonForNode(node);
            if (targetParentAddon is not null) {
                ParentAddon = targetParentAddon;
            }
        }

        if (ParentAddon is not null) {
            VisitManagedChildren(this, child => child.ParentAddon = ParentAddon);
        }
    }

    private void VisitManagedChildren(NodeBase node, Action<NodeBase> visitAction) {
        foreach (var child in node.ChildNodes) {
            visitAction(child);
            VisitManagedChildren(child, visitAction);
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

    private void VisitChildren(AtkResNode* parent, Action<Pointer<AtkResNode>> visitAction) {
        visitAction(parent);

        var child = parent->ChildNode;

        while (child is not null) {
            visitAction(child);

            // Be sure to not accidentally visit a components children, they manage their own children
            if (child->ChildNode is not null && child->GetNodeType() is not NodeType.Component) {
                VisitChildren(child->ChildNode, visitAction);
            }

            child = child->PrevSiblingNode;
        }
    }

    private AtkUnitBase* GetAddonForNode(AtkResNode* node)
        => RaptureAtkUnitManager.Instance()->GetAddonByNode(node);

    private uint GetMaxNodeId(AtkUldManager* uldManager) {
        if (uldManager is null) return 0;
        
        uint max = 1;
        foreach (var child in uldManager->Nodes) {
            if (child.Value is null) continue;

            max = Math.Max(child.Value->NodeId, max);
        }

        return max;
    }
}
