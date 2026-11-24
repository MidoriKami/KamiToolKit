namespace KamiToolKit.Core.Interfaces;

/// <summary>
/// Represents a node whose visibility can be toggled.
/// </summary>
public interface IVisibilityNode {

    /// <summary>
    /// Gets or sets whether the node is visible.
    /// </summary>
    bool IsVisible { get; set; }
}
