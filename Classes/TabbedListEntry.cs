using System.Collections.Generic;
using KamiToolKit.BaseTypes;

namespace KamiToolKit.Classes;

/// <summary>
/// Record representing a tabbed entry for use in <see cref="Nodes.TabbedVerticalListNode"/>
/// </summary>
public class TabbedListEntry {

    public TabbedListEntry(int index, NodeBase node) {
        TabIndex = index;
        Node = node;
    }

    public TabbedListEntry(int index, ICollection<NodeBase> nodes) {
        TabIndex = index;
        Nodes = nodes;
    }

    public TabbedListEntry() { }

    /// <summary>
    /// Gets or sets the tab index for this tabbed node entry.
    /// </summary>
    public int TabIndex { get; init; }

    /// <summary>
    /// Gets or sets a single node for this tabbed node entry.
    /// </summary>
    public NodeBase? Node { get; init; }

    /// <summary>
    /// Gets or sets a collection of nodes for this tabbed node entry.
    /// </summary>
    public ICollection<NodeBase> Nodes { get; init; } = [];
}
