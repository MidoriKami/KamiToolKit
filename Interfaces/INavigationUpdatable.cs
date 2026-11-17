namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing an element with updatable controller navigation.
/// </summary>
public interface INavigationUpdatable {

    /// <summary>
    /// Function that is invoked when this node needs to update its controller navigation layout.
    /// </summary>
    void UpdateNavigation();
}
