using System;

namespace KamiToolKit.Classes;

[Flags]
public enum TextInputFlags : ushort {
    Capitalize = 0x1,
    Mask = 0x2,
    EnableDictionary = 0x4,
    EnableHistory = 0x8,
    EnableIme = 0x10,
    EscapeClears = 0x20,
    AllowUpperCase = 0x40,
    AllowLowerCase = 0x80,
    AllowNumberInput = 0x100,
    AllowSymbolInput = 0x200,
    WordWrap = 0x400,
    MultiLine = 0x800,
    AutoMaxWidth = 0x1000,
}
