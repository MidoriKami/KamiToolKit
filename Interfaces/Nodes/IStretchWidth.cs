namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a node that can stretch its children to fill the parent's width.
/// </summary>
public interface IStretchWidth {

    /// <summary>
    /// Gets or sets whether child nodes will be stretched to fill this node's width.
    /// This property only affects the layout of child nodes; it does not change this node's own width.
    /// </summary>
    bool StretchWidth { get; set; }
}
