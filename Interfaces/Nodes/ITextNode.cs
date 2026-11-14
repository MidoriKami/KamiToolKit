using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing the capabilities of a TextNode.
/// </summary>
public interface ITextNode : ITextLabel, IPredefinedText, ISelectableText {

    /// <summary>
    /// Gets or sets the vertical line spacing.
    /// </summary>
    uint LineSpacing { get; set; }

    /// <summary>
    /// Gets or sets the character spacing.
    /// </summary>
    uint CharSpacing { get; set; }

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
    /// Gets the size of the specified text if it were drawn with this node's given params.
    /// </summary>
    Vector2 GetTextDrawSize(ReadOnlySeString text, bool considerScale = true);

    /// <summary>
    /// Gets the size of this node's text.
    /// </summary>
    Vector2 GetTextDrawSize(bool considerScale = true);
}
