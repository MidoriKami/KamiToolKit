using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public sealed class LabelTextNode : TextNode {
    public LabelTextNode() {
        TextFlags = TextFlags.AutoAdjustNodeSize | TextFlags.Edge;
        TextColor = ColorHelper.GetColor(8); 
        TextOutlineColor = ColorHelper.GetColor(7);
        FontType = FontType.Axis;
        FontSize = 14;
        LineSpacing = 24;
        AlignmentType = AlignmentType.Left;
    }
}
