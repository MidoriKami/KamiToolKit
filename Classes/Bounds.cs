using System.Numerics;

namespace KamiToolKit.Classes;

public class Bounds {
    public required Vector2 TopLeft { get; set; }
    public required Vector2 BottomRight { get; set; }

    public float Top => TopLeft.Y;
    public float Left => TopLeft.X;
    public float Bottom => BottomRight.Y;
    public float Right => BottomRight.X;

    public float Width => BottomRight.X - TopLeft.X;
    public float Height => BottomRight.Y - TopLeft.Y;
    public Vector2 Size => new(Width, Height);

    public float CenterX => (TopLeft.X + BottomRight.X) / 2.0f;
    public float CenterY => (TopLeft.Y + BottomRight.Y) / 2.0f;
    public Vector2 Center => new(CenterX, CenterY);

    public override string ToString() => $"{TopLeft}, {BottomRight}";
}
