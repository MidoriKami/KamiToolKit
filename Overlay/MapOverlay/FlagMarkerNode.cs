using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace KamiToolKit.Overlay.MapOverlay;

public unsafe class FlagMarkerNode : MapMarkerNode {
    public FlagMarkerNode() {
        IconId = 60561;
        AllowAnyMap = true;
        Size = new Vector2(32.0f, 32.0f);
        IsPrescaled = true;
    }

    protected override void OnUpdate() {
        var agentMap = AgentMap.Instance();
        
        ref var flagMarker = ref agentMap->FlagMapMarkers[0];
        
        Position = new Vector2(flagMarker.XFloat, flagMarker.YFloat);
        IsVisible = agentMap->FlagMarkerCount is not 0 && flagMarker.TerritoryId == agentMap->SelectedTerritoryId;

        base.OnUpdate();
    }
}
