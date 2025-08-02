using KamiToolKit.System;

namespace KamiToolKit.Classes;

public record TabbedNodeEntry<T>(T Node, int Tab) where T : NodeBase;
