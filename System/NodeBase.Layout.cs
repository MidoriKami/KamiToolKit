using System.Numerics;
using Newtonsoft.Json;

namespace KamiToolKit.System;

public class Spacing {
    public float Top { get; set; }
    public float Left { get; set; }
    public float Right { get; set; }
    public float Bottom { get; set; }
    
    
    public Spacing(float allSides) : this(allSides, allSides, allSides, allSides) { }

    [JsonConstructor] public Spacing(float top, float left, float right, float bottom) {
        Top = top;
        Left = left;
        Right = right;
        Bottom = bottom;
    }
}

public abstract partial class NodeBase {
    public Spacing Margin { get; set; } = new(0.0f);

    public Vector2 LayoutSize => new(Width + Margin.Left + Margin.Right, Height + Margin.Top + Margin.Bottom);
}