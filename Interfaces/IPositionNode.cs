using System.Numerics;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Represents a node that has a position.
/// </summary>
public interface IPositionNode {

    /// <summary>
    /// Gets or sets the node's X position.
    /// </summary>
    float X { get; set; }

    /// <summary>
    /// Gets or sets the node's Y position.
    /// </summary>
    float Y { get; set; }

    /// <summary>
    /// Gets or sets the node's position.
    /// </summary>
    Vector2 Position { get; set; }
}
