using System.Numerics;
using Dalamud.Interface;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a node that displays a color in both RGBA and HSVA formats.
/// </summary>
public interface IColorDisplay {

    /// <summary>
    /// Gets or sets the displayed color as RGBA.
    /// </summary>
    Vector4 Color { get; set; }

    /// <summary>
    /// Gets or sets the displayed color as HSVA.
    /// </summary>
    ColorHelpers.HsvaColor ColorHsva { get; set; }
}
