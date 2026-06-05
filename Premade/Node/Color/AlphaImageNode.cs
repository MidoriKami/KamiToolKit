using KamiToolKit.Classes.Internal;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;

namespace KamiToolKit.Premade.Node.Color;

/// <summary>
/// Specialization of image node to represent the black and white grid pattern that represents alpha transparency.
/// </summary>
public sealed class AlphaImageNode : ImGuiImageNode {
    public AlphaImageNode() {
        TexturePath = Services.GetAssetPath("alpha_background.png");
        WrapMode = WrapMode.Tile;
    }
}
