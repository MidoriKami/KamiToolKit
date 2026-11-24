namespace KamiToolKit.Core.Interfaces;

/// <summary>
/// Represents a node that can be rotated.
/// </summary>
public interface IRotationNode {

    /// <summary>
    /// Gets or sets the node's rotation in radians.
    /// </summary>
    float Rotation { get; set; }

    /// <summary>
    /// Gets or sets the node's rotation in degrees.
    /// </summary>
    float RotationDegrees { get; set; }
}
