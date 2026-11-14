using System;
using System.Collections.Generic;
using KamiToolKit.BaseTypes;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a tree list node with collapsible categories.
/// </summary>
/// <typeparam name="T">The data model type.</typeparam>
/// <typeparam name="TU">The view type to render the data model.</typeparam>
public interface ITreeListNode<T, TU> : ILayoutUpdatable, IUpdatable where TU : NodeBase {

    /// <summary>
    /// Gets or sets the item spacing.
    /// </summary>
    float ItemSpacing { get; set; }

    /// <summary>
    /// Gets or sets the string to show when there are no items in the list.
    /// </summary>
    ReadOnlySeString? NoResultsString { get; set; }

    /// <summary>
    /// Gets or sets the selected item.
    /// </summary>
    T? SelectedItem { get; set; }

    /// <summary>
    /// Gets or sets the dictionary of options used to populate the tree list.
    /// </summary>
    /// <remarks>
    /// Keys represent collapsing headers, where values are the entries shown per header.
    /// </remarks>
    Dictionary<ReadOnlySeString, List<T>> Options { get; set; }

    /// <summary>
    /// Action that is invoked when an option is clicked on.
    /// </summary>
    Action<T?>? OnOptionSelected { get; set; }

}
