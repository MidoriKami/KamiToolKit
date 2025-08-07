using System.Numerics;
using Dalamud.Game.Addon.Events.EventDataTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Extensions;

public static unsafe class AddonEventDataExtensions {
    public static void SetHandled(this AddonEventData data, bool forced = true)
        => data.GetEvent()->SetEventIsHandled(forced);

    public static Vector2 GetMousePosition(this AddonEventData data)
        => new(data.GetMouseData().PosX, data.GetMouseData().PosY);

    public static ref AtkEventData.AtkMouseData GetMouseData(this AddonEventData data)
        => ref data.GetEventData()->MouseData;

    public static ref AtkEventData.AtkDragDropData GetDragDropData(this AddonEventData data)
        => ref data.GetEventData()->DragDropData;

    private static AtkEvent* GetEvent(this AddonEventData data)
        => (AtkEvent*)data.AtkEventPointer;

    private static AtkEventData* GetEventData(this AddonEventData data)
        => (AtkEventData*)data.AtkEventDataPointer;
}
