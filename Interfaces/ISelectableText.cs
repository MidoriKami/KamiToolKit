using System.Numerics;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing selectable text for text nodes.
/// </summary>
/// <remarks>
/// Currently not really implemented or used.
/// </remarks>
public interface ISelectableText {

    /// <summary>
    /// Gets or sets the background color.
    /// </summary>
    Vector4 SelectionColor { get; set; }

    /// <summary>
    /// Gets or sets the selection start.
    /// </summary>
    /// <remarks>
    /// This is used in conjunction with <see cref="SelectionColor"/> and <see cref="SelectEnd"/>.
    /// </remarks>
    uint SelectStart { get; set; }

    /// <summary>
    /// Gets or sets the selection end.
    /// </summary>
    /// <remarks>
    /// This is used in conjunction with <see cref="SelectionColor"/> and <see cref="SelectStart"/>.
    /// </remarks>
    uint SelectEnd { get; set; }
}
