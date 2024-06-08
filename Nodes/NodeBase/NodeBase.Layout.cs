using System.Numerics;

namespace KamiToolKit.Nodes;

public record Spacing(float Top, float Left, float Right, float Bottom) {
    public Spacing(float allSides) : this(allSides, allSides, allSides, allSides) { }
}

public abstract unsafe partial class NodeBase {
    public Spacing Margin { get; set; } = new(0.0f);

    public Vector2 LayoutSize => new(Width + Margin.Left + Margin.Right, Height + Margin.Top + Margin.Bottom);
}