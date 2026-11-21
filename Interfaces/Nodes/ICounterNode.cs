using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Enums;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a counter node for displaying formatted numbers.
/// </summary>
public interface ICounterNode {

    /// <summary>
    /// Gets or sets the width of each digit.
    /// </summary>
    uint NumberWidth { get; set; }

    /// <summary>
    /// Gets or sets the width of the numeric separator.
    /// </summary>
    uint CommaWidth { get; set; }

    /// <summary>
    /// Gets or sets the width of spaces.
    /// </summary>
    uint SpaceWidth { get; set; }

    /// <summary>
    /// Gets or sets the text alignment.
    /// </summary>
    AlignmentType TextAlignment { get; set; }

    /// <summary>
    /// Gets or sets the width of the counter itself.
    /// </summary>
    float CounterWidth { get; set; }

    /// <summary>
    /// Gets or sets the number displayed.
    /// </summary>
    int Number { get; set; }

    /// <summary>
    /// Gets or sets the string displayed.
    /// </summary>
    ReadOnlySeString String { get; set; }

    /// <summary>
    /// Gets or sets the font used for the counter.
    /// </summary>
    CounterFont Font { get; set; }
}
