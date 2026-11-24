using KamiToolKit.Core.Primitives.NodeBase;
using KamiToolKit.UI.Nodes.Layout;

namespace KamiToolKit.UI.Nodes;

/// <summary>
/// A tabbed node data entry for use with <see cref="TabbedVerticalListNode"/>
/// </summary>
public record TabbedNodeEntry<T>(T Node, int Tab) where T : NodeBase;
