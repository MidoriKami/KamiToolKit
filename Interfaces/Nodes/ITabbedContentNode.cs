using System;
using KamiToolKit.BaseTypes;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a tabbed content node container.
/// </summary>
public interface ITabbedContentNode : ILayoutUpdatable, ILayoutFiller {

    /// <summary>
    /// Adds a tab and a vertical list content area, and outputs the built <see cref="VerticalListNode"/>.
    /// </summary>
    void AddTab(ReadOnlySeString label, out VerticalListNode tabContents, ReadOnlySeString? tooltip = null, Action? tabChanged = null);

    /// <summary>
    /// Adds a generic node based on the provided templated type and outputs the built node.
    /// </summary>
    void AddTab<T>(ReadOnlySeString label, out T tabContents, ReadOnlySeString? tooltip = null, Action? tabChanged = null) where T : NodeBase, new();

    /// <summary>
    /// Adds an already defined node as the content node to a new tab.
    /// </summary>
    void AddTab<T>(ReadOnlySeString label, T contentNode, ReadOnlySeString? tooltip = null, Action? tabChanged = null) where T : NodeBase;

    /// <summary>
    /// Action invoked when tab is changed.
    /// </summary>
    Action? OnTabChanged { get; set; }
}
