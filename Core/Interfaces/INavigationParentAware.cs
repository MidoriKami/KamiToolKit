namespace KamiToolKit.Core.Interfaces;

internal interface INavigationParentAware {

    INavigationContextUpdatable? NavigationParent { get; }

    void SetNavigationParent(INavigationContextUpdatable? parent);
}