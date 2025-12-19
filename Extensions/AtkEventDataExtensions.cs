using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ModifierFlag = FFXIVClientStructs.FFXIV.Component.GUI.AtkEventData.AtkMouseData.ModifierFlag;

namespace KamiToolKit.Extensions;

public static class AtkEventDataExtensions {
    extension(ref AtkEventData data) {
        public Vector2 MousePosition => new(data.MouseData.PosX, data.MouseData.PosY);
        public bool IsLeftClick => data.MouseData.ButtonId is 0;
        public bool IsRightClick => data.MouseData.ButtonId is 1;
        public bool IsNoModifiers => data.MouseData.Modifier is 0;
        public bool IsAltHeld => data.MouseData.Modifier.HasFlag(ModifierFlag.Alt);
        public bool IsControlHeld => data.MouseData.Modifier.HasFlag(ModifierFlag.Ctrl);
        public bool IsShiftHeld => data.MouseData.Modifier.HasFlag(ModifierFlag.Shift);
        public bool IsDragging => data.MouseData.Modifier.HasFlag(ModifierFlag.Dragging);
        public bool IsScrollUp => data.MouseData.WheelDirection is 1;
        public bool IsScrollDown => data.MouseData.WheelDirection is -1;
    }
}
