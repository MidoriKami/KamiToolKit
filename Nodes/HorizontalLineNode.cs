using System.Numerics;
using KamiToolKit.Nodes.Simplified;

namespace KamiToolKit.Nodes;

/// <summary>
/// Specialization of a NineGridNode to represent a 4px thick horizontal line.
/// </summary>
public class HorizontalLineNode : SimpleNineGridNode {

    /// <summary>
    /// Constructs a new <see cref="HorizontalLineNode"/>
    /// </summary>
    public HorizontalLineNode() {
        TexturePath = "ui/uld/WindowA_Line.tex";
        TextureCoordinates = Vector2.Zero;
        TextureSize = new Vector2(32.0f, 4.0f);
        LeftOffset = 12.0f;
        RightOffset = 12.0f;
    }
}
