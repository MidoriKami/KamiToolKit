namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing an element with recalculatable controller nav.
/// </summary>
public interface INavigationRecalculable {

    /// <summary>
    /// Function that is invoked when this node needs to rebuild its controller navigation layout.
    /// </summary>
    void RecalculateNavigation();
}
