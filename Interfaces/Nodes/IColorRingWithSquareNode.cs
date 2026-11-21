using System;
using Dalamud.Interface;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a color ring with a gradient square for hue, saturation, and value selection.
/// </summary>
public interface IColorRingWithSquareNode {

    /// <summary>
    /// Action invoked when the hue value changes.
    /// </summary>
    Action<float>? OnHueChanged { get; }

    /// <summary>
    /// Action invoked when the saturation value changes.
    /// </summary>
    Action<float>? OnSaturationChanged { get; }

    /// <summary>
    /// Action invoked when the value (brightness) changes.
    /// </summary>
    Action<float>? OnValueChanged { get; }

    /// <summary>
    /// Gets or sets the ring selector's color.
    /// </summary>
    ColorHelpers.HsvaColor SelectorColor { get; set; }

    /// <summary>
    /// Gets or sets the gradient square's color.
    /// </summary>
    ColorHelpers.HsvaColor SquareColor { get; set; }

    /// <summary>
    /// Gets or sets the saturation and value position within the gradient square.
    /// </summary>
    ColorHelpers.HsvaColor SquareSaturationValue { get; set; }
}
