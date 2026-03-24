using KamiToolKit.Dalamud;
using KamiToolKit.Enums;

namespace KamiToolKit.Nodes;

public sealed class AlphaImageNode : ImGuiImageNode {
    public AlphaImageNode() {
        TexturePath = Services.GetAssetPath("alpha_background.png");
        WrapMode = WrapMode.Tile;
    }
}
