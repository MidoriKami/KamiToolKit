namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a node that can be selected or deselected.
/// </summary>
public interface ISelectable {

    /// <summary>
    /// Gets or sets whether this node is currently selected.
    /// </summary>
    bool IsSelected { get; set; }
}
