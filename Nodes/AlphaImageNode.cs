using KamiToolKit.Enums;
using KamiToolKit.Internal.Classes;

namespace KamiToolKit.Nodes;

/// <summary>
/// Specialization of image node to represent the black and white grid pattern that represents alpha transparency.
/// </summary>
public sealed class AlphaImageNode : ImGuiImageNode {

    /// <summary>
    /// Constructs a <see cref="AlphaImageNode"/> instance.
    /// </summary>
    public AlphaImageNode() {
        TexturePath = Services.GetAssetPath("alpha_background.png");
        WrapMode = WrapMode.Tile;
    }
}
