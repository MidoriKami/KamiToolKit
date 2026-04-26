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

public unsafe class MapOverlayController : IDisposable {
    private readonly AddonController<AddonAreaMap> mapController;
    private SimpleOverlayNode? clippingContainerNode;
    private SimpleOverlayNode? flagContainerNode;
    private SimpleOverlayNode? overlayNode;
    private ViewportEventListener? viewportEventListener;

    private bool showingTooltip;
    private bool showingInteractCursor;

    private readonly List<MapMarkerNode> markerNodes = [];

    private readonly List<MapMarkerInfo> queuedMarkers = [];
    private readonly List<MapMarkerNode> queuedNodes = [];

    private MapMarkerNode? flagNode;

    public bool IsVisible { get; set; } = true;
    
    public MapOverlayController() {
        mapController = new AddonController<AddonAreaMap> {
            AddonName = "AreaMap",
            OnSetup = OnAttach,
            OnPreUpdate = OnUpdate,
            OnFinalize = OnDetach,
        };

        mapController.Enable();
    }

    public void Dispose() {
        viewportEventListener?.Dispose();
        viewportEventListener = null;
        
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

    private void OnAttach(AddonAreaMap* addon) {
        var mapComponentNode = addon->GetNodeById(53);
        if (mapComponentNode is null) return;

        clippingContainerNode = new SimpleOverlayNode {
            NodeFlags = NodeFlags.Clip | NodeFlags.Visible,
        };
        clippingContainerNode.AttachNode(mapComponentNode, NodePosition.AfterTarget);

        viewportEventListener = new ViewportEventListener(OnViewportEvent);
        viewportEventListener.AddEvent(AtkEventType.MouseMove, clippingContainerNode);
        viewportEventListener.AddEvent(AtkEventType.MouseDown, clippingContainerNode);
        
        overlayNode = new SimpleOverlayNode();
        overlayNode.AttachNode(clippingContainerNode);
        
        flagContainerNode = new SimpleOverlayNode();
        flagContainerNode.AttachNode(clippingContainerNode);

        flagNode = new MapMarkerNode {
            Size = new Vector2(32.0f, 32.0f),
            IconId = 60561,
            AllowAnyMap = true,
        };
        flagNode.AttachNode(flagContainerNode);
    }

    private void OnUpdate(AddonAreaMap* addon) {
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

        ref var areaMap = ref addon->AreaMap;

        var mapComponent = areaMap.ComponentMap;
        if (mapComponent is null) return;

        clippingContainerNode.IsVisible = !agentMap->IsControlKeyPressed && IsVisible;

        clippingContainerNode.Size = mapComponent->OwnerNode->AtkResNode.Size;
        clippingContainerNode.Position = mapComponent->OwnerNode->AtkResNode.Position;

        var mapComponentNode = mapComponent->OwnerNode->AtkResNode;
        var center = mapComponentNode.Size / 2.0f + new Vector2(18.0f, 46.0f);

        overlayNode.Scale = new Vector2(areaMap.MapScale, areaMap.MapScale);
        overlayNode.Size = new Vector2(2048.0f, 2048.0f);

        // Start with current position
        var offset = new Vector2(areaMap.MapOffsetX, areaMap.MapOffsetY);

        // Add map-specific offset using the selected map
        offset += new Vector2(agentMap->SelectedOffsetX, agentMap->SelectedOffsetY);

        // Set object position relative to center of node
        offset += overlayNode.Size / 2.0f;

        // Scale to current Zoom Level
        offset *= mapComponent->MapScale;

        overlayNode.Position = center - offset - clippingContainerNode.Position;

        foreach (var marker in markerNodes) {
            marker.Update();
            marker.Scale = Vector2.One / new Vector2(areaMap.MarkerPositionScaling, areaMap.MarkerPositionScaling);
        }

        UpdateFlagNode(agentMap, areaMap);
    }

    private void OnDetach(AddonAreaMap* addon) {
        viewportEventListener?.Dispose();
        viewportEventListener = null;

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
                AllowAnyMap = markerInfo.AllowAnyMap,
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

    private void UpdateFlagNode(AgentMap* agentMap, Atk2DAreaMap areaMap) {
        if (overlayNode is null) return;

        if (flagContainerNode is not null && flagNode is not null) {
            flagContainerNode.Size = overlayNode.Size;
            flagContainerNode.Scale = overlayNode.Scale;
            flagContainerNode.Position = overlayNode.Position;

            ref var flagMarker = ref agentMap->FlagMapMarkers[0];
        
            flagNode?.Position = new Vector2(flagMarker.XFloat, flagMarker.YFloat);
            flagNode?.IsVisible = agentMap->FlagMarkerCount is not 0;
            flagNode?.Update();
            flagNode?.Scale = Vector2.One / new Vector2(areaMap.MarkerPositionScaling, areaMap.MarkerPositionScaling);
        }
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
        if (clippingContainerNode is null) return;
        
        var mapAddon = RaptureAtkUnitManager.Instance()->GetAddonByName("AreaMap");
        if (mapAddon is null) return;

        if (RaptureAtkModule.Instance()->AtkCollisionManager.IntersectingAddon != mapAddon) return;

        var anyCollisions = false;

        if (!AgentMap.Instance()->IsControlKeyPressed) {
            foreach (var node in markerNodes) {
                if (node.IsActuallyVisible() && node.CheckCollision(atkEventData) && clippingContainerNode.CheckCollision(atkEventData)) {
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
