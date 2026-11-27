using KamiToolKit.Core.Layout;

namespace KamiToolKit.Core.Interfaces;

/// <summary>
/// Represents a navigation group that accepts a parent-provided local context and exposes its edge targets.
/// </summary>
public interface INavigationContextUpdatable : INavigationUpdatable {

    /// <summary>
    /// Gets the focusable components exposed at the edges of this group.
    /// </summary>
    ControllerNavigationBoundary NavigationBoundary { get; }

    /// <summary>
    /// Updates navigation using a context prepared by the owning group.
    /// </summary>
    void UpdateNavigation(ControllerNavigationContext context);
}