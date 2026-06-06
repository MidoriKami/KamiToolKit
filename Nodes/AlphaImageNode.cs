using KamiToolKit.Classes.Internal;
using KamiToolKit.Enums;

namespace KamiToolKit.Nodes;

/// <summary>
/// Specialization of image node to represent the black and white grid pattern that represents alpha transparency.
/// </summary>
public sealed class AlphaImageNode : ImGuiImageNode {
    public AlphaImageNode() {
        TexturePath = Services.GetAssetPath("alpha_background.png");
        WrapMode = WrapMode.Tile;
    }
}
