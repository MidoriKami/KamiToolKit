using System.Numerics;
using Dalamud.Interface;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Represents an object that can have its color reset to a pre-set default value.
/// </summary>
public interface IColorResettable {

    /// <summary>
    /// Gets or sets the color to display in the square.
    /// </summary>
    ColorHelpers.HsvaColor? DefaultHsvaColor { get; set; }

    /// <summary>
    /// Gets or sets the color to display in the square.
    /// </summary>
    public Vector4? DefaultColor { get; set; }
}
