using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Controllers;
using KamiToolKit.Dalamud;
using KamiToolKit.Premade.Node.Simple;
using MapMarkerInfo = KamiToolKit.Classes.MapMarkerInfo;

namespace KamiToolKit.Overlay.MapOverlay;

public abstract unsafe class MapOverlayControllerBase<T> : IDisposable where T : unmanaged {
    private readonly AddonController mapController;
    private SimpleOverlayNode? clippingContainerNode;
    private SimpleOverlayNode? overlayNode;

    private bool showingTooltip;
    private bool showingInteractCursor;
    private readonly string addonName;

    private readonly List<MapMarkerNode> markerNodes = [];

    private readonly List<MapMarkerInfo> queuedMarkers = [];
    private readonly List<MapMarkerNode> queuedNodes = [];

    public bool IsVisible { get; set; } = true;

    protected ViewportEventListener? ViewportEventListener;
    protected abstract AtkComponentNode* GetMapComponentNode(T* addon);
    protected abstract Atk2DMap* GetMapDataPointer(T* addon);

    protected MapOverlayControllerBase(string targetAddon) {
        addonName = targetAddon;
        
        mapController = new AddonController {
            AddonName = targetAddon,
            OnSetup = OnAttach,
            OnPreUpdate = OnUpdate,
            OnFinalize = OnDetach,
        };

        mapController.Enable();
    }

    public void Dispose() {
        ViewportEventListener?.Dispose();
        ViewportEventListener = null;
        
        mapController.Dispose();

        RemoveAllMarkers();

        overlayNode?.Dispose();
        overlayNode = null;
        
        clippingContainerNode?.Dispose();
        clippingContainerNode = null;
    }

    public void AddMarker(MapMarkerInfo markerInfo) {
        queuedMarkers.Add(markerInfo);
    }

    public void AddMarker(MapMarkerNode marker) {
        queuedNodes.Add(marker);
    }

    public void RemoveMarker(MapMarkerNode marker) {
        if (queuedNodes.Remove(marker)) {
            marker.Dispose();
        }

        if (markerNodes.Remove(marker)) {
            marker.Dispose();
        }
    }

    public void RemoveAllMarkers() {
        foreach (var node in markerNodes) {
            node.Dispose();
        }
        markerNodes.Clear();

        foreach (var node in queuedNodes) {
            node.Dispose();
        }
        queuedNodes.Clear();

        queuedMarkers.Clear();
    }

    protected void OnAttach(AtkUnitBase* addon) {
        var mapComponentNode = GetMapComponentNode((T*)addon);
        if (mapComponentNode is null) return;

        clippingContainerNode = new SimpleOverlayNode {
            NodeFlags = NodeFlags.Clip | NodeFlags.Visible,
        };
        clippingContainerNode.AttachNode(mapComponentNode, NodePosition.AfterTarget);

        ViewportEventListener = new ViewportEventListener(OnViewportEvent);
        ViewportEventListener.AddEvent(AtkEventType.MouseMove, clippingContainerNode);
        ViewportEventListener.AddEvent(AtkEventType.MouseDown, clippingContainerNode);
        
        overlayNode = new SimpleOverlayNode();
        overlayNode.AttachNode(clippingContainerNode);
    }

    protected void OnUpdate(AtkUnitBase* addon) {
        if (clippingContainerNode is null) return;
        if (overlayNode is null) return;

        var agentMap = AgentMap.Instance();
        
        if (showingTooltip && agentMap->IsControlKeyPressed) {
            AtkStage.Instance()->TooltipManager.HideTooltip(addon->Id);
            showingTooltip = false;
        }
        
        if (Services.ClientState.IsPvP) {
            clippingContainerNode.IsVisible = false;
            return;
        }

        ProcessQueues();

        var areaMap = GetMapDataPointer((T*)addon);

        var mapComponent = GetMapComponentNode((T*)addon)->GetAsAtkComponentMap();
        if (mapComponent is null) return;

        clippingContainerNode.IsVisible = !agentMap->IsControlKeyPressed && IsVisible;

        clippingContainerNode.Size = mapComponent->OwnerNode->AtkResNode.Size;
        clippingContainerNode.Position = mapComponent->OwnerNode->AtkResNode.Position;

        var mapComponentNode = mapComponent->OwnerNode->AtkResNode;
        var center = mapComponentNode.Size / 2.0f + new Vector2(18.0f, 46.0f);

        overlayNode.Scale = new Vector2(areaMap->MarkerPositionScaling, areaMap->MarkerPositionScaling);
        overlayNode.Size = new Vector2(2048.0f, 2048.0f);

        // Start with current position
        var offset = new Vector2(areaMap->X, areaMap->Y);

        // Add map-specific offset
        offset += new Vector2(agentMap->CurrentOffsetX, agentMap->CurrentOffsetY);

        // Set object position relative to center of node
        offset += overlayNode.Size / 2.0f;

        // Scale to current Zoom Level
        offset *= mapComponent->MapScale;

        overlayNode.Position = center - offset - clippingContainerNode.Position;

        foreach (var marker in markerNodes) {
            marker.Update();
            marker.Scale = Vector2.One / new Vector2(areaMap->MarkerPositionScaling, areaMap->MarkerPositionScaling);
        }
    }

    protected void OnDetach(AtkUnitBase* addon) {
        foreach (var marker in markerNodes) {
            marker.DetachNode();
            queuedNodes.Add(marker);
        }
        markerNodes.Clear();

        clippingContainerNode?.Dispose();
        clippingContainerNode = null;
        
        overlayNode?.Dispose();
        overlayNode = null;
    }
    
    private void ProcessQueues() {
        foreach (var markerInfo in queuedMarkers) {
            var newMarkerNode = new MapMarkerNode {
                IconId = markerInfo.IconId,
                MapId = markerInfo.MapId,
                Texture = markerInfo.Texture,
                TexturePath = markerInfo.TexturePath,
                Size = markerInfo.Size ?? new Vector2(16.0f, 16.0f),
                Origin = (markerInfo.Size ?? new Vector2(16.0f, 16.0f)) / 2.0f,
                Position = markerInfo.Position ?? new Vector2(1024.0f, 1024.0f),
                TextTooltip = markerInfo.Tooltip ?? string.Empty,
            };
    
            markerNodes.Add(newMarkerNode);
            newMarkerNode.AttachNode(overlayNode);
        }
        queuedMarkers.Clear();

        foreach (var markerNode in queuedNodes) {
            markerNodes.Add(markerNode);
            markerNode.AttachNode(overlayNode);
        }
        queuedNodes.Clear();
    }
    
    private void OnViewportEvent(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        switch (eventType) {
            case AtkEventType.MouseMove:
                ProcessMouseMove(atkEventData);
                break;

            case AtkEventType.MouseDown when !AgentMap.Instance()->IsControlKeyPressed:
                ProcessMouseClick(atkEventData);
                break;
        }
    }

    private void ProcessMouseMove(AtkEventData* atkEventData) {
        var mapAddon = RaptureAtkUnitManager.Instance()->GetAddonByName(addonName);
        if (mapAddon is null) return;

        var anyCollisions = false;

        if (!AgentMap.Instance()->IsControlKeyPressed) {
            foreach (var node in markerNodes) {
                if (node.IsActuallyVisible() && node.CheckCollision(atkEventData)) {
                    node.ShowTextTooltip(node.TextTooltip);
                    showingTooltip = true;
                    anyCollisions = true;
                }

                if (node.OnClick is not null) {
                    Services.AddonEventManager.SetCursor(AddonCursorType.Clickable);
                    showingInteractCursor = true;
                }
            }
        }

        if (!anyCollisions && showingTooltip) {
            AtkStage.Instance()->TooltipManager.HideTooltip(mapAddon->Id);
            showingTooltip = false;
        }

        if (!anyCollisions && showingInteractCursor) {
            Services.AddonEventManager.ResetCursor();
            showingInteractCursor = false;
        }
    }

    private void ProcessMouseClick(AtkEventData* atkEventData) {
        foreach (var node in markerNodes) {
            if (node.IsActuallyVisible() && node.CheckCollision(atkEventData)) {
                node.OnClick?.Invoke();
            }
        }
    }
}
