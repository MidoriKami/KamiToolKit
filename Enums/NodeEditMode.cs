using System;

namespace KamiToolKit.Enums;

[Flags]
public enum NodeEditMode {
    Resize = 1 << 1,
    Move = 1 << 2,
}
