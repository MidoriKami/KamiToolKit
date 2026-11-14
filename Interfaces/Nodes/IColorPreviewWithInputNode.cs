using System;
using System.Numerics;
using Dalamud.Interface;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a color preview node with a hex string input.
/// </summary>
public interface IColorPreviewWithInputNode {

    /// <summary>
    /// Action invoked when the color is changed, providing the color as HSVA.
    /// </summary>
    Action<ColorHelpers.HsvaColor>? OnHsvaColorChanged { get; set; }

    /// <summary>
    /// Action invoked when the color is changed, providing the color as RGBA.
    /// </summary>
    Action<Vector4>? OnColorChanged { get; set; }

    /// <summary>
    /// Gets or sets the hex string representation of the current color.
    /// </summary>
    ReadOnlySeString String { get; set; }

    /// <summary>
    /// Gets or sets the current color as RGBA.
    /// </summary>
    Vector4 Color { get; set; }

    /// <summary>
    /// Gets or sets the current color as HSVA.
    /// </summary>
    ColorHelpers.HsvaColor ColorHsva { get; set; }
}
