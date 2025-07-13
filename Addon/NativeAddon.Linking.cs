using KamiToolKit.Classes;
using KamiToolKit.System;

namespace KamiToolKit.Addon;

public abstract partial class NativeAddon {
	public void AttachNode(NodeBase node, NodePosition position = NodePosition.AsLastChild)
		=> NativeController.AttachNode(node, this,  position);
	
	public void AttachNode(NodeBase node, NodeBase target,  NodePosition position = NodePosition.AfterAllSiblings)
		=> NativeController.AttachNode(node, target, position);
}