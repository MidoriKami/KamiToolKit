using KamiToolKit.Classes;
using KamiToolKit.Enums;

namespace KamiToolKit.Nodes;

public sealed class AlphaImageNode : ImGuiImageNode {
    public AlphaImageNode() {
        TexturePath = DalamudInterface.Instance.GetAssetPath("alpha_background.png");
        WrapMode = WrapMode.Tile;
    }
}
