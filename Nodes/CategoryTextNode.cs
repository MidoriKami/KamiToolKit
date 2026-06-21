using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

/// <summary>
/// Specialization of <see cref="TextNode"/> that represents a gray colored text that is often used as a category label.
/// </summary>
public sealed class CategoryTextNode : TextNode {

    /// <summary>
    /// Gets or sets the nodes height including <see cref="HeightPadding"/>
    /// </summary>
    /// <remarks>
    /// Padding is automatically added when setting the height.
    /// </remarks>
    public override float Height {
        get => base.Height;
        set => base.Height = value + HeightPadding;
    }

    /// <summary>
    /// Gets or sets the nodes height padding.
    /// </summary>
    public float HeightPadding { get; set; } = 8.0f;

    /// <summary>
    /// Constructs a <see cref="CategoryTextNode"/> instance.
    /// </summary>
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
}
