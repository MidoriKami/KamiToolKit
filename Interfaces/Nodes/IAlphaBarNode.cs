using System;
using System.Numerics;
using Dalamud.Interface;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing an alpha bar node for adjusting color transparency.
/// </summary>
public interface IAlphaBarNode {

    /// <summary>
    /// Gets or sets the current color value.
    /// </summary>
    Vector4 Color { get; set; }

    /// <summary>
    /// Gets or sets the current color in HSVA format.
    /// </summary>
    ColorHelpers.HsvaColor ColorHsva { get; set; }

    /// <summary>
    /// Action invoked when the alpha value changes.
    /// </summary>
    /// <remarks>
    /// Alpha value is in the range of 0.0f to 1.0f.
    /// </remarks>
    Action<float>? OnAlphaChanged { get; }
}
