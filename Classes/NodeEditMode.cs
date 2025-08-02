using System;

namespace KamiToolKit.Classes;

[Flags]
public enum NodeEditMode {
    Resize = 1 << 1,
    Move = 1 << 2,
}
