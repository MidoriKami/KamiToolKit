using System;
using KamiToolKit.Nodes;

namespace KamiToolKit.Enums;

/// <summary>
/// Flags for enabling various <see cref="TextInputNode"/> features.
/// </summary>
[Flags]
public enum TextInputFlags : ushort {
    /// <summary>
    /// Capitalize
    /// </summary>
    Capitalize = 0x1,

    /// <summary>
    /// Mask
    /// </summary>
    Mask = 0x2,

    /// <summary>
    /// EnableDictionary
    /// </summary>
    EnableDictionary = 0x4,

    /// <summary>
    /// EnableHistory
    /// </summary>
    EnableHistory = 0x8,

    /// <summary>
    /// EnableIme
    /// </summary>
    EnableIme = 0x10,

    /// <summary>
    /// EscapeClears
    /// </summary>
    EscapeClears = 0x20,

    /// <summary>
    /// AllowUpperCase
    /// </summary>
    AllowUpperCase = 0x40,

    /// <summary>
    /// AllowLowerCase
    /// </summary>
    AllowLowerCase = 0x80,

    /// <summary>
    /// AllowNumberInput
    /// </summary>
    AllowNumberInput = 0x100,

    /// <summary>
    /// AllowSymbolInput
    /// </summary>
    AllowSymbolInput = 0x200,

    /// <summary>
    /// WordWrap
    /// </summary>
    WordWrap = 0x400,

    /// <summary>
    /// MultiLine
    /// </summary>
    MultiLine = 0x800,

    /// <summary>
    /// AutoMaxWidth
    /// </summary>
    AutoMaxWidth = 0x1000,
}
