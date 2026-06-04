using System;
using System.Numerics;

namespace KamiToolKit.Classes;

/// <summary>
/// Basic Size/Positioning helper for AtkResNodes.
/// </summary>
public class Bounds {

    /// <summary>
    /// Gets or sets the top left coordinate of these bounds.
    /// </summary>
    public required Vector2 TopLeft { get; set; }

    /// <summary>
    /// Gets or sets the bottom right coordinate of these bounds.
    /// </summary>
    public required Vector2 BottomRight { get; set; }

    /// <summary>
    /// Gets the Y position of the top edge.
    /// </summary>
    public float Top => TopLeft.Y;

    /// <summary>
    /// Gets the X position of the left edge.
    /// </summary>
    public float Left => TopLeft.X;

    /// <summary>
    /// Gets the Y position of the bottom edge.
    /// </summary>
    public float Bottom => BottomRight.Y;

    /// <summary>
    /// Gets the X position of the right edge.
    /// </summary>
    public float Right => BottomRight.X;

    /// <summary>
    /// Gets the width of these bounds.
    /// </summary>
    public float Width => Math.Abs(BottomRight.X - TopLeft.X);

    /// <summary>
    /// Gets the height of these bounds.
    /// </summary>
    public float Height => Math.Abs(BottomRight.Y - TopLeft.Y);

    /// <summary>
    /// Gets the total size of these bounds.
    /// </summary>
    public Vector2 Size => new(Width, Height);

    /// <summary>
    /// Gets the centerpoint horizontally of these bounds.
    /// </summary>
    public float CenterX => (TopLeft.X + BottomRight.X) / 2.0f;

    /// <summary>
    /// Gets the centerpoint vertically of these bounds.
    /// </summary>
    public float CenterY => (TopLeft.Y + BottomRight.Y) / 2.0f;

    /// <summary>
    /// Gets the centerpoint of these bounds.
    /// </summary>
    public Vector2 Center => new(CenterX, CenterY);

    /// <summary>
    /// Prints the bounds in a somewhat friendly manner.
    /// </summary>
    public override string ToString() => $"{TopLeft}, {BottomRight}";
}
