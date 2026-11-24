using System;
using KamiToolKit.Core.Layout;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Core.Interfaces.Nodes;

/// <summary>
/// Interface representing a DropDown node.
/// </summary>
public interface IDropDownNode<T> : ICollapsible, IOptionSelectable<T>, IPlaceholderText, ILayoutFiller, INode {

    /// <summary>
    /// Gets or sets the maximum number of options to show.
    /// </summary>
    int MaxListOptions { get; set; }

    /// <summary>
    /// Gets or sets a function to be used to define how to extract a label from a <typeparamref name="T"/>.
    /// </summary>
    Func<T, ReadOnlySeString>? GetLabelFunction { get; set; }

    /// <summary>
    /// Refreshes the displayed label by re-evaluating the label function for the currently selected option.
    /// </summary>
    void RefreshLabel();
}
