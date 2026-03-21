using System;

namespace KamiToolKit.Enums;

[Flags]
public enum ListConfigDisplayMode {
    None = 0,
    Add = 1 << 1,
    Edit = 1 << 2,
    Remove = 1 << 3,
}
