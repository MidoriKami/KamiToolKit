using System;

namespace KamiToolKit.Classes;

[Flags]
public enum DrawFlags : uint {
    None = 0,
    IsDirty = 0x1,
    IsAnimating = 0x2,
    CalculateTransformation = 0x4,
    DisableRapidUp = 0x10,
    DisableRapidDown = 0x20,
    DisableRapidLeft = 0x40,
    DisableRapidRight = 0x80,
    DisableTimelineLabel = 0x100,
    ClickableCursor = 0x100000,
    RenderOnTop = 0x200000,
    TextInputCursor = 0x400000,
    UseEllipticalCollision = 0x800000,
    UseTransformedCollision = 0x1000000,
}
