using System;
using System.Collections.Generic;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;

namespace KamiToolKit.Internal.Classes;

internal unsafe class NativeListAttachment(NodeBase node, AtkComponentListItemRenderer* renderer, Dictionary<NodeBase, NativeListAttachment> attachments) : IDisposable {

    public NodeBase Node { get; } = node;

    private readonly Experimental.ListItemRenderer* rendererData = (Experimental.ListItemRenderer*)renderer;
    private readonly uint nodeId = node.ResNode->NodeId;
    private bool isAttached;
    private bool isDisposed;

    private static readonly AtkEventType[] EventTypes = [
        AtkEventType.MouseOver, AtkEventType.MouseOut, AtkEventType.MouseDown, AtkEventType.MouseUp,
        AtkEventType.MouseClick, AtkEventType.MouseDoubleClick, AtkEventType.InputReceived,
        AtkEventType.DragDropCanAcceptCheck, AtkEventType.FocusStop, AtkEventType.FocusStart,
    ];

    public void Attach() {
        if (isDisposed || isAttached) {
            throw new InvalidOperationException("Attachment is no longer available.");
        }

        var count = rendererData->CollisionNodeListCount;
        if (count == ushort.MaxValue) {
            throw new InvalidOperationException("Renderer collision list is full.");
        }

        var buffer = (AtkResNode**)IMemorySpace.GetUISpace()->Realloc<nint>(rendererData->CollisionNodeList, count + 1);
        if (buffer is null) {
            throw new OutOfMemoryException();
        }

        new Span<nint>(buffer, count).CopyTo(new Span<nint>(buffer + 1, count));
        buffer[0] = Node;

        rendererData->CollisionNodeList = buffer;
        rendererData->CollisionNodeListCount++;

        isAttached = true;
        attachments.Add(Node, this);

        foreach (var eventType in EventTypes) {
            Node.ResNode->AtkEventManager.RegisterEvent(eventType, nodeId, null, Node, (AtkEventListener*)renderer, false);
        }

        Node.AddNodeFlags(NodeFlags.HasCollision, NodeFlags.RespondToMouse);
        Node.RemoveNodeFlags(NodeFlags.EmitsEvents);
        Node.ParentAddon->UpdateCollisionNodeList(false);
    }

    public void Dispose() {
        ThreadSafety.AssertMainThread();
        if (isDisposed) return;
        isDisposed = true;
        attachments.Remove(Node);
        if (!isAttached) return;

        var node = Node.ResNode;
        for (var index = 0; index < rendererData->CollisionNodeListCount; index++) {
            if (rendererData->CollisionNodeList[index] != node) {
                continue;
            }

            rendererData->CollisionNodeListCount--;

            for (var next = index; next < rendererData->CollisionNodeListCount; next++) {
                rendererData->CollisionNodeList[next] = rendererData->CollisionNodeList[next + 1];
            }

            rendererData->CollisionNodeList[rendererData->CollisionNodeListCount] = null;
            break;
        }

        foreach (var eventType in EventTypes) {
            node->AtkEventManager.UnregisterEvent(eventType, nodeId, (AtkEventListener*)renderer, false);
        }

        if (rendererData->EventNode == node) rendererData->EventNode = null;
        for (var parent = (AtkResNode*)renderer->OwnerNode; parent is not null; parent = parent->ParentNode) {
            if (parent->GetNodeType() is not NodeType.Component) {
                continue;
            }

            var component = parent->GetComponent();
            if (component->GetComponentType() is not (ComponentType.List or ComponentType.TreeList)) {
                continue;
            }

            var list = (AtkComponentList*)component;
            if ((AtkResNode*)list->HoveredItemCollisionNode == node) {
                list->HoveredItemCollisionNode = null;
            }

            break;
        }

        var stage = AtkStage.Instance();

        if (stage->TooltipManager.TargetNode == node) {
            Node.HideTooltip();
        }

        if ((AtkResNode*)stage->AtkCollisionManager->IntersectingCollisionNode == node) {
            stage->AtkCollisionManager->IntersectingCollisionNode = null;
        }

        foreach (ref var savedNode in stage->AtkInputManager->SavedMouseClicksCollisionNodes) {
            if ((AtkResNode*)savedNode.Value == node) {
                savedNode = null;
            }
        }
        Node.Dispose();
    }
}
