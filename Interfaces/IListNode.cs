using System;
using System.Collections.Generic;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a virtualized list node.
/// </summary>
public interface IListNode<T, TU> : IOptionMultiSelectable<T>, IPlaceholderText, IUpdatable, IScrollable {

    /// <summary>
    /// Gets or sets the action to be invoked when an item is changed.
    /// Also provides reference to the node used.
    /// </summary>
    /// <remarks>
    /// Warning this violates the nodes managed state and undefined behavior may occur if
    /// you are calling base class functions on the provided node.
    /// </remarks>
    Action<TU, T>? OnItemStateChanged { get; set; }

    /// <summary>
    /// Object that is provided to each node, this can be things like a reference to a config object.
    /// </summary>
    object? NodeReferenceData { get; set; }

    /// <summary>
    /// Gets or sets the item spacing.
    /// </summary>
    float ItemSpacing { get; set; }

    /// <summary>
    /// Set whether items will be able to be selected.
    /// </summary>
    bool EnableSelection { get; set; }

    /// <summary>
    /// Gets a read-only enumerable for the available option nodes.
    /// </summary>
    IReadOnlyList<TU> OptionNodes { get; }

    /// <summary>
    /// Resets and rebuilds list
    /// </summary>
    /// <remarks>
    /// Use sparingly, this is an expensive operation.
    /// </remarks>
    void FullRebuild();
}
