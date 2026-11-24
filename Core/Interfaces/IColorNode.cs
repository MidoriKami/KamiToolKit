using System.Numerics;

namespace KamiToolKit.Core.Interfaces;

/// <summary>
/// Represents a node that has color and alpha properties.
/// </summary>
public interface IColorNode {

    /// <summary>
    /// Gets or sets the node's color as RGBA values (0.0f to 1.0f).
    /// </summary>
    Vector4 Color { get; set; }

    /// <summary>
    /// Gets or sets the node's alpha value (0.0f to 1.0f).
    /// </summary>
    float Alpha { get; set; }

    /// <summary>
    /// Gets or sets the node's additive color (0.0f to 1.0f).
    /// </summary>
    Vector3 AddColor { get; set; }

    /// <summary>
    /// Gets or sets the node's multiply color (0.0f to 1.0f).
    /// </summary>
    Vector3 MultiplyColor { get; set; }
}
