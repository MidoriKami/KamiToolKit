using System.Numerics;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

/// <summary>
/// A ready to use <see cref="NineGridNode"/> with the border used for the party list members.
/// </summary>
/// <remarks>
/// Some modders have decided this texture isn't worth of existing, and replace it with a blank texture.
/// </remarks>
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
