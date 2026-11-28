namespace KamiToolKit.Core.Layout;

/// <summary>
/// Carries a shared next-index allocator and surrounding directional destinations through a layout tree.
/// </summary>
public sealed class NavigationContext {

    /// <summary>
    /// Gets or sets the current nav index.
    /// </summary>
    public byte CurrentIndex { get; set; }

    /// <summary>
    /// Gets or sets the current Left nav index.
    /// </summary>
    public byte Left { get; set; }

    /// <summary>
    /// Gets or sets the current Right nav index.
    /// </summary>
    public byte Right { get; set; }

    /// <summary>
    /// Gets or sets the current Up nav index.
    /// </summary>
    public byte Up { get; set; }

    /// <summary>
    /// Gets or sets the current Down nav index.
    /// </summary>
    public byte Down { get; set; }
}
