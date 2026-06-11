namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing elements that are navigable via controller input.
/// </summary>
public interface IControllerNavigable {

    /// <summary>
    /// Index of this nav element.
    /// </summary>
    int NavIndex { get; set; }

    /// <summary>
    /// Index to go to when moving left.
    /// </summary>
    int NavLeft { get; set; }

    /// <summary>
    /// Index to go to when moving right.
    /// </summary>
    int NavRight { get; set; }

    /// <summary>
    /// Index to go to when moving up.
    /// </summary>
    int NavUp { get; set; }

    /// <summary>
    /// Index to go to when moving down.
    /// </summary>
    int NavDown { get; set; }
}
