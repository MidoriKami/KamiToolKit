using KamiToolKit.System;

namespace KamiToolKit.Nodes;

public abstract class AlignedVerticalListNode : VerticalListNode {
    protected override void AdjustNode(NodeBase node) {
        node.X = 0.0f;
    }
}
