using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using KamiToolKit.Nodes;

namespace KamiToolKit.Enums;

/// <summary>
/// Enum representing a <see cref="HotbarNode"/>'s cost text color.
/// </summary>
public enum CostTextColor {

    /// <summary>
    /// Purple-ish Color representing Mana.
    /// </summary>
    Mana,

    /// <summary>
    /// Brown-ish Color representing CP.
    /// </summary>
    DoL,
}

/// <summary>
/// Extensions for <see cref="CostTextColor"/> to get the vector color from the enum value.
/// </summary>
public static class CostTextColorExtensions {
    extension(CostTextColor color) {

        /// <summary>
        /// Gets the RGBA primary text color for this value.
        /// </summary>
        public Vector4 TextColor => color switch {
            CostTextColor.Mana => new Vector4(1.000f, 0.851f, 0.980f, 1.000f), // Slight Purple
            CostTextColor.DoL => new Vector4(1.000f, 0.945f, 0.831f, 1.000f), // Slight brown
            _ => KnownColor.White.Vector(),
        };

        /// <summary>
        /// Gets the RGBA outline color for this value.
        /// </summary>
        public Vector4 TextOutlineColor => color switch {
            CostTextColor.Mana => new Vector4(0.596f, 0.314f, 0.565f, 1.000f),
            CostTextColor.DoL => new Vector4(0.498f, 0.486f, 0.114f, 1.000f),
            _ => KnownColor.Black.Vector(),
        };
    }
}
