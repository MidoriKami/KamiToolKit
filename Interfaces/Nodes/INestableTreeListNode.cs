using System;
using System.Collections.Generic;
using KamiToolKit.BaseTypes;
using KamiToolKit.Classes;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a nestable tree list node with nested collapsible sections.
/// </summary>
/// <typeparam name="T">The data model type.</typeparam>
/// <typeparam name="TU">The view type to render the data model.</typeparam>
public interface INestableTreeListNode<T, TU> : ILayoutUpdatable, IScrollable where TU : NodeBase {

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
    /// Gets or sets the nested section tree used to populate this list.
    /// </summary>
    List<TreeListSection<T>> Sections { get; set; }


    /// <summary>
    /// Action that is invoked when an option is clicked on.
    /// </summary>
    Action<T?>? OnOptionSelected { get; set; }

}
