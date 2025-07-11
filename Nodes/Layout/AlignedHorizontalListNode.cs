﻿using KamiToolKit.System;

namespace KamiToolKit.Nodes;

public class AlignedHorizontalListNode : AlignedHorizontalListNode<NodeBase>;

public abstract class AlignedHorizontalListNode<T> : HorizontalListNode<T> where T : NodeBase {

	protected override void AdjustNode(T node) {
		node.Y = 0.0f;
	}
	
}