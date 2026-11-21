using System;
using System.Numerics;
using Dalamud.Interface;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a color picker node with hue, saturation, value, and alpha controls.
/// </summary>
public interface IColorPickerNode {

    /// <summary>
    /// Gets the current color in HSVA format.
    /// </summary>
    ColorHelpers.HsvaColor CurrentColor { get; }

    /// <summary>
    /// Action invoked when the color is changed, providing the color as HSVA.
    /// </summary>
    Action<ColorHelpers.HsvaColor>? OnColorPreviewed { get; set; }

    /// <summary>
    /// Action invoked when the color is changed, providing the color as RGBA.
    /// </summary>
    Action<Vector4>? OnRgbaColorPreviewed { get; set; }

    /// <summary>
    /// Sets the current color's alpha.
    /// </summary>
    /// <param name="alpha">The alpha value.</param>
    void SetAlpha(float alpha);

    /// <summary>
    /// Sets the current color's hue.
    /// </summary>
    /// <param name="hue">The hue value.</param>
    void SetHue(float hue);

    /// <summary>
    /// Sets the current color's saturation.
    /// </summary>
    /// <param name="saturation">The saturation value.</param>
    void SetSaturation(float saturation);

    /// <summary>
    /// Sets the current color's value (brightness).
    /// </summary>
    /// <param name="value">The value.</param>
    void SetValue(float value);

    /// <summary>
    /// Sets the current color as RGBA.
    /// </summary>
    /// <param name="color">The RGBA color.</param>
    void SetColor(Vector4 color);
}
