namespace KamiToolKit.Core.Interfaces;

/// <summary>
/// Represents a node that can receive controller navigation focus.
/// </summary>
public interface INavigationTarget {

    /// <summary>
    /// Gets or sets the component's controller navigation index.
    /// </summary>
    byte NavIndex { get; set; }

    /// <summary>
    /// Gets or sets the component's upward navigation target.
    /// </summary>
    byte NavUp { get; set; }

    /// <summary>
    /// Gets or sets the component's downward navigation target.
    /// </summary>
    byte NavDown { get; set; }

    /// <summary>
    /// Gets or sets the component's left navigation target.
    /// </summary>
    byte NavLeft { get; set; }

    /// <summary>
    /// Gets or sets the component's right navigation target.
    /// </summary>
    byte NavRight { get; set; }
}