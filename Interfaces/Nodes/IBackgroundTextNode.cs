using System.Numerics;
using KamiToolKit.Nodes;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a text node with a background texture to make the text more readable.
/// </summary>
/// <remarks>
/// Expected to be used by <see cref="TextNineGridNode"/> and <see cref="BackgroundTextNode"/>
/// </remarks>
public interface IBackgroundTextNode : ITextLabel {

    /// <summary>
    /// Gets or sets the background visibility.
    /// </summary>
    bool ShowBackground { get; set; }

    /// <summary>
    /// Gets or sets the background color.
    /// </summary>
    Vector4 BackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets whether the node will be resized to fit the string when using <see cref="ITextLabel.String"/>.
    /// </summary>
    bool FitToString { get; set; }

    /// <summary>
    /// Gets or sets the displayed number.
    /// </summary>
    int Number { get; set; }
}
