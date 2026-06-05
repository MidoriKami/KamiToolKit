using KamiToolKit.Classes.Internal;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;

namespace KamiToolKit.Premade.Node.Color;

public sealed class AlphaImageNode : ImGuiImageNode {
    public AlphaImageNode() {
        TexturePath = Services.GetAssetPath("alpha_background.png");
        WrapMode = WrapMode.Tile;
    }
}
