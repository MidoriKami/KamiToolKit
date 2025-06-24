using KamiToolKit.System;

namespace KamiToolKit.Nodes;

public class AlignedVerticalListNode : VerticalListNode<NodeBase>;

public class AlignedVerticalListNode<T> : VerticalListNode<T> where T : NodeBase {

	protected override void AdjustNode(T node) {
		node.X = 0.0f;
	}
}