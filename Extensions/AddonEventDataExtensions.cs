using System.Numerics;
using Dalamud.Game.Addon.Events.EventDataTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ModifierFlag = FFXIVClientStructs.FFXIV.Component.GUI.AtkEventData.AtkMouseData.ModifierFlag;

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

    public static bool IsLeftClick(this AddonEventData data)
        => data.GetMouseData().ButtonId is 0;

    public static bool IsRightClick(this AddonEventData data)
        => data.GetMouseData().ButtonId is 1;

    public static bool IsNoModifier(this AddonEventData data)
        => data.GetMouseData().Modifier is 0;

    public static bool IsAltHeld(this AddonEventData data)
        => data.GetMouseData().Modifier.HasFlag(ModifierFlag.Alt);

    public static bool IsControlHeld(this AddonEventData data)
        => data.GetMouseData().Modifier.HasFlag(ModifierFlag.Ctrl);

    public static bool IsShiftHeld(this AddonEventData data)
        => data.GetMouseData().Modifier.HasFlag(ModifierFlag.Shift);

    public static bool IsDragging(this AddonEventData data)
        => data.GetMouseData().Modifier.HasFlag(ModifierFlag.Dragging);

    public static bool IsScrollUp(this AddonEventData data)
        => data.GetMouseData().WheelDirection is 1;

    public static bool IsScrollDown(this AddonEventData data)
        => data.GetMouseData().WheelDirection is -1;

    public static Vector2 MousePosition(this AddonEventData data)
        => new(data.GetMouseData().PosX, data.GetMouseData().PosY);

    private static AtkEvent* GetEvent(this AddonEventData data)
        => (AtkEvent*)data.AtkEventPointer;

    private static AtkEventData* GetEventData(this AddonEventData data)
        => (AtkEventData*)data.AtkEventDataPointer;
}
