namespace KamiToolKit.Core.Interfaces;

/// <summary>
/// Interface representing a tab bar radio button node.
/// </summary>
public interface ITabBarRadioButtonNode : IClickable, ISelectable, ITextLabel, IPredefinedText, INode {

    /// <summary>
    /// Gets or sets whether this tab is checked.
    /// </summary>
    bool IsChecked { get; set; }
}
