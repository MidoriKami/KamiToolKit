using System;
using System.Numerics;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a color edit node with a clickable color square and label.
/// </summary>
public interface IColorEditNode : ITextLabel {

    /// <summary>
    /// Gets or sets the current color.
    /// </summary>
    Vector4 CurrentColor { get; set; }

    /// <summary>
    /// Gets or sets the default color shown when the user clicks "Default" in the color picker.
    /// </summary>
    Vector4? DefaultColor { get; set; }

    /// <summary>
    /// Action invoked when the color picker is canceled.
    /// </summary>
    Action? OnColorCancelled { get; set; }

    /// <summary>
    /// Action invoked on any color change within the color picker, for preview purposes.
    /// </summary>
    Action<Vector4>? OnColorPreviewed { get; set; }

    /// <summary>
    /// Action invoked when a color is confirmed from the color picker.
    /// </summary>
    Action<Vector4>? OnColorConfirmed { get; set; }
}
