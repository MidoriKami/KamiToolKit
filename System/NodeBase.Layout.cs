using System.Numerics;
using Newtonsoft.Json;

namespace KamiToolKit.System;

[method: JsonConstructor]
public class Spacing(float top, float left, float right, float bottom) {

    public Spacing() : this(0.0f, 0.0f, 0.0f, 0.0f) { }

    public Spacing(float allSides) : this(allSides, allSides, allSides, allSides) { }
    public float Top { get; set; } = top;
    public float Left { get; set; } = left;
    public float Right { get; set; } = right;
    public float Bottom { get; set; } = bottom;

    public static implicit operator Spacing(Vector4 vector) => new(vector.X, vector.Y, vector.Z, vector.W);
    public static implicit operator Vector4(Spacing spacing) => new(spacing.Top, spacing.Left, spacing.Right, spacing.Bottom);
    public static Spacing operator +(Spacing a, Spacing b) => new(a.Top + b.Top, a.Left + b.Left, a.Right + b.Right, a.Bottom + b.Bottom);
}

public abstract partial class NodeBase {
    [JsonProperty] public Spacing Margin { get; set; } = new(0.0f);

    public Vector2 LayoutSize => new(Width + Margin.Left + Margin.Right, Height + Margin.Top + Margin.Bottom);
}
