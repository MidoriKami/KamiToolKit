using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Represents something with a settable label.
/// </summary>
public interface ITextLabel {

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
    /// Gets or sets the displayed string.
    /// </summary>
    ReadOnlySeString String { get; set; }

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
}
