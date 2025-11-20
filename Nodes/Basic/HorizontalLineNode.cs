using System.Numerics;

namespace KamiToolKit.Nodes;

public class HorizontalLineNode : SimpleNineGridNode {
    public HorizontalLineNode() {
        TexturePath = "ui/uld/WindowA_Line.tex";
        TextureCoordinates = Vector2.Zero;
        TextureSize = new Vector2(32.0f, 4.0f);
        LeftOffset = 12.0f;
        RightOffset = 12.0f;
    }
}
