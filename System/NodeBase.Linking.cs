using System;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using KamiToolKit.Addon;
using KamiToolKit.Classes;
using KamiToolKit.Extensions;
using KamiToolKit.Nodes;

namespace KamiToolKit.System;

public abstract unsafe partial class NodeBase {

    private bool HasAddon { get; set; }
    private AtkUnitBase* ParentAddon { get; set; }
    private AtkUldManager* ParentUldManager { get; set; }

    internal void AttachNode(AtkResNode* target, NodePosition position = NodePosition.AsLastChild) {
        NodeLinker.AttachNode(InternalResNode, target, position);
        UpdateNative();
    }

    internal void AttachNode(NodeBase target, NodePosition position = NodePosition.AsLastChild) {
        if (target is ComponentNode node) {
            AttachNode(node);
            return;
        }

        NodeLinker.AttachNode(InternalResNode, target.InternalResNode, position);
        EnableChildEvents(target);
        UpdateNative();
    }

    internal void AttachNode(ComponentNode target, NodePosition position = NodePosition.AfterAllSiblings) {
        NodeLinker.AttachNode(InternalResNode, target.ComponentBase->UldManager.RootNode, position);
        EnableChildEvents(target);
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

        NodeLinker.AttachNode(InternalResNode, (AtkResNode*)attachTarget, position);
        UpdateNative();
    }

    internal void AttachNode(NativeAddon addon, NodePosition position = NodePosition.AsLastChild) {
        NodeLinker.AttachNode(InternalResNode, addon.InternalAddon->RootNode, position);
        UpdateNative();
    }

    internal void ReattachNode(AtkResNode* newTarget) {
        DetachNode(false);
        AttachNode(newTarget);
    }

    internal void ReattachNode(NodeBase target) {
        DetachNode(false);
        AttachNode(target);
    }

    internal void DetachNode(bool disableEvents = true) {
        if (disableEvents) DisableEvents();

        NodeLinker.DetachNode(InternalResNode);

        if (ParentUldManager is not null) {
            VisitChildren(InternalResNode, pointer => {
                ParentUldManager->RemoveNodeFromObjectList(pointer);
            });
            ParentUldManager->UpdateDrawNodeList();
            ParentUldManager = null;
        }

        if (ParentAddon is not null) {
            ParentAddon->UldManager.UpdateDrawNodeList();
            ParentAddon->UpdateCollisionNodeList(false);
            ParentAddon = null;
        }
    }

    private void UpdateNative() {
        if (InternalResNode is null) return;

        MarkDirty();

        if (ParentUldManager is null) {
            ParentUldManager = GetUldManagerForNode(InternalResNode);
        }

        if (ParentUldManager is not null) {
            VisitChildren(InternalResNode, pointer => {
                ParentUldManager->AddNodeToObjectList(pointer);
            });
            ParentUldManager->UpdateDrawNodeList();
        }

        if (ParentAddon is null) {
            ParentAddon = GetAddonForNode(InternalResNode);
        }

        if (ParentAddon is not null) {
            ParentAddon->UldManager.UpdateDrawNodeList();
            ParentAddon->UpdateCollisionNodeList(false);
        }
    }

    private void EnableChildEvents(NodeBase targetParent) {
        if (targetParent.EventsActive) {
            EnableEvents(targetParent.EventAddonPointer);
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
}
