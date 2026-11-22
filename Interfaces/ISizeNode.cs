using System.Numerics;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Represents a node that has a size.
/// </summary>
public interface ISizeNode {

    /// <summary>
    /// Gets or sets the node's width.
    /// </summary>
    float Width { get; set; }

    /// <summary>
    /// Gets or sets the node's height.
    /// </summary>
    float Height { get; set; }

    /// <summary>
    /// Gets or sets the node's size.
    /// </summary>
    Vector2 Size { get; set; }
}
