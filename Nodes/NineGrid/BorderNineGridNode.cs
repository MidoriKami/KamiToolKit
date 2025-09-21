using System.Numerics;
using KamiToolKit.NodeParts;

namespace KamiToolKit.Nodes;

/// <summary>
///     A node that shows a border loaded from the party list textures
/// </summary>
public unsafe class BorderNineGridNode : NineGridNode {
    public BorderNineGridNode() {
        PartsList.Add(new Part {
            TextureCoordinates = new Vector2(0.0f, 0.0f),
            Size = new Vector2(64.0f, 64.0f),
            Id = 0,
            TexturePath = "ui/uld/PartyListTargetBase.tex",
        });

        TopOffset = 20;
        LeftOffset = 20;
        RightOffset = 20;
        BottomOffset = 20;
        PartsRenderType = 108;
    }
}
