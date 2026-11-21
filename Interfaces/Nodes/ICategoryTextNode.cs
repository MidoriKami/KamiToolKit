namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a text node formatted to be a category label.
/// </summary>
public interface ICategoryTextNode : ITextNode {

    /// <summary>
    /// Gets or sets the nodes height padding.
    /// </summary>
    float HeightPadding { get; set; }
}
