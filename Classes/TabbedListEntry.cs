using KamiToolKit.BaseTypes;

namespace KamiToolKit.Classes;

/// <summary>
/// Record representing a tabbed entry for use in <see cref="Nodes.TabbedVerticalListNode"/>
/// </summary>
public record TabbedListEntry(int TabIndex, NodeBase Node);
