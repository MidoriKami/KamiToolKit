using KamiToolKit.Core.Layout;

namespace KamiToolKit.Core.Interfaces;

/// <summary>
/// Represents a node that updates controller navigation recursively.
/// </summary>
public interface INavigationUpdatable {

    /// <summary>
    /// Updates navigation using the shared index allocator and directional destinations in the context.
    /// </summary>
    void UpdateNavigation(NavigationContext context);
}