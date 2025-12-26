using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

// Simple helper class for making basic text label, node will auto-resize to fit label
public sealed class CategoryTextNode : TextNode {
    public CategoryTextNode() {
        Height = 16.0f;
        TextFlags = TextFlags.AutoAdjustNodeSize;
        TextColor = ColorHelper.GetColor(2);
        TextOutlineColor = ColorHelper.GetColor(7);
        FontType = FontType.Axis;
        FontSize = 14;
        LineSpacing = 24;
        AlignmentType = AlignmentType.Left;
    }

    public override float Height {
        get => base.Height;
        set => base.Height = value + 8.0f; // Add extra height for padding
    }
}
