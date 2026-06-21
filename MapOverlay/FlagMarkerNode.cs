using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace KamiToolKit.MapOverlay;

/// <summary>
/// Specialized implementation of a <see cref="MapMarkerNode"/> for use in <see cref="MapOverlayController"/>
/// to render a flag marker node over top all the other custom marker nodes so flags can still be seen when using the overlay.
/// </summary>
public unsafe class FlagMarkerNode : MapMarkerNode {

    /// <summary>
    /// Constructs a instance of <see cref="FlagMarkerNode"/>
    /// </summary>
    public FlagMarkerNode() {
        IconId = 60561;
        AllowAnyMap = true;
        Size = new Vector2(32.0f, 32.0f);
    }

    /// <inheritdoc />
    protected override void OnUpdate() {
        var agentMap = AgentMap.Instance();

        ref var flagMarker = ref agentMap->FlagMapMarkers[0];

        if (IconId != flagMarker.MapMarker.IconId) {
            IconId = flagMarker.MapMarker.IconId;
        }

        // For flags, take the map positions of the flag, remove the offset from it
        // then multiply by mapSize before adding offset back
        var markerXPos = (flagMarker.XFloat - agentMap->SelectedOffsetX) * agentMap->SelectedMapSizeFactorFloat + agentMap->SelectedOffsetX;
        var markerYPos = (flagMarker.YFloat - agentMap->SelectedOffsetY) * agentMap->SelectedMapSizeFactorFloat + agentMap->SelectedOffsetY;

        Position = new Vector2(markerXPos, markerYPos);
        IsVisible = agentMap->FlagMarkerCount is not 0 && flagMarker.TerritoryId == agentMap->SelectedTerritoryId;

        base.OnUpdate();
    }
}
