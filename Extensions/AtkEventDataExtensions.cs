using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Extensions;

/// <summary>
/// Extension methods for AtkEventData.
/// </summary>
public static class AtkEventDataExtensions {
    extension(ref AtkEventData data) {

        /// <summary>
        /// Gets the mouse position for this event.
        /// </summary>
        public Vector2 MousePosition => new(data.MouseData.PosX, data.MouseData.PosY);

        /// <summary>
        /// Gets if this event was a left click.
        /// </summary>
        public bool IsLeftClick => data.MouseData.ButtonId is 0;

        /// <summary>
        /// Gets if this event was a right click.
        /// </summary>
        public bool IsRightClick => data.MouseData.ButtonId is 1;

        /// <summary>
        /// Gets if this event did not have any key modifiers like Alt, Ctrl, Shift or Dragging.
        /// </summary>
        public bool IsNoModifiers => data.MouseData.Modifier is 0;

        /// <summary>
        /// Gets if Alt was held during this event.
        /// </summary>
        public bool IsAltHeld => data.MouseData.Modifier.HasFlag(ModifierFlag.Alt);

        /// <summary>
        /// Gets if Control was held during this event.
        /// </summary>
        public bool IsControlHeld => data.MouseData.Modifier.HasFlag(ModifierFlag.Ctrl);

        /// <summary>
        /// Gets if Shift was held during this event.
        /// </summary>
        public bool IsShiftHeld => data.MouseData.Modifier.HasFlag(ModifierFlag.Shift);

        /// <summary>
        /// Gets if this event is a mouse drag.
        /// </summary>
        public bool IsDragging => data.MouseData.Modifier.HasFlag(ModifierFlag.Dragging);

        /// <summary>
        /// Gets if this was a scroll up.
        /// </summary>
        /// <remarks>
        /// <see cref="ScrollValue"/> to get the intensity of the scroll signed. Negative is down.
        /// </remarks>
        public bool IsScrollUp => data.MouseData.WheelDirection >= 1;

        /// <summary>
        /// Gets if this was a scroll down.
        /// </summary>
        /// <remarks>
        /// <see cref="ScrollValue"/> to get the intensity of the scroll signed. Negative is down.
        /// </remarks>
        public bool IsScrollDown => data.MouseData.WheelDirection <= -1;

        /// <summary>
        /// Gets the scroll intensity, this is the per-frame value for the scroll, its value can be as low as 0/1 or as high as 10/20+ depending on scroll speed.
        /// </summary>
        public int ScrollValue => data.MouseData.WheelDirection;
    }
}
