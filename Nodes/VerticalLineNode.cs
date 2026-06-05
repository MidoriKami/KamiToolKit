namespace KamiToolKit.Nodes;

/// <summary>
/// Specialization of a NineGridNode to represent a 4px thick vertical line.
/// </summary>
/// <remarks>
/// This is a HorizontalLineNode rotated 90 degrees.
/// </remarks>
public sealed unsafe class VerticalLineNode : HorizontalLineNode {

    /// <summary>
    /// Sets the height of the node.
    /// </summary>
    /// <remarks>
    /// Internally this is setting the width, but because its rotated this makes more sense.
    /// </remarks>
    public override float Height {
        get => ResNode->GetWidth();
        set => ResNode->SetWidth((ushort)value);
    }

    /// <summary>
    /// Sets the width of the node.
    /// </summary>
    /// <remarks>
    /// Internally this is setting the height, but because its rotated this makes more sense.
    /// </remarks>
    public override float Width {
        get => ResNode->GetHeight();
        set => ResNode->SetHeight((ushort)value);
    }

    public VerticalLineNode()
        => RotationDegrees = 90.0f;
}
