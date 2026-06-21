using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

/// <summary>
/// Specialization of a <see cref="TextNode"/> that represents a text label often used next to configurable elements.
/// </summary>
public sealed class LabelTextNode : TextNode {

    /// <summary>
    /// Constructs a new <see cref="LabelTextNode"/>
    /// </summary>
    public LabelTextNode() {
        TextColor = ColorHelper.GetColor(8);
        TextOutlineColor = ColorHelper.GetColor(7);
        FontType = FontType.Axis;
        FontSize = 14;
        LineSpacing = 24;
        AlignmentType = AlignmentType.Left;
    }
}
