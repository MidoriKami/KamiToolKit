using System.Numerics;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Represents a node that can be scaled.
/// </summary>
public interface IScaleNode {

    /// <summary>
    /// Gets or sets the node's scale in the X direction.
    /// </summary>
    float ScaleX { get; set; }

    /// <summary>
    /// Gets or sets the node's scale in the Y direction.
    /// </summary>
    float ScaleY { get; set; }

    /// <summary>
    /// Gets or sets the node's scale.
    /// </summary>
    Vector2 Scale { get; set; }
}
