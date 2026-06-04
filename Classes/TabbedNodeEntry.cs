namespace KamiToolKit.Classes;

/// <summary>
/// A tabbed node data entry for use with <see cref="KamiToolKit.Nodes.TabbedVerticalListNode"/>
/// </summary>
public record TabbedNodeEntry<T>(T Node, int Tab) where T : NodeBase;
