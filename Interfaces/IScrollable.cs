namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a scrollable element.
/// </summary>
public interface IScrollable {

    /// <summary>
    /// If this element should reset its scroll automatically.
    /// </summary>
    bool AutoResetScroll { get; set; }

    /// <summary>
    /// Resets scroll position back to the top.
    /// </summary>
    void ResetScroll();
}
