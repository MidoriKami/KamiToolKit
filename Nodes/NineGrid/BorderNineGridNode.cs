using System.Numerics;
using KamiToolKit.NodeParts;

namespace KamiToolKit.Nodes;

/// <summary>
///     A node that shows a border loaded from the party list textures
/// </summary>
public class BorderNineGridNode : NineGridNode {
    public BorderNineGridNode() {
        var renderPart = new Part {
            TextureCoordinates = new Vector2(0.0f, 0.0f), Size = new Vector2(64.0f, 64.0f), Id = 0,
        };

        renderPart.LoadTexture("ui/uld/PartyListTargetBase.tex");

        PartsList.Add(renderPart);

        TopOffset = 20;
        LeftOffset = 20;
        RightOffset = 20;
        BottomOffset = 20;
        PartsRenderType = 108;
    }
}
