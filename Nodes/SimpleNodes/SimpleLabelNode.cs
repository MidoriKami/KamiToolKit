using System.Drawing;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

// Simple helper class for making basic text lable, node will auto-resize to fit label
public sealed class SimpleLabelNode : TextNode {
    public SimpleLabelNode() {
        Height = 16.0f;
        TextFlags = TextFlags.AutoAdjustNodeSize | TextFlags.Emboss;
        TextColor = ColorHelper.GetColor(2);
        TextOutlineColor = KnownColor.Black.Vector();
        FontType = FontType.Axis;
        FontSize = 14;
        AlignmentType = AlignmentType.Left;
    }

    public override float Height {
        get => base.Height;
        set => base.Height = value + 8.0f; // Add extra height for padding
    }
}
