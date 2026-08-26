using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Data.Parsing.Uld;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interfacing representing the capabilities of a TextNode.
/// </summary>
public interface ITextNode {
    /// <summary>
    /// Gets or sets the text color.
    /// </summary>
    /// <remarks>
    /// Expects values between 0.0f and 1.0f.
    /// </remarks>
    Vector4 TextColor { get; set; }

    /// <summary>
    /// Gets or sets the outline color.
    /// </summary>
    /// <remarks>
    /// Expects values between 0.0f and 1.0f.
    /// </remarks>
    Vector4 TextOutlineColor { get; set; }

    /// <summary>
    /// Gets or sets the background color.
    /// </summary>
    Vector4 BackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the selection start.
    /// </summary>
    /// <remarks>
    /// This is used in conjunction with <see cref="BackgroundColor"/> and <see cref="SelectEnd"/>.
    /// </remarks>
    uint SelectStart { get; set; }

    /// <summary>
    /// Gets or sets the selection end.
    /// </summary>
    /// <remarks>
    /// This is used in conjunction with <see cref="BackgroundColor"/> and <see cref="SelectStart"/>.
    /// </remarks>
    uint SelectEnd { get; set; }

    /// <summary>
    /// Gets or sets the text alignment.
    /// </summary>
    AlignmentType AlignmentType { get; set; }

    /// <summary>
    /// Gets or sets the used font.
    /// </summary>
    FontType FontType { get; set; }

    /// <summary>
    /// Gets or sets the text flags.
    /// </summary>
    TextFlags TextFlags { get; set; }

    /// <summary>
    /// Gets or sets the font size.
    /// </summary>
    uint FontSize { get; set; }

    /// <summary>
    /// Gets or sets the vertical line spacing.
    /// </summary>
    uint LineSpacing { get; set; }

    /// <summary>
    /// Gets or sets the character spacing.
    /// </summary>
    uint CharSpacing { get; set; }

    /// <summary>
    /// Gets or sets the sheet type used when setting text via <see cref="TextId"/>
    /// </summary>
    NodeData.SheetType SheetType { get; set; }

    /// <summary>
    /// Gets or sets the textId, this is a row in a <see cref="NodeData.SheetType"/>.
    /// </summary>
    uint TextId { get; set; }

    /// <summary>
    /// Gets or sets the displayed string.
    /// </summary>
    ReadOnlySeString String { get; set; }

    /// <summary>
    /// Adds the specified text flags.
    /// </summary>
    void AddTextFlags(params TextFlags[] flags);

    /// <summary>
    /// Removes the specified text flags.
    /// </summary>
    void RemoveTextFlags(params TextFlags[] flags);

    /// <summary>
    /// Sets the specified number using the provided formatting params.
    /// </summary>
    void SetNumber(int number, bool showCommas = false, bool showPlusSign = false, int digits = 0, bool zeroPad = false);

    /// <summary>
    /// Gets the size of the specified text if it were drawn with this nodes given params.
    /// </summary>
    Vector2 GetTextDrawSize(ReadOnlySeString text, bool considerScale = true);

    /// <summary>
    /// Gets the size of this nodes text.
    /// </summary>
    Vector2 GetTextDrawSize(bool considerScale = true);
}
