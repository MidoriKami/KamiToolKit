namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a checkbox node.
/// </summary>
public interface ICheckboxNode : ITextLabel, ICheckable {

    /// <summary>
    /// Gets or sets whether the text should be truncated.
    /// </summary>
    bool TruncateText { get; set; }
}
