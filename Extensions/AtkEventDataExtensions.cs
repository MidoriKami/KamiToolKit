using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ModifierFlag = FFXIVClientStructs.FFXIV.Component.GUI.AtkEventData.AtkMouseData.ModifierFlag;

namespace KamiToolKit.Extensions;

public static class AtkEventDataExtensions {
    public static Vector2 GetMousePosition(ref this AtkEventData data)
        => new(data.MouseData.PosX, data.MouseData.PosY);

    public static bool IsLeftClick(ref this AtkEventData data)
        => data.MouseData.ButtonId is 0;

    public static bool IsRightClick(ref this AtkEventData data)
        => data.MouseData.ButtonId is 1;

    public static bool IsNoModifiers(ref this AtkEventData data)
        => data.MouseData.Modifier is 0;

    public static bool IsAltHeld(ref this AtkEventData data)
        => data.MouseData.Modifier.HasFlag(ModifierFlag.Alt);

    public static bool IsControlHeld(ref this AtkEventData data)
        => data.MouseData.Modifier.HasFlag(ModifierFlag.Ctrl);

    public static bool IsShiftHeld(ref this AtkEventData data)
        => data.MouseData.Modifier.HasFlag(ModifierFlag.Shift);

    public static bool IsDragging(ref this AtkEventData data)
        => data.MouseData.Modifier.HasFlag(ModifierFlag.Dragging);

    public static bool IsScrollUp(ref this AtkEventData data)
        => data.MouseData.WheelDirection is 1;

    public static bool IsScrollDown(ref this AtkEventData data)
        => data.MouseData.WheelDirection is -1;
}
