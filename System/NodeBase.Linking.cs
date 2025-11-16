using System;
using System.Runtime.CompilerServices;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using KamiToolKit.Addon;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace KamiToolKit.System;

public abstract unsafe partial class NodeBase {

    internal AtkUldManager* ParentUldManager { get; set; }

    internal void AttachNode(AtkResNode* target, NodePosition position = NodePosition.AsLastChild) {
        NodeLinker.AttachNode(ResNode, target, position);
        UpdateNative();
    }

    [OverloadResolutionPriority(1)] 
    internal void AttachNode(NodeBase target, NodePosition position = NodePosition.AsLastChild) {
        if (target is ComponentNode targetComponent) {
            AttachNode(targetComponent);
            return;
        }

        NodeLinker.AttachNode(ResNode, target, position);

        UpdateNative();
    }

    [OverloadResolutionPriority(2)] 
    internal void AttachNode(ComponentNode target, NodePosition position = NodePosition.AfterAllSiblings) {
        NodeLinker.AttachNode(ResNode, target.ComponentBase->UldManager.RootNode, position);

        if (NodeId > NodeIdBase) {
            NodeId = GetMaxNodeId(&target.ComponentBase->UldManager) + 1;
        }

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
        UpdateNative();
    }

    internal void AttachNode(NativeAddon addon, NodePosition position = NodePosition.AsLastChild) {
        NodeLinker.AttachNode(ResNode, addon.InternalAddon->RootNode, position);
        
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
        var parentAddon = ParentAddon;

        NodeLinker.DetachNode(ResNode);
        ResNode->ParentNode = null;

        if (ParentUldManager is not null) {
            VisitChildren(ResNode, pointer => {
                ParentUldManager->RemoveNodeFromObjectList(pointer);
            });
            ParentUldManager->UpdateDrawNodeList();
            ParentUldManager = null;
        }

        if (parentAddon is not null && parentAddon->UldManager.ResourceFlags.HasFlag(AtkUldManagerResourceFlag.Initialized)) {
            parentAddon->UldManager.UpdateDrawNodeList();
            parentAddon->UpdateCollisionNodeList(false);
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
        var parentAddon = GetAddonForNode(node);
        if (parentAddon is not null) {
            return &parentAddon->UldManager;
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
